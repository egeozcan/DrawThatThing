using System.Diagnostics;
using DrawThatThing.Core.Imaging;
using DrawThatThing.Core.Models;
using DrawThatThing.Core.Readers;
using SkiaSharp;

namespace DrawThatThing.Tests.Core;

public class DetailedReaderTests : IDisposable
{
    private const int RandomImageCount = 200;

    private static readonly Color White = Color.White;
    private static readonly Color Black = Color.Black;
    private static readonly Color Red = new(200, 30, 30);

    private readonly string _directory = Path.Combine(Path.GetTempPath(), "DetailedReaderTests-" + Guid.NewGuid().ToString("N"));
    private int _imageCount;

    public DetailedReaderTests()
    {
        Directory.CreateDirectory(_directory);
    }

    public void Dispose()
    {
        try
        {
            Directory.Delete(_directory, true);
        }
        catch (IOException)
        {
        }
    }

    [Fact]
    public void ChainsNeighboringPixelsAtBothEndsOfAStroke()
    {
        // Column by column: (0,0) (0,1) (0,2) form a stroke, (1,0) only touches its first point
        // and is put in front, (1,1) and (1,2) continue from its last point.
        var palette = new List<ColorSpot> { new(Black, new Point(100, 100)), new(White, new Point(200, 100)) };
        string path = SaveImage(new[,] { { Black, Black, Black }, { Black, Black, Black } });

        var actions = new DetailedReader(path).GetDrawInstructions(palette);

        Assert.Equal(
        [
            "click #000000FF 100,100",
            "drag #00000000 1,0 0,0 0,1 0,2 1,1 1,2",
        ], Describe(actions));
    }

    [Fact]
    public void MatchesTheOriginalAlgorithmOnRandomImages()
    {
        for (int seed = 0; seed < RandomImageCount; seed++)
        {
            var random = new Random(seed);
            var palette = RandomPalette(random);
            var image = RandomImage(random, palette);
            AssertMatchesReference(image, palette, $"seed {seed}");
        }
    }

    [Fact]
    public void MatchesTheOriginalAlgorithmOnASinglePixel()
    {
        var palette = new List<ColorSpot> { new(Red, new Point(1, 2)), new(White, new Point(3, 4)) };
        AssertMatchesReference(new[,] { { Red } }, palette, "red pixel");
        AssertMatchesReference(new[,] { { White } }, palette, "white pixel");
    }

    [Fact]
    public void MatchesTheOriginalAlgorithmOnAWhiteImage()
    {
        var palette = new List<ColorSpot> { new(Red, new Point(1, 2)), new(White, new Point(3, 4)) };
        AssertMatchesReference(SolidImage(7, 5, White), palette, "white image");
    }

    [Fact]
    public void MatchesTheOriginalAlgorithmWithAnEmptyPalette()
    {
        var image = RandomImage(new Random(42), [new(Red, new Point()), new(White, new Point())]);
        AssertMatchesReference(image, [], "empty palette");
    }

    [Fact]
    public void MatchesTheOriginalAlgorithmWithOnlyWhiteInThePalette()
    {
        var image = RandomImage(new Random(43), [new(Red, new Point()), new(White, new Point())]);
        AssertMatchesReference(image, [new(White, new Point(3, 4))], "only white");
    }

    [Fact]
    public void MatchesTheOriginalAlgorithmWithDuplicatePaletteColors()
    {
        var black = new ColorSpot(Black, new Point(5, 5));
        var palette = new List<ColorSpot>
        {
            new(Red, new Point(1, 1)),
            new(White, new Point(2, 2)),
            new(Red, new Point(3, 3)),
            black,
            new(White, new Point(4, 4)),
            black,
        };
        var image = RandomImage(new Random(44), palette);
        AssertMatchesReference(image, palette, "duplicate colors");
    }

