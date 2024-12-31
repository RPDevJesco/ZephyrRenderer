namespace ZephyrRenderer.UIElement
{
    public interface IButton : IDisposable
    {
        string Text { get; set; }
        bool IsEnabled { get; set; }
        event EventHandler? Click;

        void Initialize(double x, double y, double width, double height, string title);
        void Draw(Framebuffer framebuffer); // Optional for framebuffer rendering
    }
}