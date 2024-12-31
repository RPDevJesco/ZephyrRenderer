using ZephyrRenderer.Mac.UI;
using ZephyrRenderer.UIElement;

namespace ZephyrRenderer.UI
{
    public static class ButtonFactory
    {
        public static IButton CreateButton()
        {
            if (OperatingSystem.IsMacOS())
                return new MacButton();
            if (OperatingSystem.IsWindows())
                return new Button(); // Example for Windows using framebuffer logic
            throw new PlatformNotSupportedException("Only macOS and Windows are supported.");
        }
    }
}