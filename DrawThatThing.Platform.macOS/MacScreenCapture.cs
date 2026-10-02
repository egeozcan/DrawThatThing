using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DrawThatThing.Platform.macOS;

/// <summary>
/// Reads pixels from the screen. Needs the Screen Recording permission
/// (System Settings → Privacy &amp; Security → Screen &amp; System Audio Recording); without it
/// macOS only returns the desktop wallpaper instead of other applications' windows.
/// Colors are returned in sRGB, the color space of the images being drawn, rather than in the
/// display's own color space (which differs noticeably on wide-gamut screens).
/// </summary>
public class MacScreenCapture : IScreenCapture
{
    private const string CoreGraphics = "/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics";
    private const string CoreFoundation = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";
    private const string ImageIO = "/System/Library/Frameworks/ImageIO.framework/ImageIO";

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr CGWindowListCreateImageProc(CGRect screenBounds, uint listOption, uint windowId, uint imageOption);

    [DllImport(CoreGraphics)]
    private static extern void CGImageRelease(IntPtr image);

    [DllImport(CoreGraphics)]
    private static extern IntPtr CGColorSpaceCreateWithName(IntPtr name);

    [DllImport(CoreGraphics)]
    private static extern void CGColorSpaceRelease(IntPtr space);

    [DllImport(CoreGraphics)]
    private static extern IntPtr CGBitmapContextCreate(IntPtr data, nuint width, nuint height, nuint bitsPerComponent, nuint bytesPerRow, IntPtr space, uint bitmapInfo);

    [DllImport(CoreGraphics)]
    private static extern void CGContextSetInterpolationQuality(IntPtr context, int quality);

    [DllImport(CoreGraphics)]
    private static extern void CGContextDrawImage(IntPtr context, CGRect rect, IntPtr image);

    [DllImport(CoreGraphics)]
    private static extern void CGContextRelease(IntPtr context);

