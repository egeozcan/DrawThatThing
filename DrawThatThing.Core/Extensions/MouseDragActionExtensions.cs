using DrawThatThing.Core.Models;

namespace DrawThatThing.Core.Extensions;

public static class MouseDragActionExtensions
{
    /// <summary>
    /// Clicks the opener of a palette color before every click that chooses that color. Done after parsing,
    /// so every parser, plugins included, works with palettes that first have to be opened.
    /// </summary>
    public static IEnumerable<MouseDragAction> WithPaletteOpeners(this IEnumerable<MouseDragAction> actions, IReadOnlyList<ColorSpot> palette)
    {
        foreach (var action in actions)
        {
            // The parsers choose the first palette color at that position, so its opener is the one to click.
            var colorSpot = action.DiscardOffset && action.Points.Count > 0
                ? palette.FirstOrDefault(spot => !spot.Point.IsEmpty
                                                 && spot.Point.Equals(action.Points[0])
                                                 && spot.Color.DifferenceTo(action.Color) == 0)
                : null;
            if (colorSpot != null && !colorSpot.Opener.IsEmpty)
            {
                yield return new MouseDragAction([colorSpot.Opener], true);
            }
            yield return action;
        }
    }
}
