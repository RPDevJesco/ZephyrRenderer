using System.Runtime.InteropServices;
using ZephyrRenderer.Platform;

namespace ZephyrRenderer.Renderer
{
    public class Bitmap : IDisposable
    {
        private IntPtr _imageHandle;

        public void Load(string filePath)
        {
            Console.WriteLine($"Loading image from file: {filePath}");
            if (!System.IO.File.Exists(filePath))
            {
                Console.WriteLine("File not found.");
                throw new System.IO.FileNotFoundException("The specified file was not found.", filePath);
            }

            IntPtr nsStringClass = Cocoa.objc_getClass("NSString");
            IntPtr stringWithUTF8StringSelector = Cocoa.sel_registerName("stringWithUTF8String:");
            IntPtr filePathHandle = Cocoa.objc_msgSend(nsStringClass, stringWithUTF8StringSelector, filePath);

            if (filePathHandle == IntPtr.Zero)
            {
                Console.WriteLine("Failed to create NSString from file path.");
                throw new Exception("Failed to create NSString from file path.");
            }

            IntPtr nsImageClass = Cocoa.objc_getClass("NSImage");
            IntPtr allocSelector = Cocoa.sel_registerName("alloc");
            IntPtr initWithContentsOfFileSelector = Cocoa.sel_registerName("initWithContentsOfFile:");

            IntPtr imageAllocHandle = Cocoa.objc_msgSend(nsImageClass, allocSelector);
            if (imageAllocHandle == IntPtr.Zero)
            {
                Console.WriteLine("Failed to allocate NSImage.");
                throw new Exception("Failed to allocate NSImage.");
            }

            _imageHandle = Cocoa.objc_msgSend(imageAllocHandle, initWithContentsOfFileSelector, filePathHandle);
            if (_imageHandle == IntPtr.Zero)
            {
                Console.WriteLine("Failed to initialize NSImage with file path.");
                throw new Exception("Failed to initialize NSImage with file path.");
            }

            Console.WriteLine($"Bitmap loaded from {filePath}");
        }

        public void Draw(IntPtr context, float x, float y)
        {
            Console.WriteLine("Starting draw process...");
            if (_imageHandle == IntPtr.Zero)
            {
                Console.WriteLine("Bitmap has not been loaded.");
                throw new InvalidOperationException("Bitmap has not been loaded.");
            }

            if (context == IntPtr.Zero)
            {
                Console.WriteLine("Invalid graphics context.");
                throw new ArgumentException("Invalid graphics context.");
            }

            Size size = Cocoa.NSImage_size(_imageHandle);
            Console.WriteLine($"Bitmap size: {size.Width}x{size.Height}");

            var rect = new Rectangle(x, y, size.Width, size.Height);
            IntPtr imageRef = Cocoa.NSImage_CGImageForProposedRect(_imageHandle, ref rect, IntPtr.Zero, IntPtr.Zero);

            if (imageRef == IntPtr.Zero)
            {
                Console.WriteLine("Failed to get CGImage from NSImage.");
                throw new Exception("Failed to get CGImage from NSImage.");
            }

            Console.WriteLine($"Drawing image at ({x}, {y}) with size {size.Width}x{size.Height}");
            Cocoa.CGContextDrawImage(context, rect, imageRef);
            Console.WriteLine($"Bitmap drawn at ({x}, {y})");
        }

        public void Dispose()
        {
            Console.WriteLine("Disposing Bitmap...");
            if (_imageHandle != IntPtr.Zero)
            {
                Marshal.Release(_imageHandle);
                _imageHandle = IntPtr.Zero;
            }
            Console.WriteLine("Bitmap disposed.");
        }
    }
}
