using System.Runtime.InteropServices;
using ZephyrRenderer.Platform;
using ZephyrRenderer.UIElement;

namespace ZephyrRenderer.Mac.UI
{
    public class MacButton : IButton
    {
        private IntPtr _buttonHandle;
        private IntPtr _targetHandle;

        public string Text { get; set; } = string.Empty;
        public bool IsEnabled { get; set; } = true;
        string IButton.Text { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        bool IButton.IsEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IntPtr GetButtonHandle()
        {
            if (_buttonHandle == IntPtr.Zero)
                throw new InvalidOperationException("Button has not been created yet.");
            return _buttonHandle;
        }

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

        public void Initialize(double x, double y, double width, double height, string title)
        {
            Text = title;
            _buttonHandle = InitializeButton(x, y, width, height, title);
            _targetHandle = ButtonClickHandler.Create();
            NSButtonWrapper.SetTarget(_buttonHandle, _targetHandle, "buttonClicked:");
            ButtonClickHandler.ButtonClickedEvent += OnClick;
        }

        private void OnClick(object sender, EventArgs e)
        {
            if (IsEnabled)
            {
                Click?.Invoke(this, e);
            }
        }

        public void Dispose()
        {
            if (_targetHandle != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_targetHandle);
                _targetHandle = IntPtr.Zero;
            }
            ButtonClickHandler.ButtonClickedEvent -= OnClick;
        }

        private IntPtr InitializeButton(double x, double y, double width, double height, string title)
        {
            var button = NSButtonWrapper.Create(new Rectangle(x, y, width, height), title);
            Console.WriteLine($"NSButton created and titled '{title}' at ({x}, {y}, {width}, {height}).");
            return button;
        }

        void IButton.Draw(Framebuffer framebuffer)
        {
            
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }
    }
}
