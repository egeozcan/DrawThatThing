using DrawThatThing.Core.Models;

namespace DrawThatThing.Core.Extensions;

public static class ColorExtensions
{
    public static int DifferenceTo(this Color color, Color toColor)
    {
        return Math.Abs(color.R - toColor.R) + Math.Abs(color.G - toColor.G) + Math.Abs(color.B - toColor.B);
    }

    public static string ToHex(this Color c)
    {
        return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
    }
}
