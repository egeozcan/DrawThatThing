using DrawThatThing.Core.Imaging;
using DrawThatThing.Core.Interfaces;
using DrawThatThing.Core.Models;

namespace DrawThatThing.Core.Readers;

/// <summary>
/// Chains neighboring pixels of the same palette color into strokes. White is not drawn.
/// </summary>
public class DetailedReader : IBitmapReader
{
    private readonly string _bitmapPath;
    private int[,] _colors = new int[0, 0];

    public DetailedReader(string path)
    {
        _bitmapPath = path;
    }

    public IEnumerable<MouseDragAction> GetDrawInstructions(List<ColorSpot> paletteColorSpots, IDictionary<string, string>? settings = null, IBrushChanger? brushChanger = null)
    {
        var ignoredSpot = paletteColorSpots.FirstOrDefault(x => x.Color.R == 255 && x.Color.G == 255 && x.Color.B == 255);
        int ignoreColorIndex = ignoredSpot == null ? -1 : paletteColorSpots.IndexOf(ignoredSpot);

        if (paletteColorSpots.Count == 0)
        {
            paletteColorSpots = [new ColorSpot { Color = Color.Black, Point = Point.Empty }];
        }

        var output = new List<MouseDragAction>();

        var bitmap = PixelBitmap.Load(_bitmapPath);
        int bitmapHeight = bitmap.Height;
        int bitmapWidth = bitmap.Width;

        _colors = new int[bitmapWidth, bitmapHeight];
        bitmap.LoopThroughPixels((point, currentColor) =>
        {
            ColorSpot matchingColor = paletteColorSpots.OrderBy(c => c.Color.DifferenceTo(currentColor)).First();
            _colors[point.X, point.Y] = paletteColorSpots.IndexOf(matchingColor);
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
                int currentPixelColorIndex = _colors[x, y];
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
