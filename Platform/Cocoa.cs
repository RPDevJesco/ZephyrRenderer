using System;
using System.Runtime.InteropServices;

namespace ZephyrRenderer.Platform
{
    internal static class Cocoa
    {
        private const string CocoaLib = "/System/Library/Frameworks/Cocoa.framework/Cocoa";
        private const string MetalLib = "/System/Library/Frameworks/Metal.framework/Metal";
        private const string MetalKitLib = "/System/Library/Frameworks/MetalKit.framework/MetalKit";
        
        #region Basic Structures

        [StructLayout(LayoutKind.Sequential)]
        public struct NSPoint
        {
            public double X;
            public double Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NSSize
        {
            public double Width;
            public double Height;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NSRect
        {
            public NSPoint Origin;
            public NSSize Size;
            
            public NSRect(double x, double y, double width, double height)
            {
                Origin = new NSPoint { X = x, Y = y };
                Size = new NSSize { Width = width, Height = height };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MTLOrigin
        {
            public ulong x;
            public ulong y;
            public ulong z;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MTLSize
        {
            public ulong width;
            public ulong height;
            public ulong depth;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MTLRegion
        {
            public MTLOrigin origin;
            public MTLSize size;
        }

        #endregion

        #region Constants

        public const ulong NSWindowStyleMaskTitled = 1 << 0;
        public const ulong NSWindowStyleMaskClosable = 1 << 1;
        public const ulong NSWindowStyleMaskMiniaturizable = 1 << 2;
        public const ulong NSWindowStyleMaskResizable = 1 << 3;
        public const ulong NSBackingStoreBuffered = 2;
        public const long NSApplicationActivationPolicyRegular = 0;
        public static readonly IntPtr NSRunLoopDefaultMode = objc_getClass("NSRunLoopDefaultMode");

        #endregion

        #region Metal Library
        [DllImport(MetalLib)]
        public static extern IntPtr MTLDevice_newLibrary(IntPtr device, IntPtr source, out IntPtr error);

        [DllImport(MetalLib)]
        public static extern IntPtr MTLLibrary_newFunction(IntPtr library, string functionName);

        #endregion

        #region Objective-C Runtime
        
        [DllImport(CocoaLib)]
        public static extern IntPtr objc_getClass(string className);

        [DllImport(CocoaLib)]
        public static extern IntPtr sel_registerName(string selectorName);

        [DllImport(CocoaLib)]
        public static extern IntPtr objc_allocateClassPair(IntPtr superclass, string name, IntPtr extraBytes);

        [DllImport(CocoaLib)]
        public static extern void objc_registerClassPair(IntPtr cls);

        [DllImport(CocoaLib)]
        public static extern bool class_addMethod(IntPtr cls, IntPtr sel, IntPtr imp, string types);

        [DllImport(CocoaLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr objc_msgSend(IntPtr self, IntPtr selector);

        [DllImport(CocoaLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr objc_msgSend(IntPtr self, IntPtr selector, IntPtr arg1);

        #endregion

        #region NSApplication

        [DllImport(CocoaLib)]
        public static extern IntPtr NSApplication_sharedApplication();
        
        [DllImport(CocoaLib)]
        public static extern void NSApplication_setActivationPolicy(IntPtr app, long activationPolicy);
        
        [DllImport(CocoaLib)]
        public static extern void NSApplication_activateIgnoringOtherApps(IntPtr app, bool flag);

        [DllImport(CocoaLib)]
        public static extern IntPtr NSApplication_nextEventMatchingMask(
            IntPtr app,
            ulong mask,
            IntPtr expiration,
            IntPtr mode,
            bool deqFlag);

        [DllImport(CocoaLib)]
        public static extern void NSApplication_sendEvent(IntPtr app, IntPtr evt);

        [DllImport(CocoaLib)]
        public static extern void NSApplication_run(IntPtr app);

        #endregion

        #region NSWindow

        [DllImport(CocoaLib)]
        public static extern IntPtr NSWindow_alloc();
        
        [DllImport(CocoaLib)]
        public static extern IntPtr NSWindow_initWithContentRect(
            IntPtr window,
            NSRect contentRect,
            ulong windowStyle,
            ulong bufferingType,
            bool deferCreation);
            
        [DllImport(CocoaLib)]
        public static extern void NSWindow_setTitle(IntPtr window, IntPtr title);
        
        [DllImport(CocoaLib)]
        public static extern void NSWindow_makeKeyAndOrderFront(IntPtr window, IntPtr sender);

        [DllImport(CocoaLib)]
        public static extern void NSWindow_setContentView(IntPtr window, IntPtr view);

        [DllImport(CocoaLib)]
        public static extern void NSWindow_setDelegate(IntPtr window, IntPtr delegate_);

        [DllImport(CocoaLib)]
        public static extern void NSWindow_close(IntPtr window);

        #endregion

        #region Metal Device & Command Queue

        [DllImport(MetalLib)]
        public static extern IntPtr MTLCreateSystemDefaultDevice();
        
        [DllImport(MetalLib)]
        public static extern IntPtr MTLDevice_newCommandQueue(IntPtr device);

        [DllImport(MetalLib)]
        public static extern IntPtr MTLDevice_newLibrary(IntPtr device, IntPtr source, IntPtr error);

        [DllImport(MetalLib)]
        public static extern IntPtr MTLDevice_newRenderPipelineState(IntPtr device, IntPtr descriptor);

        [DllImport(MetalLib)]
        public static extern IntPtr MTLDevice_newBuffer(IntPtr device, IntPtr pointer, ulong length, ulong options);

        #endregion

        #region Metal Pipeline State

        [DllImport(MetalLib)]
        public static extern IntPtr MTLRenderPipelineDescriptor_alloc();

        [DllImport(MetalLib)]
        public static extern void MTLRenderPipelineDescriptor_setVertexFunction(IntPtr descriptor, IntPtr function);

        [DllImport(MetalLib)]
        public static extern void MTLRenderPipelineDescriptor_setFragmentFunction(IntPtr descriptor, IntPtr function);

        [DllImport(MetalLib)]
        public static extern void MTLRenderPipelineDescriptor_setColorAttachment(
            IntPtr descriptor,
            ulong index,
            uint pixelFormat,
            bool blendingEnabled,
            bool alphaBlendingEnabled);

        #endregion

        #region Metal Command Buffer & Encoder

        [DllImport(MetalLib)]
        public static extern IntPtr MTLCommandQueue_commandBuffer(IntPtr commandQueue);

        [DllImport(MetalLib)]
        public static extern IntPtr MTLCommandBuffer_renderCommandEncoderWithDescriptor(
            IntPtr commandBuffer,
            IntPtr renderPassDescriptor);

        [DllImport(MetalLib)]
        public static extern void MTLCommandBuffer_presentDrawable(IntPtr commandBuffer, IntPtr drawable);

        [DllImport(MetalLib)]
        public static extern void MTLCommandBuffer_commit(IntPtr commandBuffer);

        [DllImport(MetalLib)]
        public static extern void MTLRenderCommandEncoder_setRenderPipelineState(
            IntPtr encoder,
            IntPtr pipelineState);

        [DllImport(MetalLib)]
        public static extern void MTLRenderCommandEncoder_setVertexBuffer(
            IntPtr encoder,
            IntPtr buffer,
            ulong offset,
            ulong index);

        [DllImport(MetalLib)]
        public static extern void MTLRenderCommandEncoder_setFragmentTexture(
            IntPtr encoder,
            IntPtr texture,
            ulong index);

        [DllImport(MetalLib)]
        public static extern void MTLRenderCommandEncoder_drawPrimitives(
            IntPtr encoder,
            uint primitiveType,
            ulong vertexStart,
            ulong vertexCount);

        [DllImport(MetalLib)]
        public static extern void MTLRenderCommandEncoder_endEncoding(IntPtr encoder);

        #endregion

        #region Metal Texture

        [DllImport(MetalLib)]
        public static extern IntPtr MTLTextureDescriptor_texture2DDescriptorWithPixelFormat(
            uint pixelFormat,
            ulong width,
            ulong height,
            bool mipmapped);

        [DllImport(MetalLib)]
        public static extern IntPtr MTLDevice_newTextureWithDescriptor(IntPtr device, IntPtr descriptor);

        [DllImport(MetalLib)]
        public static extern void MTLTexture_replaceRegion(
            IntPtr texture,
            MTLRegion region,
            ulong level,
            IntPtr pixelBytes,
            ulong bytesPerRow);

        #endregion

        #region MetalKit View

        [DllImport(MetalKitLib)]
        public static extern IntPtr MTKView_alloc();

        [DllImport(MetalKitLib)]
        public static extern IntPtr MTKView_initWithFrame(IntPtr view, NSRect frame, IntPtr device);

        [DllImport(MetalKitLib)]
        public static extern void MTKView_setDevice(IntPtr view, IntPtr device);

        [DllImport(MetalKitLib)]
        public static extern void MTKView_setColorPixelFormat(IntPtr view, uint pixelFormat);

        [DllImport(MetalKitLib)]
        public static extern IntPtr MTKView_currentDrawable(IntPtr view);

        [DllImport(MetalKitLib)]
        public static extern IntPtr MTKView_currentRenderPassDescriptor(IntPtr view);

        #endregion

        #region Memory Management

        [DllImport(CocoaLib)]
        public static extern void NSObject_release(IntPtr obj);

        [DllImport(MetalLib)]
        public static extern void MTLTexture_release(IntPtr texture);

        [DllImport(MetalLib)]
        public static extern void MTLTextureDescriptor_release(IntPtr descriptor);

        [DllImport(MetalLib)]
        public static extern void MTLBuffer_release(IntPtr buffer);

        [DllImport(MetalLib)]
        public static extern void MTLRenderPipelineState_release(IntPtr pipelineState);

        [DllImport(MetalLib)]
        public static extern void MTLCommandQueue_release(IntPtr commandQueue);

        [DllImport(MetalLib)]
        public static extern void MTLLibrary_release(IntPtr library);

        #endregion
    }
}