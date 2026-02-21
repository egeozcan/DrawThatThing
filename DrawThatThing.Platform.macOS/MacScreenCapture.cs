using System.Runtime.InteropServices;

namespace DrawThatThing.Platform.macOS;

public class MacScreenCapture : IScreenCapture
{
    private const string CoreGraphics = "/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics";

    [DllImport(CoreGraphics)]
    private static extern IntPtr CGWindowListCreateImage(CGRect screenBounds, CGWindowListOption listOption, uint windowID, CGWindowImageOption imageOption);

    [DllImport(CoreGraphics)]
    private static extern IntPtr CGImageGetDataProvider(IntPtr image);

    [DllImport(CoreGraphics)]
    private static extern IntPtr CGDataProviderCopyData(IntPtr provider);

    [DllImport(CoreGraphics)]
    private static extern IntPtr CFDataGetBytePtr(IntPtr theData);

    [DllImport(CoreGraphics)]
    private static extern long CFDataGetLength(IntPtr theData);

    [DllImport(CoreGraphics)]
    private static extern void CFRelease(IntPtr cf);

    [DllImport(CoreGraphics)]
    private static extern int CGImageGetWidth(IntPtr image);

    [DllImport(CoreGraphics)]
    private static extern int CGImageGetHeight(IntPtr image);

    [DllImport(CoreGraphics)]
    private static extern int CGImageGetBytesPerRow(IntPtr image);

    [StructLayout(LayoutKind.Sequential)]
    private struct CGRect
    {
        public CGPoint origin;
        public CGSize size;

        public CGRect(double x, double y, double width, double height)
        {
            origin = new CGPoint(x, y);
            size = new CGSize(width, height);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct CGPoint
    {
        public double X;
        public double Y;

        public CGPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct CGSize
    {
        public double Width;
        public double Height;

        public CGSize(double width, double height)
        {
            Width = width;
            Height = height;
        }
    }

    private enum CGWindowListOption : uint
    {
        OptionAll = 0,
        OptionOnScreenOnly = 1,
        OptionOnScreenAboveWindow = 2,
        OptionOnScreenBelowWindow = 4,
        OptionIncludingWindow = 8
    }

    private enum CGWindowImageOption : uint
    {
        Default = 0,
        BoundsIgnoreFraming = 1,
        ShouldBeOpaque = 2,
        OnlyShadows = 4,
        BestResolution = 8,
        NominalResolution = 16
    }

    public (byte R, byte G, byte B) GetPixelColor(int x, int y)
    {
        var pixels = CaptureRegion(x, y, 1, 1);
        if (pixels.Length >= 4)
        {
            return (pixels[0], pixels[1], pixels[2]);
        }
        return (0, 0, 0);
    }

    public byte[] CaptureRegion(int x, int y, int width, int height)
    {
        var rect = new CGRect(x, y, width, height);
        var imageRef = CGWindowListCreateImage(rect, CGWindowListOption.OptionOnScreenOnly, 0, CGWindowImageOption.Default);

        if (imageRef == IntPtr.Zero)
        {
            return new byte[width * height * 4];
        }

        IntPtr data = IntPtr.Zero;
        try
        {
            var dataProvider = CGImageGetDataProvider(imageRef);
            if (dataProvider == IntPtr.Zero)
            {
                return new byte[width * height * 4];
            }

            data = CGDataProviderCopyData(dataProvider);
            if (data == IntPtr.Zero)
            {
                return new byte[width * height * 4];
            }

            var dataPtr = CFDataGetBytePtr(data);
            var dataLength = CFDataGetLength(data);
            if (dataPtr == IntPtr.Zero || dataLength <= 0)
            {
                return new byte[width * height * 4];
            }

            var capturedWidth = CGImageGetWidth(imageRef);
            var capturedHeight = CGImageGetHeight(imageRef);
            var bytesPerRow = CGImageGetBytesPerRow(imageRef);
            if (capturedWidth <= 0 || capturedHeight <= 0 || bytesPerRow <= 0)
            {
                return new byte[width * height * 4];
            }

            byte[] pixels = new byte[width * height * 4];

            if (dataPtr != IntPtr.Zero && dataLength > 0)
            {
                byte[] rawData = new byte[dataLength];
                Marshal.Copy(dataPtr, rawData, 0, (int)dataLength);

                // Copy and convert data (macOS uses BGRA)
                for (int row = 0; row < Math.Min(height, capturedHeight); row++)
                {
                    for (int col = 0; col < Math.Min(width, capturedWidth); col++)
                    {
                        int srcIndex = row * bytesPerRow + col * 4;
                        int dstIndex = (row * width + col) * 4;

                        if (srcIndex + 3 < rawData.Length && dstIndex + 3 < pixels.Length)
                        {
                            // Convert BGRA to RGBA
                            pixels[dstIndex] = rawData[srcIndex + 2];     // R
                            pixels[dstIndex + 1] = rawData[srcIndex + 1]; // G
                            pixels[dstIndex + 2] = rawData[srcIndex];     // B
                            pixels[dstIndex + 3] = rawData[srcIndex + 3]; // A
                        }
                    }
                }
            }

            return pixels;
        }
        finally
        {
            if (data != IntPtr.Zero)
            {
                CFRelease(data);
            }
            CFRelease(imageRef);
        }
    }
}
