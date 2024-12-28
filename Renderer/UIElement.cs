namespace ZephyrRenderer.UI
{
    public abstract class UIElement
    {
        public Rectangle Bounds { get; set; } = new Rectangle();
        public bool IsVisible { get; set; } = true;
        public UIElement? Parent { get; set; }
        public List<UIElement> Children { get; } = new List<UIElement>();
        
        public virtual void Draw(Framebuffer framebuffer)
        {
            if (!IsVisible) return;
            
            // Draw this element
            OnDraw(framebuffer);
            
            // Draw children
            foreach (var child in Children)
            {
                child.Draw(framebuffer);
            }
        }

        protected abstract void OnDraw(Framebuffer framebuffer);

        public virtual bool HandleMouseEvent(int x, int y, bool isDown)
        {
            if (!IsVisible) return false;

            // Check children first (in reverse order for proper z-order)
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                if (Children[i].HandleMouseEvent(x, y, isDown))
                    return true;
            }

            // Then check this element
            var point = new Point { X = x, Y = y };
            if (Bounds.Contains(point))
            {
                return OnMouseEvent(x, y, isDown);
            }

            return false;
        }

        protected virtual bool OnMouseEvent(int x, int y, bool isDown) => false;
    }
}