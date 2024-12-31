using System.Runtime.InteropServices;

using static ZephyrRenderer.Platform.Cocoa;

namespace ZephyrRenderer.Renderer
{
    public class MacRenderer : IRenderer
    {
        private IntPtr _viewHandle;
        private BitmapRenderer _bitmapRenderer;
        private bool _shouldClose;
        private static DrawRectDelegate _drawRectDelegate;

        public bool ShouldClose => _shouldClose;

        public event Action<int, int>? OnMouseMove;
        public event Action<int, int, bool>? OnMouseButton;

        public MacRenderer()
        {
            _shouldClose = false;
        }

        public MacRenderer(BitmapRenderer bitmapRenderer)
        {
            _bitmapRenderer = bitmapRenderer;
            _shouldClose = false;
        }

        public void Initialize(string title, int width, int height)
        {
            var frame = new Rectangle(0, 0, width, height);

            // Create NSView subclass
            var className = "CustomView";
            var baseClass = objc_getClass("NSView");
            var newClass = objc_allocateClassPair(baseClass, className, 0);

            if (newClass == IntPtr.Zero)
            {
                // Class might already exist, try to get it
                newClass = objc_getClass(className);
                if (newClass == IntPtr.Zero)
                {
                    throw new Exception("Failed to create or get CustomView class");
                }
            }
            else
            {
                // Add drawRect: method
                _drawRectDelegate = new DrawRectDelegate(DrawRect);
                var drawRectPtr = Marshal.GetFunctionPointerForDelegate(_drawRectDelegate);
                if (!class_addMethod(newClass, sel_registerName("drawRect:"), drawRectPtr, "v@:{CGRect=dddd}"))
                {
                    throw new Exception("Failed to add drawRect: method");
                }

                // Register the class
                objc_registerClassPair(newClass);
            }

            // Create an instance
            _viewHandle = objc_msgSend(newClass, sel_registerName("alloc"));
            if (_viewHandle == IntPtr.Zero)
            {
                throw new Exception("Failed to allocate CustomView instance");
            }

            _viewHandle = objc_msgSend_Init(_viewHandle, sel_registerName("initWithFrame:"), frame);
            if (_viewHandle == IntPtr.Zero)
            {
                throw new Exception("Failed to initialize CustomView instance");
            }

            // Set needs display
            var setNeedsDisplaySelector = sel_registerName("setNeedsDisplay:");
            objc_msgSend_bool(_viewHandle, setNeedsDisplaySelector, true);
        }

        public void Present(Framebuffer framebuffer)
        {
            // Force redrawing by setting the view as needing display
            var setNeedsDisplaySelector = sel_registerName("setNeedsDisplay:");
            objc_msgSend_bool(_viewHandle, setNeedsDisplaySelector, true);
        }

        public void ProcessEvents()
        {
            // Placeholder for event processing if needed. macOS's event loop is typically managed by the application.
        }
        public IntPtr CreateWindow(string title, int width, int height)
        {
            var frame = new Rectangle(0, 0, width, height);
            Initialize(title, width, height); // Reuse the NSView logic
            return _viewHandle; // Return the NSView handle
        }

        public IntPtr GetWindowHandle()
        {
            return _viewHandle;
        }



        private void DrawRect(IntPtr self, IntPtr cmd, Rectangle dirtyRect)
        {
            try
            {
                var currentContext = NSGraphicsContext_currentContext();
                if (currentContext == IntPtr.Zero)
                {
                    Console.WriteLine("Failed to get current graphics context");
                    return;
                }

                var cgContext = NSGraphicsContext_CGContext(currentContext);
                if (cgContext == IntPtr.Zero)
                {
                    Console.WriteLine("Failed to get CGContext");
                    return;
                }

                _bitmapRenderer.Render(cgContext, 0, 0); // Render framebuffer content
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DrawRect: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void Dispose()
        {
            // Properly release any resources used by the custom NSView
            if (_viewHandle != IntPtr.Zero)
            {
                objc_msgSend(_viewHandle, sel_registerName("release"));
                _viewHandle = IntPtr.Zero;
            }

            GC.SuppressFinalize(this);
        }

        private delegate void DrawRectDelegate(IntPtr self, IntPtr cmd, Rectangle dirtyRect);
    }
}
