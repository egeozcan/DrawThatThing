namespace DrawThatThing.Core.Models;

public class ColorSpot
{
    public Color Color { get; set; }
    public Point Point { get; set; } = new();
    public bool IsBackgroundColor { get; set; }

    /// <summary>
    /// Where to click before choosing this color, for programs that hide their palette behind a button
    /// that opens it. Empty when the color can be clicked directly.
    /// </summary>
    public Point Opener { get; set; } = Point.Empty;

    public ColorSpot()
    {
    }

    public ColorSpot(Color color, Point point, bool isBackgroundColor = false)
    {
        Color = color;
        Point = point;
        IsBackgroundColor = isBackgroundColor;
    }
}
