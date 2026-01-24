using DrawThatThing.Core.Models;

namespace DrawThatThing.Core.Interfaces;

public interface IBitmapReader
{
    IEnumerable<MouseDragAction> GetDrawInstructions(
        List<ColorSpot> colorPalette,
        IDictionary<string, string>? options = null,
        IBrushChanger? brushChanger = null);
}
