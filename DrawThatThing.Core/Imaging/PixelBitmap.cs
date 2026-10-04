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
        using var decoded = DecodeOriented(path);

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

    /// <summary>Decodes the image and applies the rotation or mirroring stored in its EXIF data, like image viewers do.</summary>
    private static SKBitmap DecodeOriented(string path)
    {
        using var codec = SKCodec.Create(path);
        var failure = $"Could not read the image \"{Path.GetFileName(path)}\".";
        if (codec == null)
        {
            throw new InvalidOperationException(failure);
        }

        var decoded = SKBitmap.Decode(codec) ?? throw new InvalidOperationException(failure);
        var origin = codec.EncodedOrigin;
        if (origin == SKEncodedOrigin.TopLeft || origin == SKEncodedOrigin.Default)
        {
            return decoded;
        }

        using (decoded)
        {
            var swap = origin is SKEncodedOrigin.LeftTop or SKEncodedOrigin.RightTop
                or SKEncodedOrigin.RightBottom or SKEncodedOrigin.LeftBottom;
            int w = decoded.Width, h = decoded.Height;
            var oriented = new SKBitmap(swap ? h : w, swap ? w : h, decoded.ColorType, decoded.AlphaType);
            using var canvas = new SKCanvas(oriented);
            // Maps the stored pixels to the upright image.
            var matrix = origin switch
            {
                SKEncodedOrigin.TopRight => new SKMatrix(-1, 0, w, 0, 1, 0, 0, 0, 1),
                SKEncodedOrigin.BottomRight => new SKMatrix(-1, 0, w, 0, -1, h, 0, 0, 1),
                SKEncodedOrigin.BottomLeft => new SKMatrix(1, 0, 0, 0, -1, h, 0, 0, 1),
                SKEncodedOrigin.LeftTop => new SKMatrix(0, 1, 0, 1, 0, 0, 0, 0, 1),
                SKEncodedOrigin.RightTop => new SKMatrix(0, -1, h, 1, 0, 0, 0, 0, 1),
                SKEncodedOrigin.RightBottom => new SKMatrix(0, -1, h, -1, 0, w, 0, 0, 1),
                SKEncodedOrigin.LeftBottom => new SKMatrix(0, 1, 0, -1, 0, w, 0, 0, 1),
                _ => SKMatrix.Identity
            };
            canvas.SetMatrix(matrix);
            canvas.DrawBitmap(decoded, 0, 0);
            return oriented;
        }
    }
}
