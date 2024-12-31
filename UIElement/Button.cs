using ZephyrRenderer.UI;

namespace ZephyrRenderer.UIElement
{
    public class Button : UI.UIElement, IButton
    {
        public string Text { get; set; } = "";
        public Color TextColor { get; set; } = new Color(0, 0, 0);
        public Color BackgroundColor { get; set; } = new Color(200, 200, 200);
        public Color HoverColor { get; set; } = new Color(220, 220, 220);
        public Color PressedColor { get; set; } = new Color(180, 180, 180);
        public Color BorderColor { get; set; } = new Color(100, 100, 100);
        public Color BorderHoverColor { get; set; } = new Color(80, 80, 80);
        public bool IsEnabled { get; set; } = true;
        public event Action? OnClick;
        public event Action? OnMouseEnter;
        public event Action? OnMouseLeave;

        private bool isHovered;
        private bool isPressed;
        private bool wasPressed;
        string IButton.Text { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        bool IButton.IsEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event EventHandler? Click;

        event EventHandler? IButton.Click
        {
            add
            {
                Click += value; // Add the new handler to the invocation list
            }
            remove
            {
                Click -= value; // Remove the handler from the invocation list
            }
        }

        protected override void OnDraw(Framebuffer framebuffer)
        {
            // Determine the current state colors
            var backgroundColor = IsEnabled ?
                (isPressed ? PressedColor :
                 isHovered ? HoverColor :
                 BackgroundColor) :
                new Color(150, 150, 150); // Disabled state

            var borderColor = isHovered ? BorderHoverColor : BorderColor;

            // Draw button background with a slight 3D effect
            if (isPressed)
            {
                // Pressed state - inset effect
                framebuffer.FillRect(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, backgroundColor);
                framebuffer.DrawLine(Bounds.X, Bounds.Y + Bounds.Height - 1, Bounds.X + Bounds.Width - 1, Bounds.Y + Bounds.Height - 1, new Color(220, 220, 220));
                framebuffer.DrawLine(Bounds.X + Bounds.Width - 1, Bounds.Y, Bounds.X + Bounds.Width - 1, Bounds.Y + Bounds.Height - 1, new Color(220, 220, 220));
                framebuffer.DrawLine(Bounds.X, Bounds.Y, Bounds.X + Bounds.Width - 1, Bounds.Y, new Color(100, 100, 100));
                framebuffer.DrawLine(Bounds.X, Bounds.Y, Bounds.X, Bounds.Y + Bounds.Height - 1, new Color(100, 100, 100));
            }
            else
            {
                // Normal/Hover state - raised effect
                framebuffer.FillRect(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, backgroundColor);
                framebuffer.DrawLine(Bounds.X, Bounds.Y + Bounds.Height - 1, Bounds.X + Bounds.Width - 1, Bounds.Y + Bounds.Height - 1, new Color(100, 100, 100));
                framebuffer.DrawLine(Bounds.X + Bounds.Width - 1, Bounds.Y, Bounds.X + Bounds.Width - 1, Bounds.Y + Bounds.Height - 1, new Color(100, 100, 100));
                framebuffer.DrawLine(Bounds.X, Bounds.Y, Bounds.X + Bounds.Width - 1, Bounds.Y, new Color(220, 220, 220));
                framebuffer.DrawLine(Bounds.X, Bounds.Y, Bounds.X, Bounds.Y + Bounds.Height - 1, new Color(220, 220, 220));
            }

            // Draw border
            framebuffer.DrawRect(
                Bounds.X,
                Bounds.Y,
                Bounds.Width,
                Bounds.Height,
                borderColor
            );

            // Draw text centered
            if (!string.IsNullOrEmpty(Text))
            {
                int textWidth = FontRenderer.MeasureText(Text);
                double textX = Bounds.X + (Bounds.Width - textWidth) / 2;
                double textY = Bounds.Y + (Bounds.Height - 7) / 2; // 7 is the font height

                // If pressed, offset text slightly to enhance button press effect
                if (isPressed)
                {
                    textX += 1;
                    textY += 1;
                }

                FontRenderer.DrawText(
                    framebuffer,
                    Text,
                    (int)textX,
                    (int)textY,
                    IsEnabled ? TextColor : new Color(120, 120, 120)
                );
            }
        }

        protected override bool OnMouseEvent(int x, int y, bool isDown)
        {
            if (!IsEnabled) return false;

            var point = new Point { X = x, Y = y };
            var isInBounds = Bounds.Contains(point);

            // Handle mouse enter/leave
            if (isInBounds && !isHovered)
            {
                isHovered = true;
                OnMouseEnter?.Invoke();
            }
            else if (!isInBounds && isHovered)
            {
                isHovered = false;
                isPressed = false;
                OnMouseLeave?.Invoke();
            }

            // Handle click
            if (isInBounds)
            {
                if (isDown)
                {
                    isPressed = true;
                    wasPressed = true;
                }
                else if (wasPressed)
                {
                    if (isPressed)
                    {
                        OnClick?.Invoke();
                    }
                    isPressed = false;
                    wasPressed = false;
                }
            }
            else
            {
                isPressed = false;
                wasPressed = false;
            }

            return isInBounds;
        }

        void IDisposable.Dispose()
        {

        }

        void IButton.Draw(Framebuffer framebuffer)
        {
            OnDraw(framebuffer);
        }

        void IButton.Initialize(double x, double y, double width, double height, string title)
        {

        }
    }
}