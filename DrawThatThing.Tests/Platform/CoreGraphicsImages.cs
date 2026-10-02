using System.Runtime.InteropServices;

namespace DrawThatThing.Tests.Platform;

/// <summary>Creates CoreGraphics images with a known color space and memory layout.</summary>
internal static class CoreGraphicsImages
{
    private const string CoreGraphics = "/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics";
    private const string CoreFoundation = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

    public const uint PremultipliedFirstLittleEndian = 2 | 0x2000; // BGRA in memory, like screen captures
    public const uint NoneSkipLastBigEndian = 5 | 0x4000; // RGBX in memory

    [DllImport(CoreGraphics)]
    private static extern IntPtr CGColorSpaceCreateWithName(IntPtr name);

    [DllImport(CoreGraphics)]
    private static extern void CGColorSpaceRelease(IntPtr space);

    [DllImport(CoreGraphics)]
    private static extern IntPtr CGDataProviderCreateWithCFData(IntPtr data);

    [DllImport(CoreGraphics)]
    private static extern void CGDataProviderRelease(IntPtr provider);

    [DllImport(CoreGraphics)]
    private static extern IntPtr CGImageCreate(
        nuint width, nuint height, nuint bitsPerComponent, nuint bitsPerPixel, nuint bytesPerRow,
        IntPtr space, uint bitmapInfo, IntPtr provider, IntPtr decode,
        [MarshalAs(UnmanagedType.I1)] bool shouldInterpolate, int intent);

    [DllImport(CoreGraphics)]
    public static extern void CGImageRelease(IntPtr image);

    [DllImport(CoreFoundation)]
    private static extern IntPtr CFDataCreate(IntPtr allocator, byte[] bytes, nint length);

    [DllImport(CoreFoundation)]
    private static extern void CFRelease(IntPtr cf);

    /// <summary>Creates a 32-bit image from raw bytes in the named color space (e.g. "kCGColorSpaceDisplayP3").</summary>
    public static IntPtr Create(int width, int height, byte[] bytes, uint bitmapInfo, string colorSpaceName)
    {
        var library = NativeLibrary.Load(CoreGraphics);
        var name = Marshal.ReadIntPtr(NativeLibrary.GetExport(library, colorSpaceName));
        var space = CGColorSpaceCreateWithName(name);
        var data = CFDataCreate(IntPtr.Zero, bytes, bytes.Length);
        var provider = CGDataProviderCreateWithCFData(data);
        try
        {
            return CGImageCreate((nuint)width, (nuint)height, 8, 32, (nuint)(width * 4), space, bitmapInfo, provider, IntPtr.Zero, false, 0);
        }
        finally
        {
            CGDataProviderRelease(provider);
            CFRelease(data);
            CGColorSpaceRelease(space);
        }
    }
}
