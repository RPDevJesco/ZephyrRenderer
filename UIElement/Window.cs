using ZephyrRenderer.Platform;
using ZephyrRenderer.UI;

namespace ZephyrRenderer.UIElement
{
    public class Window : UI.UIElement
    {
        private readonly IRenderer renderer;
        private readonly Framebuffer framebuffer;
        private bool isMouseDown;
        private int lastMouseX;
        private int lastMouseY;

        public Window(string title, int width, int height)
        {
            renderer = CreatePlatformRenderer();
            framebuffer = new Framebuffer(width, height);
            
            renderer.Initialize(title, width, height);
            Bounds = new Rectangle { Width = width, Height = height };

            // Hook up to the renderer's mouse events
            if (renderer is WindowsRenderer winRenderer)
            {
                winRenderer.OnMouseMove += HandleMouseMove;
                winRenderer.OnMouseButton += HandleMouseButton;
            }
        }

        private IRenderer CreatePlatformRenderer()
        {
            if (OperatingSystem.IsWindows())
                return new WindowsRenderer();
            else if (OperatingSystem.IsMacOS())
                return new MacOSRenderer();
            else if (OperatingSystem.IsLinux())
                throw new PlatformNotSupportedException("Linux support coming soon");
    
            throw new PlatformNotSupportedException(
                $"Platform not supported: {Environment.OSVersion.Platform}. " +
                "Currently supported platforms are Windows and macOS."
            );
        }

        private void HandleMouseMove(int x, int y)
        {
            lastMouseX = x;
            lastMouseY = y;
            HandleMouseEvent(x, y, isMouseDown);
        }

        private void HandleMouseButton(int x, int y, bool isDown)
        {
            isMouseDown = isDown;
            lastMouseX = x;
            lastMouseY = y;
            HandleMouseEvent(x, y, isDown);
        }

        protected override void OnDraw(Framebuffer framebuffer)
        {
            framebuffer.FillRect(
                Bounds.X,
                Bounds.Y,
                Bounds.Width,
                Bounds.Height,
                new Color(240, 240, 240) // Light gray background
            );
        }

        public void Run()
        {
            while (!renderer.ShouldClose)
            {
                // Draw the UI tree starting from the window
                Draw(framebuffer);

                // Present the frame
                renderer.Present(framebuffer);
                renderer.ProcessEvents();

                // Cap frame rate
                Thread.Sleep(1000 / 60);
            }
        }
    }
}