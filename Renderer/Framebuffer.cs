using System.Runtime.InteropServices;

namespace ZephyrRenderer
{
    public class Framebuffer : IDisposable
    {
        private readonly int width;
        private readonly int height;
        private readonly byte[] buffer;
        private readonly GCHandle bufferHandle;
        private bool disposed;

        public int Width => width;
        public int Height => height;
        public IntPtr BufferPtr => bufferHandle.AddrOfPinnedObject();
        public int Stride => width * 4;
        public int BufferSize => width * height * 4;

        public Framebuffer(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Width and height must be positive numbers");

            this.width = width;
            this.height = height;
            this.buffer = new byte[width * height * 4];
            this.bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        }

        public void Clear(Color color)
        {
            for (int i = 0; i < buffer.Length; i += 4)
            {
                buffer[i] = color.B;     // Blue
                buffer[i + 1] = color.G; // Green
                buffer[i + 2] = color.R; // Red
                buffer[i + 3] = color.A; // Alpha
            }
        }

        public void SetPixel(int x, int y, Color color)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return;

            int index = (y * width + x) * 4;
            buffer[index] = color.B;     // Blue
            buffer[index + 1] = color.G; // Green
            buffer[index + 2] = color.R; // Red
            buffer[index + 3] = color.A; // Alpha
        }

        public Color GetPixel(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return Color.Transparent;

            int index = (y * width + x) * 4;
            return new Color(
                buffer[index + 2], // Red
                buffer[index + 1], // Green
                buffer[index],     // Blue
                buffer[index + 3]  // Alpha
            );
        }

        public void DrawLine(int x1, int y1, int x2, int y2, Color color)
        {
            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = x1 < x2 ? 1 : -1;
            int sy = y1 < y2 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                SetPixel(x1, y1, color);
                if (x1 == x2 && y1 == y2) break;
                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x1 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y1 += sy;
                }
            }
        }

        public void DrawRect(int x, int y, int width, int height, Color color)
        {
            DrawLine(x, y, x + width, y, color);
            DrawLine(x + width, y, x + width, y + height, color);
            DrawLine(x + width, y + height, x, y + height, color);
            DrawLine(x, y + height, x, y, color);
        }

        public void FillRect(int x, int y, int width, int height, Color color)
        {
            for (int py = y; py < y + height; py++)
            {
                for (int px = x; px < x + width; px++)
                {
                    SetPixel(px, py, color);
                }
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                if (bufferHandle.IsAllocated)
                    bufferHandle.Free();
                disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        ~Framebuffer()
        {
            Dispose();
        }
    }
}