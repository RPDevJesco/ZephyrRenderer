using System.Runtime.InteropServices;

namespace ZephyrRenderer
{
    public class Framebuffer : IDisposable
    {
        private readonly double width;
        private readonly double height;
        private readonly byte[] buffer;
        private readonly GCHandle bufferHandle;
        private bool disposed;

        public double Width => width;
        public double Height => height;
        public IntPtr BufferPtr => bufferHandle.AddrOfPinnedObject();
        public double Stride => width * 4;
        public double BufferSize => width * height * 4;

        public Framebuffer(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Width and height must be positive numbers");

            this.width = width;
            this.height = height;
            this.buffer = new byte[width * height * 4];
            this.bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        }
        
        public byte[] GetBuffer()
        {
            return buffer;
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

        public void SetPixel(double x, double y, Color color)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return;

            double index = (y * width + x) * 4;
            buffer[(int)index] = color.B;     // Blue
            buffer[(int)(index + 1)] = color.G; // Green
            buffer[(int)(index + 2)] = color.R; // Red
            buffer[(int)(index + 3)] = color.A; // Alpha
        }

        public Color GetPixel(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return Color.Transparent;

            double index = (y * width + x) * 4;
            return new Color(
                buffer[(int)(index + 2)], // Red
                buffer[(int)(index + 1)], // Green
                buffer[(int)index],     // Blue
                buffer[(int)(index + 3)]  // Alpha
            );
        }

        public void DrawLine(double x1, double y1, double x2, double y2, Color color)
        {
            double dx = Math.Abs(x2 - x1);
            double dy = Math.Abs(y2 - y1);
            double sx = x1 < x2 ? 1 : -1;
            double sy = y1 < y2 ? 1 : -1;
            double err = dx - dy;

            while (true)
            {
                SetPixel(x1, y1, color);
                if (x1 == x2 && y1 == y2) break;
                double e2 = 2 * err;
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

        public void DrawRect(double x, double y, double width, double height, Color color)
        {
            DrawLine(x, y, x + width, y, color);
            DrawLine(x + width, y, x + width, y + height, color);
            DrawLine(x + width, y + height, x, y + height, color);
            DrawLine(x, y + height, x, y, color);
        }

        public void FillRect(double x, double y, double width, double height, Color color)
        {
            for (double py = y; py < y + height; py++)
            {
                for (double px = x; px < x + width; px++)
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