    [Fact]
    public void ParsesALargeNoisyImageQuickly()
    {
        // The original implementation compared every pixel with all strokes of its color and needed about 20 s for this.
        var palette = new List<ColorSpot>
        {
            new(White, new Point(10, 10)),
            new(Black, new Point(20, 10)),
            new(Red, new Point(30, 10)),
            new(new Color(30, 160, 60), new Point(40, 10)),
            new(new Color(40, 60, 200), new Point(50, 10)),
            new(new Color(240, 210, 40), new Point(60, 10)),
        };
        string path = SaveImage(PhotoLikeImage(800, 600, new Random(1), noise: 10));

        var stopwatch = Stopwatch.StartNew();
        var actions = new DetailedReader(path).GetDrawInstructions(palette).ToList();
        stopwatch.Stop();

        Assert.True(actions.Count > palette.Count);
        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(3), $"Parsing took {stopwatch.Elapsed.TotalSeconds:0.00} s");
    }

    private void AssertMatchesReference(Color[,] image, List<ColorSpot> palette, string description)
    {
        string path = SaveImage(image);
        // The original clicked every palette color; colors without strokes are not worth a click.
        var expected = Describe(ReferenceGetDrawInstructions(path, palette.ToList()));
        expected = expected.Where((line, i) => !line.StartsWith("click") || (i + 1 < expected.Count && expected[i + 1].StartsWith("drag"))).ToList();
        var actual = Describe(new DetailedReader(path).GetDrawInstructions(palette.ToList()));

        Assert.True(expected.Count == actual.Count, $"{description}: expected {expected.Count} actions, got {actual.Count}");
        for (int i = 0; i < expected.Count; i++)
        {
            Assert.True(expected[i] == actual[i], $"{description}: action {i} differs\nexpected: {expected[i]}\nactual:   {actual[i]}");
        }
    }

    private static List<string> Describe(IEnumerable<MouseDragAction> actions)
    {
        return actions.Select(action =>
        {
            string kind = action.DiscardOffset ? "click" : "drag";
            string color = $"{action.Color.ToHex()}{action.Color.A:X2}";
            string points = string.Join(" ", action.Points.Select(p => p.IsEmpty ? "empty" : $"{p.X},{p.Y}"));
            return $"{kind} {color} {points}";
        }).ToList();
    }

    /// <summary>Writes the pixels (indexed [x, y]) to a PNG file and returns its path.</summary>
    private string SaveImage(Color[,] image)
    {
        int width = image.GetLength(0);
        int height = image.GetLength(1);
        using var bitmap = new SKBitmap(new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul));
        var pixels = new SKColor[width * height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color c = image[x, y];
                pixels[y * width + x] = new SKColor(c.R, c.G, c.B, c.A);
            }
        }
        bitmap.Pixels = pixels;

        string path = Path.Combine(_directory, $"{_imageCount++}.png");
        using var data = bitmap.Encode(SKEncodedImageFormat.Png, 100);
        File.WriteAllBytes(path, data.ToArray());
        return path;
    }

    private static Color[,] SolidImage(int width, int height, Color color)
    {
        var image = new Color[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                image[x, y] = color;
            }
        }
        return image;
    }

    private static Color RandomColor(Random random)
    {
        return new Color((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
    }

    private static List<ColorSpot> RandomPalette(Random random)
    {
        var palette = new List<ColorSpot>();
        int count = random.Next(2, 7);
        for (int i = 0; i < count; i++)
        {
            Color color = random.Next(4) switch
            {
                0 when palette.Count > 0 => palette[random.Next(palette.Count)].Color,
                1 => White,
                _ => RandomColor(random),
            };
            Point point = random.Next(10) == 0 ? Point.Empty : new Point(random.Next(1000), random.Next(1000));
            palette.Insert(random.Next(palette.Count + 1), new ColorSpot(color, point));
        }
        return palette;
    }

    /// <summary>
    /// Random blobs, lines and noise in (mostly) palette colors, so that strokes grow, meet and
    /// get extended at both ends in many different ways.
    /// </summary>
    private static Color[,] RandomImage(Random random, List<ColorSpot> palette)
    {
        int width = random.Next(1, 41);
        int height = random.Next(1, 41);

        Color PickColor()
        {
            return random.Next(8) == 0 ? RandomColor(random) : palette[random.Next(palette.Count)].Color;
        }

        var image = SolidImage(width, height, random.Next(3) == 0 ? White : PickColor());
        int style = random.Next(4);
        if (style == 0)
        {
            // pure noise: lots of tiny strokes that touch each other
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    image[x, y] = PickColor();
                }
            }
            return image;
        }
        if (style == 1)
        {
            return PhotoLikeImage(width, height, random, noise: random.Next(0, 80));
        }

        int shapeCount = random.Next(1, 15);
        for (int i = 0; i < shapeCount; i++)
        {
            Color color = PickColor();
            int cx = random.Next(width);
            int cy = random.Next(height);
            int rx = random.Next(1, Math.Max(2, width / 2));
            int ry = random.Next(1, Math.Max(2, height / 2));
            int shape = random.Next(3);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    double dx = (x - cx) / (double)rx;
                    double dy = (y - cy) / (double)ry;
                    bool inside = shape switch
                    {
                        0 => dx * dx + dy * dy <= 1,
                        1 => Math.Abs(dx) <= 1 && Math.Abs(dy) <= 1,
                        _ => Math.Abs((x - cx) * ry - (y - cy) * rx) <= Math.Max(rx, ry),
                    };
                    if (inside)
                    {
                        image[x, y] = color;
                    }
                }
            }
        }

        int noise = style == 2 ? 0 : random.Next(1, 30);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (random.Next(100) < noise)
                {
                    image[x, y] = PickColor();
                }
            }
        }
        return image;
    }

    /// <summary>Smooth color gradients with per-pixel noise, roughly what a photo looks like.</summary>
    private static Color[,] PhotoLikeImage(int width, int height, Random random, int noise)
    {
        double phase = random.NextDouble() * 10;
        var image = new Color[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                double r = 128 + 90 * Math.Sin(x * 0.013 + y * 0.007 + phase) + 40 * Math.Sin(x * 0.051) * Math.Cos(y * 0.043);
                double g = 128 + 90 * Math.Cos(x * 0.011 - y * 0.017 + phase) + 40 * Math.Sin(y * 0.061);
                double b = 128 + 90 * Math.Sin((x + y) * 0.009 + phase) + 40 * Math.Cos(x * 0.037);
                image[x, y] = new Color(Noisy(r, noise, random), Noisy(g, noise, random), Noisy(b, noise, random));
            }
        }
        return image;
    }

    private static byte Noisy(double value, int noise, Random random)
    {
        return (byte)Math.Clamp((int)value + random.Next(-noise, noise + 1), 0, 255);
    }

    /// <summary>
    /// The original, quadratic implementation of <see cref="DetailedReader"/>, kept verbatim as the
    /// reference for the expected output.
    /// </summary>
    private static List<MouseDragAction> ReferenceGetDrawInstructions(string bitmapPath, List<ColorSpot> paletteColorSpots)
    {
        var ignoredSpot = paletteColorSpots.FirstOrDefault(x => x.Color.R == 255 && x.Color.G == 255 && x.Color.B == 255);
        int ignoreColorIndex = ignoredSpot == null ? -1 : paletteColorSpots.IndexOf(ignoredSpot);

        if (paletteColorSpots.Count == 0)
        {
            paletteColorSpots = [new ColorSpot { Color = Color.Black, Point = Point.Empty }];
        }

        var output = new List<MouseDragAction>();

        var bitmap = PixelBitmap.Load(bitmapPath);
        int bitmapHeight = bitmap.Height;
        int bitmapWidth = bitmap.Width;

        var colors = new int[bitmapWidth, bitmapHeight];
        bitmap.LoopThroughPixels((point, currentColor) =>
        {
            ColorSpot matchingColor = paletteColorSpots.OrderBy(c => c.Color.DifferenceTo(currentColor)).First();
            colors[point.X, point.Y] = paletteColorSpots.IndexOf(matchingColor);
        });

        int colorCount = paletteColorSpots.Count;
        var strokesForEachColor = new List<MouseDragAction>[colorCount];

        for (int i = 0; i < colorCount; i++)
        {
            ColorSpot paletteColorSpot = paletteColorSpots[i];
            strokesForEachColor[i] = [new MouseDragAction([paletteColorSpot.Point], true, paletteColorSpot.Color)];
        }

        for (int x = 0; x < bitmapWidth; x++)
        {
            for (int y = 0; y < bitmapHeight; y++)
            {
                int currentPixelColorIndex = colors[x, y];
                if (currentPixelColorIndex == ignoreColorIndex)
                {
                    continue;
                }
                var currentPixelPoint = new Point(x, y);
                List<MouseDragAction> strokesForCurrentColor = strokesForEachColor[currentPixelColorIndex];
                bool found = false;

                foreach (MouseDragAction stroke in strokesForCurrentColor)
                {
                    if (stroke.DiscardOffset)
                    {
                        continue;
                    }
                    Point lastPoint = stroke.Points[^1];
                    if (PointsAreNeighbors(lastPoint.X, lastPoint.Y, x, y))
                    {
                        stroke.PushPoint(new Point(x, y));
                        found = true;
                        break;
                    }
                    Point firstPoint = stroke.Points[0];
                    if (PointsAreNeighbors(firstPoint.X, firstPoint.Y, x, y))
                    {
                        stroke.AddPoint(new Point(x, y));
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    continue;
                }

                strokesForEachColor[currentPixelColorIndex].Add(new MouseDragAction([currentPixelPoint]));
            }
        }

        foreach (var mouseDragActions in strokesForEachColor)
        {
            output.AddRange(mouseDragActions);
        }

        return output;
    }

    private static bool PointsAreNeighbors(int x1, int y1, int x2, int y2)
    {
        return Math.Abs(x1 - x2) <= 1 && Math.Abs(y1 - y2) <= 1;
    }
}
