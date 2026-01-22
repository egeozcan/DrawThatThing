namespace DrawThatThing.Core.Models;

public class Brush
{
    public Color Color { get; set; }
    public BrushSize Size { get; set; } = new();
}

public class BrushSize
{
    public int Width { get; set; }
    public int Height { get; set; }

    public BrushSize()
    {
    }

    public BrushSize(int width, int height)
    {
        Width = width;
        Height = height;
    }
}

public enum BrushShape
{
    Round,
    Square
}
