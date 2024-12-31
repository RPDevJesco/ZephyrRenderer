using System.Runtime.InteropServices;

namespace ZephyrRenderer.Platform
{
    internal static class Cocoa
    {
        // Constants for window/view ordering
        public const int NSWindowAbove = 1;
        public const int NSWindowBelow = -1;
        public const int NSWindowOut = 0;

        // Basic AppKit functions
        [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
        public static extern void NSApplicationLoad();

        // Basic Objective-C runtime functions
        [DllImport("/usr/lib/libobjc.A.dylib")]
        public static extern IntPtr objc_getClass(string name);

        [DllImport("/usr/lib/libobjc.A.dylib")]
        public static extern IntPtr sel_registerName(string name);

        // objc_msgSend variants - all with EntryPoint="objc_msgSend"
        [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
        public static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector);

        [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
        public static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, Rectangle rect);

        [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
        public static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, string arg1);

        [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
        public static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, IntPtr arg1);

        [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
        public static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, Rectangle rect, ulong style, ulong bufferingType, bool defer);

        [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
        public static extern void objc_msgSend_void(IntPtr receiver, IntPtr selector, IntPtr arg1);

        [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
        public static extern void objc_msgSend_bool(IntPtr receiver, IntPtr selector, bool arg1);

        [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
        public static extern IntPtr objc_msgSend_Init(IntPtr receiver, IntPtr selector, Rectangle rect);

        [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
        public static extern Size objc_msgSend_CGSize(IntPtr receiver, IntPtr selector);

        // Class creation and method adding
        [DllImport("/usr/lib/libobjc.A.dylib")]
        public static extern IntPtr objc_allocateClassPair(IntPtr superclass, string name, int extraBytes);

        [DllImport("/usr/lib/libobjc.A.dylib")]
        public static extern void objc_registerClassPair(IntPtr cls);

        [DllImport("/usr/lib/libobjc.A.dylib")]
        public static extern bool class_addMethod(IntPtr cls, IntPtr sel, IntPtr imp, string types);

        // Helper methods that use objc_msgSend internally
        public static IntPtr NSGraphicsContext_currentContext()
        {
            IntPtr nsGraphicsContextClass = objc_getClass("NSGraphicsContext");
            IntPtr currentContextSelector = sel_registerName("currentContext");
            return objc_msgSend(nsGraphicsContextClass, currentContextSelector);
        }

        public static IntPtr NSGraphicsContext_CGContext(IntPtr graphicsContext)
        {
            if (graphicsContext == IntPtr.Zero)
                return IntPtr.Zero;

            IntPtr cgContextSelector = sel_registerName("CGContext");
            return objc_msgSend(graphicsContext, cgContextSelector);
        }

        public static Size NSImage_size(IntPtr nsImage)
        {
            if (nsImage == IntPtr.Zero)
                throw new ArgumentNullException(nameof(nsImage));

            IntPtr sizeSelector = sel_registerName("size");
            return objc_msgSend_CGSize(nsImage, sizeSelector);
        }

        [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
        public static extern IntPtr objc_msgSend_CGImage(IntPtr receiver, IntPtr selector, ref Rectangle rect, IntPtr context, IntPtr hints);

        [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        public static extern void CGContextDrawImage(IntPtr context, Rectangle rect, IntPtr image);

        [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
        public static extern void objc_msgSend_AddSubview(IntPtr receiver, IntPtr selector, IntPtr view, int position, IntPtr relativeTo);

        public static IntPtr NSImage_CGImageForProposedRect(IntPtr nsImage, ref Rectangle proposedRect, IntPtr context, IntPtr hints)
        {
            if (nsImage == IntPtr.Zero)
                throw new ArgumentNullException(nameof(nsImage));

            IntPtr selector = sel_registerName("CGImageForProposedRect:context:hints:");
            return objc_msgSend_CGImage(nsImage, selector, ref proposedRect, context, hints);
        }
    }

    internal static class NSApplicationWrapper
    {
        public static void Load()
        {
            Cocoa.NSApplicationLoad();
        }

        public static IntPtr Create()
        {
            IntPtr appClass = Cocoa.objc_getClass("NSApplication");
            IntPtr sharedAppSelector = Cocoa.sel_registerName("sharedApplication");
            return Cocoa.objc_msgSend(appClass, sharedAppSelector);
        }

        public static void Activate(IntPtr app)
        {
            IntPtr activateIgnoringOtherAppsSelector = Cocoa.sel_registerName("activateIgnoringOtherApps:");
            Cocoa.objc_msgSend_bool(app, activateIgnoringOtherAppsSelector, true);
        }

        public static void Run(IntPtr app)
        {
            IntPtr runSelector = Cocoa.sel_registerName("run");
            Cocoa.objc_msgSend(app, runSelector);
        }
    }

    internal static class NSButtonWrapper
    {
        public static IntPtr Create(Rectangle rect, string title)
        {
            IntPtr buttonClass = Cocoa.objc_getClass("NSButton");
            IntPtr allocSelector = Cocoa.sel_registerName("alloc");
            IntPtr initWithFrameSelector = Cocoa.sel_registerName("initWithFrame:");
            IntPtr setTitleSelector = Cocoa.sel_registerName("setTitle:");

            IntPtr button = Cocoa.objc_msgSend(buttonClass, allocSelector);
            button = Cocoa.objc_msgSend(button, initWithFrameSelector, rect);

            IntPtr nsStringClass = Cocoa.objc_getClass("NSString");
            IntPtr stringWithUTF8StringSelector = Cocoa.sel_registerName("stringWithUTF8String:");
            IntPtr buttonTitle = Cocoa.objc_msgSend(nsStringClass, stringWithUTF8StringSelector, title);
            Cocoa.objc_msgSend(button, setTitleSelector, buttonTitle);

            return button;
        }

        public static void SetTarget(IntPtr button, IntPtr target, string action)
        {
            IntPtr setTargetSelector = Cocoa.sel_registerName("setTarget:");
            IntPtr setActionSelector = Cocoa.sel_registerName("setAction:");
            IntPtr actionSelector = Cocoa.sel_registerName(action);

            Cocoa.objc_msgSend(button, setTargetSelector, target);
            Cocoa.objc_msgSend(button, setActionSelector, actionSelector);
        }
    }

    public static class NSViewWrapper
    {
        private static readonly IntPtr nsViewClass = Cocoa.objc_getClass("NSView");
        private static readonly IntPtr addSubviewSelector = Cocoa.sel_registerName("addSubview:");
        private static readonly IntPtr removeFromSuperviewSelector = Cocoa.sel_registerName("removeFromSuperview");
        private static readonly IntPtr setWantsBestResolutionOpenGLSurfaceSelector = Cocoa.sel_registerName("setWantsBestResolutionOpenGLSurface:");

        public static IntPtr Create(Rectangle frame)
        {
            IntPtr view = Cocoa.objc_msgSend(nsViewClass, Cocoa.sel_registerName("alloc"));
            view = Cocoa.objc_msgSend(view, Cocoa.sel_registerName("initWithFrame:"), frame);
            return view;
        }

        public static void AddSubview(IntPtr parentView, IntPtr childView)
        {
            Cocoa.objc_msgSend(parentView, addSubviewSelector, childView);
        }

        public static void AddSubviewPositioned(IntPtr parentView, IntPtr childView, bool above)
        {
            var selector = Cocoa.sel_registerName("addSubview:positioned:relativeTo:");
            var position = above ? Cocoa.NSWindowAbove : Cocoa.NSWindowBelow;
            Cocoa.objc_msgSend_AddSubview(parentView, selector, childView, position, IntPtr.Zero);
        }

        public static void RemoveSubview(IntPtr parentView, IntPtr childView)
        {
            Cocoa.objc_msgSend(childView, removeFromSuperviewSelector);
        }
    }

    internal static class NSWindowWrapper
    {
        public static IntPtr Create(Rectangle rect)
        {
            IntPtr windowClass = Cocoa.objc_getClass("NSWindow");
            IntPtr allocSelector = Cocoa.sel_registerName("alloc");
            IntPtr initSelector = Cocoa.sel_registerName("initWithContentRect:styleMask:backing:defer:");
            IntPtr makeKeyAndOrderFrontSelector = Cocoa.sel_registerName("makeKeyAndOrderFront:");

            IntPtr window = Cocoa.objc_msgSend(windowClass, allocSelector);
            window = Cocoa.objc_msgSend(window, initSelector, rect, 15, 2, false);
            Cocoa.objc_msgSend(window, makeKeyAndOrderFrontSelector, IntPtr.Zero);

            // Create a container view and set it as the content view
            IntPtr containerView = NSViewWrapper.Create(rect);
            SetContentView(window, containerView);

            return window;
        }

        public static void SetContentView(IntPtr window, IntPtr view)
        {
            IntPtr setContentViewSelector = Cocoa.sel_registerName("setContentView:");
            Cocoa.objc_msgSend(window, setContentViewSelector, view);
        }

        public static void AddSubview(IntPtr window, IntPtr subview)
        {
            IntPtr contentView = GetContentView(window);
            NSViewWrapper.AddSubview(contentView, subview);
        }

        private static IntPtr GetContentView(IntPtr window)
        {
            IntPtr contentViewSelector = Cocoa.sel_registerName("contentView");
            return Cocoa.objc_msgSend(window, contentViewSelector);
        }
    }

    internal static class ButtonClickHandler
    {
        private delegate void ButtonClickedDelegate(IntPtr self, IntPtr cmd);

        public static event EventHandler ButtonClickedEvent;

        public static IntPtr Create()
        {
            IntPtr classHandle = Cocoa.objc_getClass("NSObject");
            IntPtr instance = Cocoa.objc_msgSend(classHandle, Cocoa.sel_registerName("alloc"));
            instance = Cocoa.objc_msgSend(instance, Cocoa.sel_registerName("init"));

            ButtonClickedDelegate buttonClickedDelegate = ButtonClicked;
            IntPtr buttonClickedSelector = Cocoa.sel_registerName("buttonClicked:");
            IntPtr imp = Marshal.GetFunctionPointerForDelegate(buttonClickedDelegate);
            Cocoa.class_addMethod(classHandle, buttonClickedSelector, imp, "v@:@");

            return instance;
        }

        private static void ButtonClicked(IntPtr self, IntPtr cmd)
        {
            Console.WriteLine("Button clicked!");
            ButtonClickedEvent?.Invoke(null, EventArgs.Empty);
        }
    }
}