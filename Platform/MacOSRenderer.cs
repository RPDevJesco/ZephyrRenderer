using System.Runtime.InteropServices;
using static ZephyrRenderer.Platform.Cocoa;

namespace ZephyrRenderer.Platform
{
    public class MacOSRenderer : IRenderer
    {
        private IntPtr nsApp;
        private IntPtr window;
        private IntPtr metalDevice;
        private IntPtr commandQueue;
        private IntPtr metalView;
        private IntPtr pipelineState;
        private IntPtr vertexBuffer;
        private IntPtr textureDescriptor;
        private IntPtr texture;
        private bool shouldClose;
        private bool disposed;

        // Vertex data for full-screen quad
        private readonly float[] vertices = new float[]
        {
            -1.0f,  1.0f,  0.0f, 0.0f, // Top left
            -1.0f, -1.0f,  0.0f, 1.0f, // Bottom left
             1.0f, -1.0f,  1.0f, 1.0f, // Bottom right
             1.0f,  1.0f,  1.0f, 0.0f  // Top right
        };

        public event Action<int, int>? OnMouseMove;
        public event Action<int, int, bool>? OnMouseButton;

        public bool ShouldClose => shouldClose;

        private delegate void WindowWillCloseDelegate(IntPtr self, IntPtr sel, IntPtr notification);
        private delegate void MouseEventDelegate(IntPtr self, IntPtr sel, IntPtr theEvent);

        public void Initialize(string title, int width, int height)
        {
            InitializeApplication(title, width, height);
            InitializeMetal(width, height);
            SetupRenderPipeline();
            CreateVertexBuffer();
            CreateTextureDescriptor((uint)width, (uint)height);
        }

        private void InitializeApplication(string title, int width, int height)
        {
            nsApp = NSApplication_sharedApplication();
            NSApplication_setActivationPolicy(nsApp, NSApplicationActivationPolicyRegular);
            
            var rect = new NSRect(100, 100, width, height);
            window = NSWindow_alloc();
            window = NSWindow_initWithContentRect(
                window,
                rect,
                NSWindowStyleMaskTitled | NSWindowStyleMaskClosable | NSWindowStyleMaskMiniaturizable | NSWindowStyleMaskResizable,
                NSBackingStoreBuffered,
                true
            );

            var titlePtr = Marshal.StringToHGlobalAuto(title);
            NSWindow_setTitle(window, titlePtr);
            Marshal.FreeHGlobal(titlePtr);

            SetupWindowDelegate();
            NSWindow_makeKeyAndOrderFront(window, IntPtr.Zero);
            NSApplication_activateIgnoringOtherApps(nsApp, true);
        }

        private void InitializeMetal(int width, int height)
        {
            metalDevice = MTLCreateSystemDefaultDevice();
            if (metalDevice == IntPtr.Zero)
            {
                throw new Exception("Metal is not supported on this device");
            }

            commandQueue = MTLDevice_newCommandQueue(metalDevice);

            var viewRect = new NSRect(0, 0, width, height);
            metalView = MTKView_alloc();
            metalView = MTKView_initWithFrame(metalView, viewRect, metalDevice);
            MTKView_setDevice(metalView, metalDevice);
            MTKView_setColorPixelFormat(metalView, 80); // MTLPixelFormatBGRA8Unorm

            NSWindow_setContentView(window, metalView);
        }

        private void SetupRenderPipeline()
        {
            string shaderSource = @"
                #include <metal_stdlib>
                using namespace metal;

                struct VertexInput {
                    float2 position [[attribute(0)]];
                    float2 texCoord [[attribute(1)]];
                };

                struct RasterizerData {
                    float4 position [[position]];
                    float2 texCoord;
                };

                vertex RasterizerData vertexShader(uint vertexID [[vertex_id]],
                                                 constant VertexInput *vertices [[buffer(0)]]) {
                    RasterizerData out;
                    out.position = float4(vertices[vertexID].position, 0.0, 1.0);
                    out.texCoord = vertices[vertexID].texCoord;
                    return out;
                }

                fragment float4 fragmentShader(RasterizerData in [[stage_in]],
                                            texture2d<float> texture [[texture(0)]]) {
                    constexpr sampler textureSampler(mag_filter::nearest,
                                                   min_filter::nearest);
                    return texture.sample(textureSampler, in.texCoord);
                }";

            var librarySourcePtr = Marshal.StringToHGlobalAuto(shaderSource);
            IntPtr libraryError;
            var library = MTLDevice_newLibrary(metalDevice, librarySourcePtr, out libraryError);
            Marshal.FreeHGlobal(librarySourcePtr);

            if (library == IntPtr.Zero)
            {
                throw new Exception("Failed to create shader library");
            }

            var vertexFunction = MTLLibrary_newFunction(library, "vertexShader");
            var fragmentFunction = MTLLibrary_newFunction(library, "fragmentShader");

            var pipelineDescriptor = MTLRenderPipelineDescriptor_alloc();
            MTLRenderPipelineDescriptor_setVertexFunction(pipelineDescriptor, vertexFunction);
            MTLRenderPipelineDescriptor_setFragmentFunction(pipelineDescriptor, fragmentFunction);
            MTLRenderPipelineDescriptor_setColorAttachment(pipelineDescriptor, 0, 80, true, true);

            pipelineState = MTLDevice_newRenderPipelineState(metalDevice, pipelineDescriptor);

            MTLLibrary_release(library);
            NSObject_release(vertexFunction);
            NSObject_release(fragmentFunction);
            NSObject_release(pipelineDescriptor);
        }

