namespace ZephyrRenderer
{
    public interface IRenderer : IDisposable
    {
        void Initialize(string title, int width, int height);
        void Present(Framebuffer framebuffer);
        bool ShouldClose { get; }
        void ProcessEvents();
    }
}