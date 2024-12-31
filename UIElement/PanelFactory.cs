using ZephyrRenderer.Mac.UI;
using ZephyrRenderer.UIElement;

namespace ZephyrRenderer.UI
{
    public static class PanelFactory
    {
        public static IPanel CreatePanel(IntPtr windowHandle, double x, double y, double width, double height)
        {
            if (OperatingSystem.IsMacOS())
                return new MacPanel(windowHandle, x, y, width, height);
            if (OperatingSystem.IsWindows())
                return new Panel(); // Example, adjust for Windows specifics
            throw new PlatformNotSupportedException("Only macOS and framebuffer platforms are supported.");
        }
    }
}