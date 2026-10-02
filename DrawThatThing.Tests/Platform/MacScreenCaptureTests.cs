using DrawThatThing.Platform.macOS;
using SkiaSharp;
using static DrawThatThing.Tests.Platform.CoreGraphicsImages;

namespace DrawThatThing.Tests.Platform;

public class MacScreenCaptureTests
{
    private static (byte R, byte G, byte B) ReadPixel(IntPtr image)
    {
        try
        {
            var pixels = MacScreenCapture.ReadSrgbPixels(image, 1, 1);
            Assert.NotNull(pixels);
            return (pixels[0], pixels[1], pixels[2]);
        }
        finally
        {
            CGImageRelease(image);
        }
    }

    [MacOSFact]
    public void WithoutScreenRecordingAccessNoColorIsMadeUp()
    {
        var capture = new MacScreenCapture(hasAccess: () => false);

        Assert.Null(capture.GetPixelColor(10, 10));
    }

    [MacOSFact]
    public void ScreenPixelsAreConvertedFromTheDisplayColorSpaceToSrgb()
    {
        // Pure sRGB red, as a wide-gamut (Display P3) screen stores it. Images and palettes are in sRGB.
        var image = Create(1, 1, [234, 51, 35, 255], NoneSkipLastBigEndian, "kCGColorSpaceDisplayP3");

        var (r, g, b) = ReadPixel(image);

        Assert.InRange(r, 252, 255);
        Assert.InRange(g, 0, 3);
        Assert.InRange(b, 0, 3);
    }

    [MacOSFact]
    public void ReadsTheChannelsInTheRightOrder()
    {
        var image = Create(1, 1, [30, 20, 10, 255], PremultipliedFirstLittleEndian, "kCGColorSpaceSRGB");

        Assert.Equal(((byte)10, (byte)20, (byte)30), ReadPixel(image));
    }

    [MacOSFact]
    public void OnRetinaScreensOnePhysicalPixelIsTakenInsteadOfABlend()
    {
        // A one-point region covers 2x2 pixels on a Retina screen.
        var image = Create(2, 2, [255, 0, 0, 255, 0, 0, 255, 255, 0, 255, 0, 255, 0, 0, 0, 255], NoneSkipLastBigEndian, "kCGColorSpaceSRGB");

        var pixel = ReadPixel(image);

        Assert.Contains(pixel, new[] { ((byte)255, (byte)0, (byte)0), ((byte)0, (byte)0, (byte)255), ((byte)0, (byte)255, (byte)0), ((byte)0, (byte)0, (byte)0) });
    }

    [MacOSFact]
    public void ReadsScreenshotFilesFromTheScreencaptureTool()
    {
        using var bitmap = new SKBitmap(2, 2);
        bitmap.Erase(new SKColor(10, 20, 30));
        using var png = SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100);

        var image = MacScreenCapture.CreateImageFromFileData(png.ToArray());

        Assert.NotEqual(IntPtr.Zero, image);
        Assert.Equal(((byte)10, (byte)20, (byte)30), ReadPixel(image));
    }
}
