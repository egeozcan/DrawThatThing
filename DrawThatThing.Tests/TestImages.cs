using SkiaSharp;

namespace DrawThatThing.Tests;

public static class TestImages
{
    /// <summary>Writes a PNG of the given size filled with white except for the given black pixels.</summary>
    public static string CreatePng(int width, int height, params (int X, int Y)[] blackPixels)
    {
        using var bitmap = new SKBitmap(width, height);
        bitmap.Erase(SKColors.White);
        foreach (var (x, y) in blackPixels)
        {
            bitmap.SetPixel(x, y, SKColors.Black);
        }

        var path = Path.Combine(Path.GetTempPath(), $"dtt-test-{Guid.NewGuid():N}.png");
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        File.WriteAllBytes(path, data.ToArray());
        return path;
    }
}
