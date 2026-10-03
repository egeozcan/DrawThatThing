namespace DrawThatThing.Core.Imaging;

/// <summary>Helpers for palettes laid out as an evenly spaced grid of swatches.</summary>
public static class PaletteGrid
{
    /// <summary>
    /// The centers of the cells of a grid whose first and last cell centers are given, row by row from the
    /// top-left. The corners may be given in any order.
    /// </summary>
    public static IReadOnlyList<(int X, int Y)> CellCenters(int x1, int y1, int x2, int y2, int columns, int rows)
    {
        if (columns < 1 || rows < 1)
        {
            return [];
        }

        int left = Math.Min(x1, x2), right = Math.Max(x1, x2);
        int top = Math.Min(y1, y2), bottom = Math.Max(y1, y2);
        var centers = new List<(int, int)>(columns * rows);
        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                centers.Add((Interpolate(left, right, column, columns), Interpolate(top, bottom, row, rows)));
            }
        }
        return centers;
    }

    private static int Interpolate(int from, int to, int index, int count) =>
        count == 1 ? from : from + (int)Math.Round((double)(to - from) * index / (count - 1));

    /// <summary>
    /// The most common color in the square around a point, so a stray border or anti-aliased pixel
    /// near the center of a swatch does not decide its color. Coordinates outside the image are skipped.
    /// </summary>
    public static (byte R, byte G, byte B) DominantColor(byte[] rgba, int width, int height, int centerX, int centerY, int radius)
    {
        var counts = new Dictionary<int, int>();
        var best = 0;
        var bestCount = 0;
        for (var y = Math.Max(0, centerY - radius); y <= Math.Min(height - 1, centerY + radius); y++)
        {
            for (var x = Math.Max(0, centerX - radius); x <= Math.Min(width - 1, centerX + radius); x++)
            {
                var o = (y * width + x) * 4;
                var key = rgba[o] << 16 | rgba[o + 1] << 8 | rgba[o + 2];
                var count = counts.GetValueOrDefault(key) + 1;
                counts[key] = count;
                // Ties go to the first color seen, which is the one nearest the top-left; stable and good enough.
                if (count > bestCount)
                {
                    best = key;
                    bestCount = count;
                }
            }
        }
        return ((byte)(best >> 16), (byte)(best >> 8), (byte)best);
    }
}
