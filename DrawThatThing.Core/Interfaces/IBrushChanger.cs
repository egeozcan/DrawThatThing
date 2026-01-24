using DrawThatThing.Core.Models;

namespace DrawThatThing.Core.Interfaces;

public interface IBrushChanger
{
    MouseDragAction? ChangeBrush(ColorSpot targetBrush);
}
