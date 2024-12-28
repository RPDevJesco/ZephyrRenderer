using System.Runtime.InteropServices;
using static ZephyrRenderer.Platform.Win32;

namespace ZephyrRenderer.Platform
{
    public class WindowsRenderer : IRenderer
    {
        private IntPtr hwnd;
        private IntPtr hdc;
        private IntPtr memDC;
        private IntPtr hBitmap;
        private bool disposed;
        private readonly WndProcDelegate wndProc;
        private bool shouldClose;

        public bool ShouldClose => shouldClose;

        // Mouse event handlers
        public event Action<int, int>? OnMouseMove;
        public event Action<int, int, bool>? OnMouseButton;

        private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        public WindowsRenderer()
        {
            wndProc = WndProc;
        }

        public void Initialize(string title, int width, int height)
        {
            RegisterWindowClass();
            CreateWindowHandle(title, width, height);
            InitializeGraphics(width, height);
        }

        private void RegisterWindowClass()
        {
            var wndClass = new WNDCLASSEX
            {
                cbSize = Marshal.SizeOf<WNDCLASSEX>(),
                style = CS_HREDRAW | CS_VREDRAW,
                lpfnWndProc = Marshal.GetFunctionPointerForDelegate(wndProc),
                hInstance = GetModuleHandle(null),
                hCursor = LoadCursor(IntPtr.Zero, IDC_ARROW),
                lpszClassName = "CustomRendererWindow"
            };

            RegisterClassEx(ref wndClass);
        }

        private void CreateWindowHandle(string title, int width, int height)
        {
            hwnd = CreateWindowEx(
                0,
                "CustomRendererWindow",
                title,
                WS_OVERLAPPEDWINDOW,
                CW_USEDEFAULT, CW_USEDEFAULT,
                width, height,
                IntPtr.Zero,
                IntPtr.Zero,
                GetModuleHandle(null),
                IntPtr.Zero
            );

            if (hwnd == IntPtr.Zero)
                throw new Exception("Failed to create window");

            ShowWindow(hwnd, SW_SHOW);
            UpdateWindow(hwnd);
        }

        private void InitializeGraphics(int width, int height)
        {
            hdc = GetDC(hwnd);
            memDC = CreateCompatibleDC(hdc);
            hBitmap = CreateCompatibleBitmap(hdc, width, height);
            SelectObject(memDC, hBitmap);
        }

        public void Present(Framebuffer framebuffer)
        {
            var bmi = new BITMAPINFO
            {
                bmiHeader = new BITMAPINFOHEADER
                {
                    biSize = (uint)Marshal.SizeOf<BITMAPINFOHEADER>(),
                    biWidth = framebuffer.Width,
                    biHeight = -framebuffer.Height,
                    biPlanes = 1,
                    biBitCount = 32,
                    biCompression = BI_RGB
                }
            };

            SetDIBitsToDevice(
                memDC,
                0, 0,
                (uint)framebuffer.Width, (uint)framebuffer.Height,
                0, 0,
                0, (uint)framebuffer.Height,
                framebuffer.BufferPtr,
                ref bmi,
                DIB_RGB_COLORS
            );

            BitBlt(
                hdc,
                0, 0,
                framebuffer.Width, framebuffer.Height,
                memDC,
                0, 0,
                SRCCOPY
            );
        }

        public void ProcessEvents()
        {
            while (PeekMessage(out MSG msg, IntPtr.Zero, 0, 0, PM_REMOVE))
            {
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }
        }

        private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case WM_MOUSEMOVE:
                    {
                        int x = (short)(lParam.ToInt32() & 0xFFFF);
                        int y = (short)((lParam.ToInt32() >> 16) & 0xFFFF);
                        OnMouseMove?.Invoke(x, y);
                        return IntPtr.Zero;
                    }

                case WM_LBUTTONDOWN:
                    {
                        int x = (short)(lParam.ToInt32() & 0xFFFF);
                        int y = (short)((lParam.ToInt32() >> 16) & 0xFFFF);
                        OnMouseButton?.Invoke(x, y, true);
                        return IntPtr.Zero;
                    }

                case WM_LBUTTONUP:
                    {
                        int x = (short)(lParam.ToInt32() & 0xFFFF);
                        int y = (short)((lParam.ToInt32() >> 16) & 0xFFFF);
                        OnMouseButton?.Invoke(x, y, false);
                        return IntPtr.Zero;
                    }

                case WM_CLOSE:
                    shouldClose = true;
                    DestroyWindow(hWnd);
                    return IntPtr.Zero;

                case WM_DESTROY:
                    PostQuitMessage(0);
                    return IntPtr.Zero;

                default:
                    return DefWindowProc(hWnd, msg, wParam, lParam);
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                if (hBitmap != IntPtr.Zero)
                    DeleteObject(hBitmap);
                if (memDC != IntPtr.Zero)
                    DeleteDC(memDC);
                if (hdc != IntPtr.Zero)
                    ReleaseDC(hwnd, hdc);
                disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}