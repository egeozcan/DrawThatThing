using DrawThatThing.Core.Attributes;
using DrawThatThing.Core.Extensions;
using DrawThatThing.Core.Imaging;
using DrawThatThing.Core.Interfaces;
using DrawThatThing.Core.Models;

namespace DrawThatThing.Core.Readers;

/// <summary>
/// Fills connected areas of the same palette color with long strokes.
/// Needs one palette color marked as the background color, which is not drawn.
/// </summary>
[DefaultSetting("MinimumStrokeSize", 15)]
public class AbstractReader : IBitmapReader
{
    private readonly string _bitmapPath;
    private int _bitmapHeight;
    private int _bitmapWidth;
    private int[,] _colors = new int[0, 0];
    private bool[,] _colorsProcessed = new bool[0, 0];

    public AbstractReader(string path)
    {
        _bitmapPath = path;
    }

    public IEnumerable<MouseDragAction> GetDrawInstructions(List<ColorSpot> color, IDictionary<string, string>? settings = null, IBrushChanger? brushChanger = null)
    {
        List<ColorSpot> knownColors = color.ToList();
        var background = knownColors.FirstOrDefault(x => x.IsBackgroundColor)
            ?? throw new Exception("I need a background color!!");
        int ignoredColor = knownColors.IndexOf(background);
        // prevent drawing of single pixels etc
        int colorGroupMinSize = settings.GetIntOrDefault("MinimumStrokeSize", 15);

        var output = new List<MouseDragAction>();

        var bitmap = PixelBitmap.Load(_bitmapPath);
        _bitmapHeight = bitmap.Height;
        _bitmapWidth = bitmap.Width;
        _colors = new int[bitmap.Width, bitmap.Height];
        _colorsProcessed = new bool[bitmap.Width, bitmap.Height];
        var closestCache = new Dictionary<Color, int>();
        bitmap.LoopThroughPixels((point, currentColor) =>
        {
            if (!closestCache.TryGetValue(currentColor, out var closestIndex))
            {
                closestIndex = DetailedReader.GetClosestColorIndex(knownColors, currentColor);
                closestCache[currentColor] = closestIndex;
            }
            _colors[point.X, point.Y] = closestIndex;
        });

        var colorGroups = new List<List<Point>>();
        for (int x = 0; x < _bitmapWidth; x++)
        {
            for (int y = 0; y < _bitmapHeight; y++)
            {
                if (_colors[x, y] != ignoredColor && !_colorsProcessed[x, y])
                {
                    colorGroups.AddRange(GetNeighboringColors(x, y));
                }
            }
        }

        int? lastColorIndex = null;
        var colorGroupsOrderedByColor =
            colorGroups.Where(x => x.Count >= colorGroupMinSize).OrderBy(x => _colors[x[0].X, x[0].Y]);
        foreach (var colorGroup in colorGroupsOrderedByColor)
        {
            int currentColorIndex = _colors[colorGroup[0].X, colorGroup[0].Y];
            if (!lastColorIndex.HasValue || lastColorIndex.Value != currentColorIndex)
            {
                lastColorIndex = currentColorIndex;
                ColorSpot currentColor = knownColors[currentColorIndex];
                output.Add(new MouseDragAction([currentColor.Point], true, currentColor.Color));
            }
            output.Add(new MouseDragAction(colorGroup.Select(x => new Point(x.X, x.Y)).ToList()));
        }

        return output;
    }

    private static bool AreDirectNeighbors(Point point1, Point point2)
    {
        return Math.Abs(point1.X - point2.X) <= 15 && Math.Abs(point1.Y - point2.Y) <= 15;
    }

    private List<Point> GetMatchingUnprocessedDirectNeighbors(int x, int y)
    {
        var directNeighbors = new List<Point>(4);
        // top
        int newX = x;
        int newY = y - 1;
        if (newY >= 0 && !_colorsProcessed[newX, newY] && _colors[x, y] == _colors[newX, newY])
        {
            directNeighbors.Add(new Point(newX, newY));
        }
        // right
        newX = x + 1;
        newY = y;
        if (newX < _bitmapWidth && !_colorsProcessed[newX, newY] && _colors[x, y] == _colors[newX, newY])
        {
            directNeighbors.Add(new Point(newX, newY));
        }
        // bottom
        newX = x;
        newY = y + 1;
        if (newY < _bitmapHeight && !_colorsProcessed[newX, newY] && _colors[x, y] == _colors[newX, newY])
        {
            directNeighbors.Add(new Point(newX, newY));
        }
        // left
        newX = x - 1;
        newY = y;
        if (newX >= 0 && !_colorsProcessed[newX, newY] && _colors[x, y] == _colors[newX, newY])
        {
            directNeighbors.Add(new Point(newX, newY));
        }
        return directNeighbors;
    }

    private List<List<Point>> GetNeighboringColors(int x, int y)
    {
        var results = new List<List<Point>>();
        var result = new List<Point>();
        results.Add(result);
        var currentPoint = new Point(x, y);
        _colorsProcessed[x, y] = true;
        result.Add(currentPoint);

        // The original implementation inserted every newly found neighbor at the current
        // read position of a list, which behaves exactly like a stack. A real stack keeps
        // the stroke order identical while avoiding the quadratic list inserts.
        var pending = new Stack<Point>();
        pending.Push(currentPoint);

        while (pending.Count > 0)
        {
            Point neighbor = pending.Pop();
            foreach (Point newNeighbor in GetMatchingUnprocessedDirectNeighbors(neighbor.X, neighbor.Y))
            {
                _colorsProcessed[newNeighbor.X, newNeighbor.Y] = true;
                if (!AreDirectNeighbors(result[^1], newNeighbor))
                {
                    result = [];
                    results.Add(result);
                }
                pending.Push(newNeighbor);
                result.Add(newNeighbor);
            }
        }
        return results;
    }
}
