using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DrawThatThing.Platform.macOS;

/// <summary>
/// Reads pixels from the screen. Needs the Screen Recording permission
/// (System Settings → Privacy &amp; Security → Screen &amp; System Audio Recording); without it
/// macOS only returns the desktop wallpaper instead of other applications' windows.
/// </summary>
public class MacScreenCapture : IScreenCapture
{
    private const string CoreGraphics = "/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics";
    private const string CoreFoundation = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr CGWindowListCreateImageProc(CGRect screenBounds, uint listOption, uint windowId, uint imageOption);

    [DllImport(CoreGraphics)]
    private static extern IntPtr CGImageGetDataProvider(IntPtr image);

    [DllImport(CoreGraphics)]
    private static extern IntPtr CGDataProviderCopyData(IntPtr provider);

    [DllImport(CoreGraphics)]
    private static extern nint CGImageGetWidth(IntPtr image);

    [DllImport(CoreGraphics)]
    private static extern nint CGImageGetHeight(IntPtr image);

    [DllImport(CoreGraphics)]
    private static extern nint CGImageGetBytesPerRow(IntPtr image);

    [DllImport(CoreGraphics)]
    private static extern nint CGImageGetBitsPerPixel(IntPtr image);

    [DllImport(CoreGraphics)]
    private static extern uint CGImageGetBitmapInfo(IntPtr image);

    [DllImport(CoreGraphics)]
    private static extern void CGImageRelease(IntPtr image);

