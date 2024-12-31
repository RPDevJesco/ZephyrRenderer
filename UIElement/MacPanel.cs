using ZephyrRenderer.Mac;
using ZephyrRenderer.Platform;
using ZephyrRenderer.UI;

namespace ZephyrRenderer.Mac.UI
{
    public class MacPanel : IPanel
    {
        private IntPtr _panelHandle;
        private IntPtr _windowHandle;
        private bool _disposed = false;

        public Color BackgroundColor { get; set; } = new Color(128, 128, 128);

        public MacPanel(IntPtr windowHandle, double x, double y, double width, double height)
        {
            if (windowHandle == IntPtr.Zero)
                throw new ArgumentException("Invalid window handle.");

            _windowHandle = windowHandle;
            _panelHandle = NSViewWrapper.Create(new Rectangle(x, y, width, height));

            // Add the panel to the window's content view
            IntPtr contentView = Cocoa.objc_msgSend(_windowHandle, Cocoa.sel_registerName("contentView"));
            NSViewWrapper.AddSubview(contentView, _panelHandle);
        }

        public void AddChild(IntPtr childHandle, bool above = true)
        {
            if (childHandle == IntPtr.Zero)
                throw new ArgumentException("Invalid child handle.");

            NSViewWrapper.AddSubviewPositioned(_panelHandle, childHandle, above);
        }

        public IntPtr GetPanelHandle()
        {
            return _panelHandle;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_panelHandle != IntPtr.Zero)
                {
                    IntPtr contentView = Cocoa.objc_msgSend(_windowHandle, Cocoa.sel_registerName("contentView"));
                    NSViewWrapper.RemoveSubview(contentView, _panelHandle);
                    _panelHandle = IntPtr.Zero;
                }

                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
