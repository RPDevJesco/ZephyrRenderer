using ZephyrRenderer;
using ZephyrRenderer.UIElement;

class Program
{
    static void Main()
    {
        // Create our main window
        var window = new Window("Zephyr UI Demo", 800, 600);

        // Create a main panel that will contain our UI elements
        var mainPanel = new Panel
        {
            Bounds = new Rectangle { X = 20, Y = 20, Width = 760, Height = 560 },
            BackgroundColor = new Color(30, 30, 40)
        };
        window.Children.Add(mainPanel);

        // Add a bouncing ball panel
        var animationPanel = new AnimatedPanel
        {
            Bounds = new Rectangle { X = 40, Y = 40, Width = 400, Height = 300 },
            BackgroundColor = new Color(20, 20, 30)
        };
        mainPanel.Children.Add(animationPanel);

        // Add some control buttons
        var buttonPanel = new Panel
        {
            Bounds = new Rectangle { X = 460, Y = 40, Width = 280, Height = 300 },
            BackgroundColor = new Color(40, 40, 50)
        };
        mainPanel.Children.Add(buttonPanel);

        // Add buttons to control the animation
        var resetButton = new Button
        {
            Bounds = new Rectangle { X = 480, Y = 60, Width = 240, Height = 40 },
            Text = "RESET ANIMATION",
            TextColor = new Color(240, 240, 240),
            BackgroundColor = new Color(60, 60, 70),
            HoverColor = new Color(70, 70, 80),
            PressedColor = new Color(50, 50, 60),
            BorderColor = new Color(80, 80, 90),
            BorderHoverColor = new Color(100, 100, 110)
        };
        resetButton.OnClick += () => animationPanel.ResetAnimation();
        resetButton.OnMouseEnter += () => Console.WriteLine("Reset button hovered");
        resetButton.OnMouseLeave += () => Console.WriteLine("Reset button unhovered");
        buttonPanel.Children.Add(resetButton);

        var colorButton = new Button
        {
            Bounds = new Rectangle { X = 480, Y = 110, Width = 240, Height = 40 },
            Text = "CHANGE COLORS",
            TextColor = new Color(240, 240, 240),
            BackgroundColor = new Color(60, 60, 70),
            HoverColor = new Color(70, 70, 80),
            PressedColor = new Color(50, 50, 60),
            BorderColor = new Color(80, 80, 90),
            BorderHoverColor = new Color(100, 100, 110)
        };
        colorButton.OnClick += () => animationPanel.RandomizeColors();
        colorButton.OnMouseEnter += () => Console.WriteLine("Color button hovered");
        colorButton.OnMouseLeave += () => Console.WriteLine("Color button unhovered");
        buttonPanel.Children.Add(colorButton);

        // Add a speed control button
        var speedButton = new Button
        {
            Bounds = new Rectangle { X = 480, Y = 160, Width = 240, Height = 40 },
            Text = "TOGGLE SPEED",
            TextColor = new Color(240, 240, 240),
            BackgroundColor = new Color(60, 60, 70),
            HoverColor = new Color(70, 70, 80),
            PressedColor = new Color(50, 50, 60),
            BorderColor = new Color(80, 80, 90),
            BorderHoverColor = new Color(100, 100, 110)
        };
        speedButton.OnClick += () => animationPanel.ToggleSpeed();
        buttonPanel.Children.Add(speedButton);

        // Run the window
        window.Run();
    }
}

// Custom panel that includes our bouncing ball animation
class AnimatedPanel : Panel
{
    private int xPos = 0;
    private int yPos = 150;
    private int xDirection = 1;
    private Color ballColor = new Color(255, 165, 0);
    private Color lineColor = new Color(0, 255, 0);
    private int speed = 2;

    protected override void OnDraw(Framebuffer framebuffer)
    {
        // Draw panel background
        base.OnDraw(framebuffer);

        // Draw border
        framebuffer.DrawRect(
            Bounds.X, 
            Bounds.Y, 
            Bounds.Width, 
            Bounds.Height, 
            new Color(100, 100, 100)
        );

        // Draw cross pattern
        framebuffer.DrawLine(
            Bounds.X, 
            Bounds.Y, 
            Bounds.X + Bounds.Width, 
            Bounds.Y + Bounds.Height, 
            lineColor
        );
        framebuffer.DrawLine(
            Bounds.X + Bounds.Width, 
            Bounds.Y, 
            Bounds.X, 
            Bounds.Y + Bounds.Height, 
            lineColor
        );

        // Draw bouncing ball
        double ballX = Bounds.X + xPos;
        double ballY = Bounds.Y + yPos;
        framebuffer.FillRect(ballX, ballY - 20, 40, 40, ballColor);

        // Update ball position
        xPos += speed * xDirection;
        if (xPos >= Bounds.Width - 40 || xPos <= 0)
        {
            xDirection *= -1;
        }
    }

    public void ResetAnimation()
    {
        xPos = 0;
        yPos = 150;
        xDirection = 1;
        speed = 2;
    }

    public void RandomizeColors()
    {
        ballColor = new Color(
            (byte)Random.Shared.Next(128, 255),
            (byte)Random.Shared.Next(128, 255),
            (byte)Random.Shared.Next(128, 255)
        );

        lineColor = new Color(
            (byte)Random.Shared.Next(128, 255),
            (byte)Random.Shared.Next(128, 255),
            (byte)Random.Shared.Next(128, 255)
        );
    }

    public void ToggleSpeed()
    {
        speed = speed == 2 ? 4 : 2;
    }
}