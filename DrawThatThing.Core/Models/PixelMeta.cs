namespace DrawThatThing.Core.Models;

public class PixelMeta
{
    public Point Position { get; set; } = new();
    public Color Color { get; set; }
    public bool IsProcessed { get; set; }
}