    [DllImport(CoreGraphics)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool CGPreflightScreenCaptureAccess();

    [DllImport(CoreGraphics)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool CGRequestScreenCaptureAccess();

    [DllImport(CoreFoundation)]
    private static extern IntPtr CFDataCreate(IntPtr allocator, byte[] bytes, nint length);

    [DllImport(CoreFoundation)]
    private static extern void CFRelease(IntPtr cf);

    [DllImport(ImageIO)]
    private static extern IntPtr CGImageSourceCreateWithData(IntPtr data, IntPtr options);

    [DllImport(ImageIO)]
    private static extern IntPtr CGImageSourceCreateImageAtIndex(IntPtr source, nuint index, IntPtr options);

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

    private const uint KCGImageAlphaNoneSkipLast = 5;
    private const uint KCGBitmapByteOrder32Big = 0x4000;
    private const int KCGInterpolationNone = 1;

    private static readonly Lazy<CGWindowListCreateImageProc?> WindowListCreateImage = new(LoadWindowListCreateImage);
    private static readonly Lazy<IntPtr> SrgbColorSpaceName = new(
        () => Marshal.ReadIntPtr(NativeLibrary.GetExport(NativeLibrary.Load(CoreGraphics), "kCGColorSpaceSRGB")));
    private readonly Func<bool> _hasAccess;
    private bool _accessRequested;

    public MacScreenCapture()
    {
        _hasAccess = EnsureAccess;
    }

    /// <param name="hasAccess">Replaces the permission check, so tests never trigger the system prompt.</param>
    internal MacScreenCapture(Func<bool> hasAccess)
    {
        _hasAccess = hasAccess;
    }

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

    /// <summary>
    /// Returns null without the Screen Recording permission: the screen would only show the wallpaper,
    /// so any color read from it would be made up.
    /// </summary>
    public (byte R, byte G, byte B)? GetPixelColor(int x, int y)
    {
        if (!_hasAccess())
        {
            return null;
        }

        var pixels = Capture(x, y, 1, 1);
        return pixels == null ? null : (pixels[0], pixels[1], pixels[2]);
    }

    public byte[] CaptureRegion(int x, int y, int width, int height)
    {
        return Capture(x, y, width, height) ?? new byte[width * height * 4];
    }

    private static byte[]? Capture(int x, int y, int width, int height)
    {
        return CaptureWithCoreGraphics(x, y, width, height)
               ?? CaptureWithScreencaptureTool(x, y, width, height);
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

        try
        {
            return ReadSrgbPixels(imageRef, width, height);
        }
        finally
        {
            CGImageRelease(imageRef);
        }
    }

    /// <summary>
    /// Returns the image's pixels as sRGB RGBA bytes, scaled to the given size. CoreGraphics does the color
    /// matching from the image's color space and handles any pixel layout. Scaling picks single pixels
    /// instead of blending, because on Retina screens a one-point region covers several pixels and a blend
    /// at the edge of a palette swatch would be a color that does not exist.
    /// </summary>
    internal static byte[]? ReadSrgbPixels(IntPtr image, int width, int height)
    {
        var pixels = new byte[width * height * 4];
        var colorSpace = CGColorSpaceCreateWithName(SrgbColorSpaceName.Value);
        if (colorSpace == IntPtr.Zero)
        {
            return null;
        }

        var handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
        try
        {
            var context = CGBitmapContextCreate(
                handle.AddrOfPinnedObject(), (nuint)width, (nuint)height, 8, (nuint)(width * 4), colorSpace,
                KCGImageAlphaNoneSkipLast | KCGBitmapByteOrder32Big);
            if (context == IntPtr.Zero)
            {
                return null;
            }

            try
            {
                CGContextSetInterpolationQuality(context, KCGInterpolationNone);
                CGContextDrawImage(context, new CGRect { Width = width, Height = height }, image);
            }
            finally
            {
                CGContextRelease(context);
            }
        }
        finally
        {
            handle.Free();
            CGColorSpaceRelease(colorSpace);
        }

        for (int i = 3; i < pixels.Length; i += 4)
        {
            pixels[i] = 255;
        }
        return pixels;
    }

    /// <summary>Decodes an image file (keeping its embedded color profile). The caller releases the image.</summary>
    internal static IntPtr CreateImageFromFileData(byte[] fileData)
    {
        var data = CFDataCreate(IntPtr.Zero, fileData, fileData.Length);
        if (data == IntPtr.Zero)
        {
            return IntPtr.Zero;
        }

        try
        {
            var source = CGImageSourceCreateWithData(data, IntPtr.Zero);
            if (source == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            try
            {
                return CGImageSourceCreateImageAtIndex(source, 0, IntPtr.Zero);
            }
            finally
            {
                CFRelease(source);
            }
        }
        finally
        {
            CFRelease(data);
        }
    }

    /// <summary>
    /// Fallback for macOS versions without CGWindowListCreateImage: the built-in screencapture tool.
    /// Its PNG files carry the display's color profile, so they go through the same color matching.
    /// </summary>
    private static byte[]? CaptureWithScreencaptureTool(int x, int y, int width, int height)
    {
        var file = Path.Combine(Path.GetTempPath(), $"drawthatthing-{Guid.NewGuid():N}.png");
        try
        {
            var startInfo = new ProcessStartInfo("/usr/sbin/screencapture")
            {
                UseShellExecute = false,
                CreateNoWindow = true
            };
            foreach (var argument in new[] { "-x", "-t", "png", $"-R{x},{y},{width},{height}", file })
            {
                startInfo.ArgumentList.Add(argument);
            }

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                return null;
            }
            if (!process.WaitForExit(5000))
            {
                // Otherwise it may still write the file after it has been cleaned up below.
                process.Kill();
                return null;
            }
            if (process.ExitCode != 0 || !File.Exists(file))
            {
                return null;
            }

            var image = CreateImageFromFileData(File.ReadAllBytes(file));
            if (image == IntPtr.Zero)
            {
                return null;
            }

            try
            {
                return ReadSrgbPixels(image, width, height);
            }
            finally
            {
                CGImageRelease(image);
            }
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
}
