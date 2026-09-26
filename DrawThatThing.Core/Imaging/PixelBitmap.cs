using DrawThatThing.Core.Models;
using SkiaSharp;

namespace DrawThatThing.Core.Imaging;

/// <summary>
/// A decoded image whose pixels can be read as <see cref="Color"/> values.
/// Transparent areas are flattened onto white, which is what they look like on a canvas.
/// </summary>
public sealed class PixelBitmap
{
    private readonly Color[] _pixels;

    public int Width { get; }
    public int Height { get; }

    private PixelBitmap(int width, int height, Color[] pixels)
    {
        Width = width;
        Height = height;
        _pixels = pixels;
    }

    public Color GetPixel(int x, int y) => _pixels[y * Width + x];

    public static PixelBitmap Load(string path)
    {
        using var decoded = SKBitmap.Decode(path)
            ?? throw new InvalidOperationException($"Could not read the image \"{Path.GetFileName(path)}\".");

        var info = new SKImageInfo(decoded.Width, decoded.Height, SKColorType.Rgba8888, SKAlphaType.Premul);
        using var flattened = new SKBitmap(info);
        using (var canvas = new SKCanvas(flattened))
        {
            canvas.Clear(SKColors.White);
            canvas.DrawBitmap(decoded, 0, 0);
        }

        var pixels = new Color[decoded.Width * decoded.Height];
        var source = flattened.Pixels;
        for (int i = 0; i < source.Length; i++)
        {
            var c = source[i];
            pixels[i] = new Color(c.Red, c.Green, c.Blue);
        }

        return new PixelBitmap(decoded.Width, decoded.Height, pixels);
    }

    /// <summary>
    /// Visits every pixel column by column (x outer, y inner), like the original GDI+ readers did.
    /// </summary>
    public void LoopThroughPixels(Action<Point, Color> action)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                action(new Point(x, y), GetPixel(x, y));
            }
        }
    }
}
