namespace ZephyrRenderer.Renderer
{
    public class BitmapRenderer : IDisposable
    {
        private IntPtr _windowHandle;
        private Bitmap _bitmap;

        public BitmapRenderer(IntPtr windowHandle)
        {
            _windowHandle = windowHandle;
            Console.WriteLine($"BitmapRenderer created with window handle: {_windowHandle}");
        }

        public void LoadBitmap(string filePath)
        {
            Console.WriteLine($"Loading bitmap from file: {filePath}");
            try
            {
                _bitmap = new Bitmap();
                _bitmap.Load(filePath);
                Console.WriteLine("Bitmap loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while loading the bitmap: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public void Render(IntPtr context, float x, float y)
        {
            Console.WriteLine("Starting render process...");
            if (_bitmap == null)
            {
                Console.WriteLine("Bitmap has not been loaded.");
                throw new InvalidOperationException("Bitmap has not been loaded.");
            }

            if (context == IntPtr.Zero)
            {
                Console.WriteLine("Invalid graphics context.");
                throw new ArgumentException("Invalid graphics context.");
            }

            Console.WriteLine($"Rendering bitmap at position ({x}, {y}) with context: {context}");
            try
            {
                _bitmap.Draw(context, x, y);
                Console.WriteLine("Bitmap rendered successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during rendering: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public void Dispose()
        {
            Console.WriteLine("Disposing BitmapRenderer...");
            try
            {
                _bitmap?.Dispose();
                Console.WriteLine("BitmapRenderer disposed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while disposing the BitmapRenderer: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
    }
}
