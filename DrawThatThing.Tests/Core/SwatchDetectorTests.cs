using DrawThatThing.Core.Imaging;

namespace DrawThatThing.Tests.Core;

public class SwatchDetectorTests
{
    private const int Width = 200, Height = 100;

    /// <summary>A gray panel with a 2 x 3 grid of 12-pixel swatches, 4 pixels apart, starting at (20, 20).</summary>
    private static byte[] Palette(Action<byte[]>? tweak = null)
    {
        var pixels = new byte[Width * Height * 4];
        Fill(pixels, 0, 0, Width, Height, (192, 192, 192));
        var colors = new (byte, byte, byte)[] { (1, 2, 3), (255, 0, 0), (0, 255, 0), (0, 0, 255), (255, 255, 0), (255, 255, 255) };
        for (var i = 0; i < colors.Length; i++)
        {
            Fill(pixels, 20 + i % 3 * 16, 20 + i / 3 * 16, 12, 12, colors[i]);
        }
        tweak?.Invoke(pixels);
        return pixels;
    }

    private static void Fill(byte[] pixels, int x, int y, int w, int h, (byte R, byte G, byte B) color)
    {
        for (var row = y; row < y + h; row++)
        {
            for (var column = x; column < x + w; column++)
            {
                var o = (row * Width + column) * 4;
                (pixels[o], pixels[o + 1], pixels[o + 2], pixels[o + 3]) = (color.R, color.G, color.B, 255);
            }
        }
    }

    [Fact]
    public void FindsEverySwatchRowByRow()
    {
        var swatches = SwatchDetector.Find(Palette(), Width, Height, 26, 26);

        Assert.Equal(
            [(26, 26, 1, 2, 3), (42, 26, 255, 0, 0), (58, 26, 0, 255, 0), (26, 42, 0, 0, 255), (42, 42, 255, 255, 0), (58, 42, 255, 255, 255)],
            swatches.Select(s => (s.CenterX, s.CenterY, (int)s.R, (int)s.G, (int)s.B)));
    }

    [Fact]
    public void WorksFromAnyOfTheSwatches()
    {
        var swatches = SwatchDetector.Find(Palette(), Width, Height, 58, 42);

        Assert.Equal(6, swatches.Count);
    }

    [Fact]
    public void PointingAtTheBorderOrTheBackgroundFindsNothing()
    {
        Assert.Empty(SwatchDetector.Find(Palette(), Width, Height, 33, 26)); // the gap between two swatches
        Assert.Empty(SwatchDetector.Find(Palette(), Width, Height, 150, 80)); // the big panel
    }

    [Fact]
    public void LeavesOutShapesOfADifferentSize()
    {
        var pixels = Palette(p => Fill(p, 120, 20, 30, 12, (9, 9, 9)));

        var swatches = SwatchDetector.Find(pixels, Width, Height, 26, 26);

        Assert.Equal(6, swatches.Count);
    }

    [Fact]
    public void LeavesOutSpecksAndDitheredAreas()
    {
        var pixels = Palette(p =>
        {
            for (var i = 0; i < 12; i++)
            {
                for (var j = 0; j < 12; j++)
                {
                    Fill(p, 120 + i, 20 + j, 1, 1, (i + j) % 2 == 0 ? ((byte)0, (byte)0, (byte)0) : ((byte)255, (byte)255, (byte)255));
                }
            }
            Fill(p, 150, 20, 2, 2, (5, 6, 7));
        });

        Assert.Equal(6, SwatchDetector.Find(pixels, Width, Height, 26, 26).Count);
    }

    [Fact]
    public void LeavesOutRings()
    {
        var pixels = Palette(p =>
        {
            Fill(p, 120, 20, 12, 12, (9, 9, 9));
            Fill(p, 123, 23, 6, 6, (192, 192, 192));
        });

        Assert.Equal(6, SwatchDetector.Find(pixels, Width, Height, 26, 26).Count);
    }

    [Fact]
    public void IgnoresSwatchesCutOffByTheEdgeOfTheImage()
    {
        var pixels = Palette(p => Fill(p, Width - 12, 20, 12, 12, (9, 9, 9)));

        Assert.Equal(6, SwatchDetector.Find(pixels, Width, Height, 26, 26).Count);
    }

    [Fact]
    public void IgnoresTheAlphaChannel()
    {
        // Windows screen captures leave it at 0.
        var pixels = Palette();
        for (var i = 3; i < pixels.Length; i += 4)
        {
            pixels[i] = 0;
        }

        Assert.Equal(6, SwatchDetector.Find(pixels, Width, Height, 26, 26).Count);
    }

    [Fact]
    public void APointOutsideTheImageFindsNothing()
    {
        Assert.Empty(SwatchDetector.Find(Palette(), Width, Height, -1, 5));
        Assert.Empty(SwatchDetector.Find(Palette(), Width, Height, 5, Height));
        Assert.Empty(SwatchDetector.Find([], Width, Height, 5, 5));
    }
}