    [DllImport(CoreGraphics)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool CGPreflightScreenCaptureAccess();

    [DllImport(CoreGraphics)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool CGRequestScreenCaptureAccess();

    [DllImport(CoreFoundation)]
    private static extern IntPtr CFDataGetBytePtr(IntPtr theData);

    [DllImport(CoreFoundation)]
    private static extern nint CFDataGetLength(IntPtr theData);

    [DllImport(CoreFoundation)]
    private static extern void CFRelease(IntPtr cf);

    [StructLayout(LayoutKind.Sequential)]
    private struct CGRect
    {
        public double X;
        public double Y;
        public double Width;
        public double Height;
    }

    private const uint KCGWindowListOptionOnScreenOnly = 1;
    private const uint KCGNullWindowId = 0;
    private const uint KCGWindowImageDefault = 0;

    private const uint AlphaInfoMask = 0x1F;
    private const uint ByteOrderMask = 0x7000;
    private const uint ByteOrder32Little = 0x2000;

    private static readonly Lazy<CGWindowListCreateImageProc?> WindowListCreateImage = new(LoadWindowListCreateImage);
    private bool _accessRequested;

    /// <summary>
    /// Asks macOS for the Screen Recording permission if it has not been granted yet.
    /// The system shows its prompt only once; afterwards the user has to enable it in System Settings.
    /// </summary>
    public bool EnsureAccess()
    {
        try
        {
            if (CGPreflightScreenCaptureAccess())
            {
                return true;
            }
            if (!_accessRequested)
            {
                _accessRequested = true;
                return CGRequestScreenCaptureAccess();
            }
        }
        catch (Exception ex) when (ex is DllNotFoundException or EntryPointNotFoundException)
        {
            return true;
        }
        return false;
    }

    public (byte R, byte G, byte B) GetPixelColor(int x, int y)
    {
        EnsureAccess();
        var pixels = CaptureRegion(x, y, 1, 1);
        return (pixels[0], pixels[1], pixels[2]);
    }

    public byte[] CaptureRegion(int x, int y, int width, int height)
    {
        return CaptureWithCoreGraphics(x, y, width, height)
               ?? CaptureWithScreencaptureTool(x, y, width, height)
               ?? new byte[width * height * 4];
    }

    private static CGWindowListCreateImageProc? LoadWindowListCreateImage()
    {
        // CGWindowListCreateImage is deprecated and may disappear from future macOS versions,
        // so look it up dynamically instead of binding to it.
        try
        {
            var library = NativeLibrary.Load(CoreGraphics);
            return NativeLibrary.TryGetExport(library, "CGWindowListCreateImage", out var address)
                ? Marshal.GetDelegateForFunctionPointer<CGWindowListCreateImageProc>(address)
                : null;
        }
        catch
        {
            return null;
        }
    }

    private static byte[]? CaptureWithCoreGraphics(int x, int y, int width, int height)
    {
        var create = WindowListCreateImage.Value;
        if (create == null)
        {
            return null;
        }

        var rect = new CGRect { X = x, Y = y, Width = width, Height = height };
        var imageRef = create(rect, KCGWindowListOptionOnScreenOnly, KCGNullWindowId, KCGWindowImageDefault);
        if (imageRef == IntPtr.Zero)
        {
            return null;
        }

        IntPtr data = IntPtr.Zero;
        try
        {
            var capturedWidth = (int)CGImageGetWidth(imageRef);
            var capturedHeight = (int)CGImageGetHeight(imageRef);
            var bytesPerRow = (int)CGImageGetBytesPerRow(imageRef);
            if (capturedWidth <= 0 || capturedHeight <= 0 || bytesPerRow <= 0 || CGImageGetBitsPerPixel(imageRef) != 32)
            {
                return null;
            }

            var dataProvider = CGImageGetDataProvider(imageRef);
            if (dataProvider == IntPtr.Zero)
            {
                return null;
            }

            data = CGDataProviderCopyData(dataProvider);
            if (data == IntPtr.Zero)
            {
                return null;
            }

            var dataPtr = CFDataGetBytePtr(data);
            var dataLength = (int)CFDataGetLength(data);
            if (dataPtr == IntPtr.Zero || dataLength <= 0)
            {
                return null;
            }

            var rawData = new byte[dataLength];
            Marshal.Copy(dataPtr, rawData, 0, dataLength);

            // Work out where each channel lives in memory from the image's bitmap info.
            var bitmapInfo = CGImageGetBitmapInfo(imageRef);
            var alphaFirst = (bitmapInfo & AlphaInfoMask) is 2 or 4 or 6; // premultiplied first, first, none-skip-first
            var littleEndian = (bitmapInfo & ByteOrderMask) == ByteOrder32Little;
            var (rIndex, gIndex, bIndex) = (alphaFirst, littleEndian) switch
            {
                (true, true) => (2, 1, 0),   // BGRA
                (true, false) => (1, 2, 3),  // ARGB
                (false, true) => (3, 2, 1),  // ABGR
                (false, false) => (0, 1, 2)  // RGBA
            };

            // On Retina displays one point covers several pixels; sample the pixel grid proportionally.
            var pixels = new byte[width * height * 4];
            for (int row = 0; row < height; row++)
            {
                int srcRow = Math.Min(capturedHeight - 1, row * capturedHeight / height);
                for (int col = 0; col < width; col++)
                {
                    int srcCol = Math.Min(capturedWidth - 1, col * capturedWidth / width);
                    int srcIndex = srcRow * bytesPerRow + srcCol * 4;
                    int dstIndex = (row * width + col) * 4;
                    if (srcIndex + 3 >= rawData.Length)
                    {
                        continue;
                    }
                    pixels[dstIndex] = rawData[srcIndex + rIndex];
                    pixels[dstIndex + 1] = rawData[srcIndex + gIndex];
                    pixels[dstIndex + 2] = rawData[srcIndex + bIndex];
                    pixels[dstIndex + 3] = 255;
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
            CGImageRelease(imageRef);
        }
    }

    /// <summary>
    /// Fallback for macOS versions without CGWindowListCreateImage: the built-in screencapture tool.
    /// </summary>
    private static byte[]? CaptureWithScreencaptureTool(int x, int y, int width, int height)
    {
        var file = Path.Combine(Path.GetTempPath(), $"drawthatthing-{Guid.NewGuid():N}.bmp");
        try
        {
            var startInfo = new ProcessStartInfo("/usr/sbin/screencapture")
            {
                UseShellExecute = false,
                CreateNoWindow = true
            };
            foreach (var argument in new[] { "-x", "-t", "bmp", $"-R{x},{y},{width},{height}", file })
            {
                startInfo.ArgumentList.Add(argument);
            }

            using var process = Process.Start(startInfo);
            if (process == null || !process.WaitForExit(5000) || process.ExitCode != 0 || !File.Exists(file))
            {
                return null;
            }

            return ReadBmp(File.ReadAllBytes(file), width, height);
        }
        catch
        {
            return null;
        }
        finally
        {
            try { File.Delete(file); } catch { /* ignore */ }
        }
    }

    private static byte[]? ReadBmp(byte[] bmp, int width, int height)
    {
        if (bmp.Length < 54 || bmp[0] != 'B' || bmp[1] != 'M')
        {
            return null;
        }

        int dataOffset = BitConverter.ToInt32(bmp, 10);
        int bmpWidth = BitConverter.ToInt32(bmp, 18);
        int bmpHeight = BitConverter.ToInt32(bmp, 22);
        int bitsPerPixel = BitConverter.ToInt16(bmp, 28);
        if (bmpWidth <= 0 || bmpHeight == 0 || (bitsPerPixel != 24 && bitsPerPixel != 32))
        {
            return null;
        }

        bool bottomUp = bmpHeight > 0;
        bmpHeight = Math.Abs(bmpHeight);
        int bytesPerPixel = bitsPerPixel / 8;
        int stride = (bmpWidth * bytesPerPixel + 3) & ~3;

        var pixels = new byte[width * height * 4];
        for (int row = 0; row < height; row++)
        {
            int srcRow = Math.Min(bmpHeight - 1, row * bmpHeight / height);
            if (bottomUp)
            {
                srcRow = bmpHeight - 1 - srcRow;
            }
            for (int col = 0; col < width; col++)
            {
                int srcCol = Math.Min(bmpWidth - 1, col * bmpWidth / width);
                int srcIndex = dataOffset + srcRow * stride + srcCol * bytesPerPixel;
                int dstIndex = (row * width + col) * 4;
                if (srcIndex + 2 >= bmp.Length)
                {
                    continue;
                }
                pixels[dstIndex] = bmp[srcIndex + 2];
                pixels[dstIndex + 1] = bmp[srcIndex + 1];
                pixels[dstIndex + 2] = bmp[srcIndex];
                pixels[dstIndex + 3] = 255;
            }
        }

        return pixels;
    }
}
