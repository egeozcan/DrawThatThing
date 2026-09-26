using DrawThatThing.Core.Models;
using SkiaSharp;

namespace DrawThatThing.Core.Imaging;

/// <summary>
/// Draws the mouse drag actions onto an image, showing what playing them will produce.
/// </summary>
public static class PreviewRenderer
{
    public static byte[] RenderPng(IEnumerable<MouseDragAction> actions, int width, int height)
    {
        width = Math.Max(1, width);
        height = Math.Max(1, height);

        using var bitmap = new SKBitmap(new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul));
        using (var canvas = new SKCanvas(bitmap))
        using (var paint = new SKPaint { IsAntialias = false, StrokeWidth = 0, Style = SKPaintStyle.Stroke, Color = SKColors.Black })
        {
            canvas.Clear(SKColors.Transparent);
            foreach (var action in actions)
            {
                if (!action.Color.IsEmpty)
                {
                    paint.Color = new SKColor(action.Color.R, action.Color.G, action.Color.B);
                }

                // Color changes click on the palette, which is somewhere on the screen and not part of the image.
                if (action.DiscardOffset || action.Points.Count == 0)
                {
                    continue;
                }

                var lastPoint = action.Points[0];
                foreach (var point in action.Points)
                {
                    if (point.X == lastPoint.X && point.Y == lastPoint.Y)
                    {
                        canvas.DrawPoint(point.X + 0.5f, point.Y + 0.5f, paint);
                    }
                    else
                    {
                        canvas.DrawLine(lastPoint.X + 0.5f, lastPoint.Y + 0.5f, point.X + 0.5f, point.Y + 0.5f, paint);
                        canvas.DrawPoint(point.X + 0.5f, point.Y + 0.5f, paint);
                    }
                    lastPoint = point;
                }
            }
        }

        using var image = SKImage.FromBitmap(bitmap);
        using var encoded = image.Encode(SKEncodedImageFormat.Png, 100);
        return encoded.ToArray();
    }
}
