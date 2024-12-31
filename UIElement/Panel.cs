
using ZephyrRenderer.UI;

namespace ZephyrRenderer.UIElement
{
    public class Panel : UI.UIElement, IPanel
    {
        public Panel() { }

        public Color BackgroundColor { get; set; } = new Color(128, 128, 128);

        protected override void OnDraw(Framebuffer framebuffer)
        {
            framebuffer.FillRect(
                Bounds.X, 
                Bounds.Y, 
                Bounds.Width, 
                Bounds.Height, 
                BackgroundColor
            );
        }

        void IPanel.AddChild(nint childHandle, bool above)
        {
            // Framebuffer-based panels don't have a concept of native child handles
            throw new NotSupportedException("AddChild is not supported on framebuffer panels.");
        }

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }

        nint IPanel.GetPanelHandle()
        {
            return IntPtr.Zero;
        }
    }
}