        private void CreateVertexBuffer()
        {
            var handle = GCHandle.Alloc(vertices, GCHandleType.Pinned);
            try
            {
                vertexBuffer = MTLDevice_newBuffer(
                    metalDevice,
                    handle.AddrOfPinnedObject(),
                    (ulong)(vertices.Length * sizeof(float)),
                    0 // MTLResourceStorageModeShared
                );
            }
            finally
            {
                handle.Free();
            }
        }

        private void CreateTextureDescriptor(uint width, uint height)
        {
            textureDescriptor = MTLTextureDescriptor_texture2DDescriptorWithPixelFormat(
                80, // MTLPixelFormatBGRA8Unorm
                width,
                height,
                false
            );
        }

        private void SetupWindowDelegate()
        {
            var delegateClass = objc_allocateClassPair(objc_getClass("NSObject"), "WindowDelegate", 0);
            
            var windowWillCloseSelector = sel_registerName("windowWillClose:");
            var windowWillCloseDelegate = new WindowWillCloseDelegate((self, sel, notification) => 
            {
                shouldClose = true;
            });
            
            var mouseMovedSelector = sel_registerName("mouseMoved:");
            var mouseMovedDelegate = new MouseEventDelegate((self, sel, theEvent) => 
            {
                // Handle mouse move
                // TODO: Get mouse coordinates and invoke OnMouseMove
            });
            
            class_addMethod(delegateClass, windowWillCloseSelector, 
                Marshal.GetFunctionPointerForDelegate(windowWillCloseDelegate), "v@:@");
            class_addMethod(delegateClass, mouseMovedSelector,
                Marshal.GetFunctionPointerForDelegate(mouseMovedDelegate), "v@:@");
            
            objc_registerClassPair(delegateClass);
            
            var delegateInstance = objc_msgSend(delegateClass, sel_registerName("alloc"));
            delegateInstance = objc_msgSend(delegateInstance, sel_registerName("init"));
            
            NSWindow_setDelegate(window, delegateInstance);
        }

        public void Present(Framebuffer framebuffer)
        {
            if (texture == IntPtr.Zero)
            {
                texture = MTLDevice_newTextureWithDescriptor(metalDevice, textureDescriptor);
            }

            var region = new MTLRegion 
            { 
                origin = new MTLOrigin(), 
                size = new MTLSize 
                { 
                    width = (ulong)framebuffer.Width, 
                    height = (ulong)framebuffer.Height, 
                    depth = 1 
                } 
            };

            MTLTexture_replaceRegion(
                texture,
                region,
                0,
                framebuffer.BufferPtr,
                (ulong)(framebuffer.Width * 4)
            );

            var drawable = MTKView_currentDrawable(metalView);
            var renderPassDescriptor = MTKView_currentRenderPassDescriptor(metalView);

            var commandBuffer = MTLCommandQueue_commandBuffer(commandQueue);
            var renderEncoder = MTLCommandBuffer_renderCommandEncoderWithDescriptor(
                commandBuffer,
                renderPassDescriptor
            );

            MTLRenderCommandEncoder_setRenderPipelineState(renderEncoder, pipelineState);
            MTLRenderCommandEncoder_setVertexBuffer(renderEncoder, vertexBuffer, 0, 0);
            MTLRenderCommandEncoder_setFragmentTexture(renderEncoder, texture, 0);
            MTLRenderCommandEncoder_drawPrimitives(renderEncoder, 3, 0, 4);

            MTLRenderCommandEncoder_endEncoding(renderEncoder);
            MTLCommandBuffer_presentDrawable(commandBuffer, drawable);
            MTLCommandBuffer_commit(commandBuffer);
        }

        public void ProcessEvents()
        {
            while (true)
            {
                var eventPtr = NSApplication_nextEventMatchingMask(
                    nsApp,
                    0xFFFFFFFF,
                    IntPtr.Zero,
                    NSRunLoopDefaultMode,
                    true
                );

                if (eventPtr == IntPtr.Zero)
                    break;

                NSApplication_sendEvent(nsApp, eventPtr);
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                if (texture != IntPtr.Zero)
                    MTLTexture_release(texture);
                if (textureDescriptor != IntPtr.Zero)
                    MTLTextureDescriptor_release(textureDescriptor);
                if (vertexBuffer != IntPtr.Zero)
                    MTLBuffer_release(vertexBuffer);
                if (pipelineState != IntPtr.Zero)
                    MTLRenderPipelineState_release(pipelineState);
                if (commandQueue != IntPtr.Zero)
                    MTLCommandQueue_release(commandQueue);
                if (metalView != IntPtr.Zero)
                    NSObject_release(metalView);
                if (window != IntPtr.Zero)
                    NSObject_release(window);

                disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        ~MacOSRenderer()
        {
            Dispose();
        }
    }
}