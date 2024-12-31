namespace ZephyrRenderer.UI
{
    public interface IPanel : IDisposable
    {
        void AddChild(IntPtr childHandle, bool above = true); // Mac-specific method, optional for other platforms
        Color BackgroundColor { get; set; }
        IntPtr GetPanelHandle(); // Optional, only for platforms that use native handles
    }
}