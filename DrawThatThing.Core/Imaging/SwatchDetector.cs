namespace DrawThatThing.Core.Imaging;

/// <summary>A flat-colored square-ish area of the screen, e.g. one color of a program's palette.</summary>
/// <param name="X">Left edge in capture coordinates.</param>
/// <param name="Y">Top edge in capture coordinates.</param>
public readonly record struct Swatch(int X, int Y, int Width, int Height, byte R, byte G, byte B)
{
    public int CenterX => X + Width / 2;
    public int CenterY => Y + Height / 2;
}

/// <summary>
/// Finds the color swatches of a palette in a screenshot: areas of exactly one color that are about as big as
/// the swatch the user pointed at.
/// </summary>
public static class SwatchDetector
{
    /// <summary>Smaller areas are specks (text, borders, dithering), not something that could be clicked.</summary>
    public const int MinimumSize = 4;

    /// <summary>How much the size of a swatch may differ from the reference swatch's, as a fraction.</summary>
    private const double SizeTolerance = 0.25;

    /// <summary>How much of its bounding box an area must fill; excludes rings, L shapes and gradients' remains.</summary>
    private const double MinimumFill = 0.9;

    /// <summary>
    /// Returns the swatches that look like the one at (<paramref name="referenceX"/>, <paramref name="referenceY"/>),
    /// the reference swatch included, ordered row by row and from left to right. Empty when the reference point
    /// is not on a swatch. Areas touching the edge of the image are ignored, since they may be cut off.
    /// </summary>
    /// <param name="rgba">Pixels as RGBA bytes, row by row. The alpha channel is ignored.</param>
    public static IReadOnlyList<Swatch> Find(byte[] rgba, int width, int height, int referenceX, int referenceY)
    {
        if (width <= 0 || height <= 0 || rgba.Length < width * height * 4
            || referenceX < 0 || referenceY < 0 || referenceX >= width || referenceY >= height)
        {
            return [];
        }

        var regions = FindRegions(rgba, width, height, out var labels);
        var reference = regions[labels[referenceY * width + referenceX]];
        if (!reference.IsSwatch)
        {
            return [];
        }

        var matches = regions
            .Where(r => r.IsSwatch
                        && SimilarSize(r.Width, reference.Width)
                        && SimilarSize(r.Height, reference.Height))
            .Select(r => r.ToSwatch())
            .ToList();
        return SortIntoRows(matches, reference.Height);
    }

    private static bool SimilarSize(int size, int referenceSize)
    {
        var tolerance = Math.Max(2, referenceSize * SizeTolerance);
        return Math.Abs(size - referenceSize) <= tolerance;
    }

    private static List<Swatch> SortIntoRows(List<Swatch> swatches, int referenceHeight)
    {
        var sorted = new List<Swatch>(swatches.Count);
        var byTop = swatches.OrderBy(s => s.Y).ThenBy(s => s.X).ToList();
        var row = new List<Swatch>();
        var rowTop = 0;
        foreach (var swatch in byTop)
        {
            if (row.Count > 0 && swatch.Y - rowTop > referenceHeight / 2)
            {
                sorted.AddRange(row.OrderBy(s => s.X));
                row.Clear();
            }
            if (row.Count == 0)
            {
                rowTop = swatch.Y;
            }
            row.Add(swatch);
        }
        sorted.AddRange(row.OrderBy(s => s.X));
        return sorted;
    }

    private sealed class Region
    {
        public int MinX = int.MaxValue, MinY = int.MaxValue, MaxX = int.MinValue, MaxY = int.MinValue;
        public int Area;
        public byte R, G, B;
        public bool TouchesEdge;

        public int Width => MaxX - MinX + 1;
        public int Height => MaxY - MinY + 1;

        public bool IsSwatch =>
            !TouchesEdge && Width >= MinimumSize && Height >= MinimumSize && Area >= MinimumFill * Width * Height;

        public Swatch ToSwatch() => new(MinX, MinY, Width, Height, R, G, B);
    }

    /// <summary>Labels the 4-connected areas of identical color.</summary>
    private static List<Region> FindRegions(byte[] rgba, int width, int height, out int[] labels)
    {
        var regions = new List<Region>();
        var visited = new bool[width * height];
        var regionOf = new int[width * height];
        var stack = new Stack<int>();

        for (var start = 0; start < visited.Length; start++)
        {
            if (visited[start])
            {
                continue;
            }

            var offset = start * 4;
            byte r = rgba[offset], g = rgba[offset + 1], b = rgba[offset + 2];
            var region = new Region { R = r, G = g, B = b };
            var label = regions.Count;
            regions.Add(region);
            visited[start] = true;
            stack.Push(start);
            while (stack.Count > 0)
            {
                var index = stack.Pop();
                var x = index % width;
                var y = index / width;
                regionOf[index] = label;
                region.Area++;
                region.MinX = Math.Min(region.MinX, x);
                region.MaxX = Math.Max(region.MaxX, x);
                region.MinY = Math.Min(region.MinY, y);
                region.MaxY = Math.Max(region.MaxY, y);
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                {
                    region.TouchesEdge = true;
                }

                if (x > 0) Visit(index - 1);
                if (x < width - 1) Visit(index + 1);
                if (y > 0) Visit(index - width);
                if (y < height - 1) Visit(index + width);
            }

            void Visit(int neighbor)
            {
                if (visited[neighbor])
                {
                    return;
                }
                var o = neighbor * 4;
                if (rgba[o] == r && rgba[o + 1] == g && rgba[o + 2] == b)
                {
                    visited[neighbor] = true;
                    stack.Push(neighbor);
                }
            }
        }

        labels = regionOf;
        return regions;
    }
}
