using DrawThatThing.Core.Models;

namespace DrawThatThing.Core.Extensions;

public static class StringExtensions
{
    public static int ToInt(this string str, int defaultValue = 0)
    {
        return int.TryParse(str, out int result) ? result : defaultValue;
    }

    public static bool ToBool(this string str, bool defaultValue = false)
    {
        return str.ToLowerInvariant().EqualsToAny("true", "1");
    }

    public static bool EqualsToAny(this string str, params string[] values)
    {
        return values.Any(x => x == str);
    }

    public static Color ToColor(this string str)
    {
        try
        {
            return Color.FromHex(str);
        }
        catch
        {
            return Color.Empty;
        }
    }
}
