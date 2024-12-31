namespace ZephyrRenderer.Renderer
{
    public interface IRenderer : IDisposable
    {
        void Initialize(string title, int width, int height);
        void Present(Framebuffer framebuffer);
        void ProcessEvents();
        bool ShouldClose { get; }
        IntPtr CreateWindow(string title, int width, int height); // Added
        IntPtr GetWindowHandle(); // Added
    }
}