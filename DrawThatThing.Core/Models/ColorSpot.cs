namespace DrawThatThing.Core.Models;

public class ColorSpot
{
    public Color Color { get; set; }
    public Point Point { get; set; } = new();
    public bool IsBackgroundColor { get; set; }

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
