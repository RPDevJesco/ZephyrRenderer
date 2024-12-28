namespace ZephyrRenderer.UIElement
{
    public class Panel : UI.UIElement
    {
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
    }
}