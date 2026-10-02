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
    private int _bitmapHeight;
    private int[,] _colors = new int[0, 0];
    private Stroke?[,] _strokeOfPixel = new Stroke?[0, 0];
    private int[] _nextPixel = [];

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

        var bitmap = PixelBitmap.Load(_bitmapPath);
        _bitmapHeight = bitmap.Height;
        int bitmapWidth = bitmap.Width;

        _colors = new int[bitmapWidth, _bitmapHeight];
        bitmap.LoopThroughPixels((point, currentColor) =>
        {
            _colors[point.X, point.Y] = GetClosestColorIndex(paletteColorSpots, currentColor);
        });

        int colorCount = paletteColorSpots.Count;
        var strokesForEachColor = new List<Stroke>[colorCount];
        for (int i = 0; i < colorCount; i++)
        {
            strokesForEachColor[i] = [];
        }

        _strokeOfPixel = new Stroke?[bitmapWidth, _bitmapHeight];
        _nextPixel = new int[bitmapWidth * _bitmapHeight];

        for (int x = 0; x < bitmapWidth; x++)
        {
            for (int y = 0; y < _bitmapHeight; y++)
            {
                int currentPixelColorIndex = _colors[x, y];
                if (currentPixelColorIndex == ignoreColorIndex)
                {
                    continue;
                }

                // Pixels are visited column by column, so the only pixels that can already be in a
                // stroke and touch this one are the three on the left and the one above. The oldest
                // stroke that starts or ends on one of them gets the pixel.
                Stroke? stroke = OlderStrokeEndingAt(x - 1, y - 1, currentPixelColorIndex, null);
                stroke = OlderStrokeEndingAt(x - 1, y, currentPixelColorIndex, stroke);
                stroke = OlderStrokeEndingAt(x - 1, y + 1, currentPixelColorIndex, stroke);
                stroke = OlderStrokeEndingAt(x, y - 1, currentPixelColorIndex, stroke);

                int pixel = GetPixelIndex(x, y);
                if (stroke == null)
                {
                    List<Stroke> strokesForCurrentColor = strokesForEachColor[currentPixelColorIndex];
                    stroke = new Stroke(strokesForCurrentColor.Count, pixel);
                    strokesForCurrentColor.Add(stroke);
                }
                else if (PointsAreNeighbors(GetX(stroke.Last), GetY(stroke.Last), x, y))
                {
                    _nextPixel[stroke.Last] = pixel;
                    stroke.Last = pixel;
                    stroke.Length++;
                }
                else
                {
                    _nextPixel[pixel] = stroke.First;
                    stroke.First = pixel;
                    stroke.Length++;
                }
                _strokeOfPixel[x, y] = stroke;
            }
        }

        var output = new List<MouseDragAction>();
        for (int i = 0; i < colorCount; i++)
        {
            ColorSpot paletteColorSpot = paletteColorSpots[i];
            output.Add(new MouseDragAction([paletteColorSpot.Point], true, paletteColorSpot.Color));
            foreach (Stroke stroke in strokesForEachColor[i])
            {
                output.Add(new MouseDragAction(GetPoints(stroke)));
            }
        }

        return output;
    }

    /// <summary>
    /// Index of the palette color closest to the given color, the first one if several are equally close.
    /// </summary>
    private static int GetClosestColorIndex(List<ColorSpot> paletteColorSpots, Color color)
    {
        int closestIndex = 0;
        int closestDifference = int.MaxValue;
        for (int i = 0; i < paletteColorSpots.Count; i++)
        {
            int difference = paletteColorSpots[i].Color.DifferenceTo(color);
            if (difference < closestDifference)
            {
                closestIndex = i;
                closestDifference = difference;
            }
        }
        return closestIndex;
    }

    /// <summary>
    /// Returns the stroke of the given color that starts or ends on (x, y) if there is one and it is
    /// older than <paramref name="olderStroke"/>, otherwise <paramref name="olderStroke"/>.
    /// </summary>
    private Stroke? OlderStrokeEndingAt(int x, int y, int colorIndex, Stroke? olderStroke)
    {
        if (x < 0 || y < 0 || y >= _bitmapHeight || _colors[x, y] != colorIndex)
        {
            return olderStroke;
        }
        Stroke? stroke = _strokeOfPixel[x, y];
        if (stroke == null || (olderStroke != null && olderStroke.Order < stroke.Order))
        {
            return olderStroke;
        }
        int pixel = GetPixelIndex(x, y);
        return stroke.First == pixel || stroke.Last == pixel ? stroke : olderStroke;
    }

    private List<Point> GetPoints(Stroke stroke)
    {
        var points = new List<Point>(stroke.Length);
        for (int pixel = stroke.First; points.Count < stroke.Length; pixel = _nextPixel[pixel])
        {
            points.Add(new Point(GetX(pixel), GetY(pixel)));
        }
        return points;
    }

    private int GetPixelIndex(int x, int y) => x * _bitmapHeight + y;

    private int GetX(int pixel) => pixel / _bitmapHeight;

    private int GetY(int pixel) => pixel % _bitmapHeight;

    private static bool PointsAreNeighbors(int x1, int y1, int x2, int y2)
    {
        return Math.Abs(x1 - x2) <= 1 && Math.Abs(y1 - y2) <= 1;
    }

    /// <summary>
    /// A stroke being built: a chain of pixels linked through <see cref="_nextPixel"/>, so pixels can be
    /// added to either end without moving the others.
    /// </summary>
    private sealed class Stroke
    {
        /// <summary>Position among the strokes of the same color, lower is older.</summary>
        public int Order { get; }
        public int First { get; set; }
        public int Last { get; set; }
        public int Length { get; set; } = 1;

        public Stroke(int order, int pixel)
        {
            Order = order;
            First = pixel;
            Last = pixel;
        }
    }
}
