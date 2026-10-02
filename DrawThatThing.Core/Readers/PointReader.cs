using DrawThatThing.Core.Attributes;
using DrawThatThing.Core.Extensions;
using DrawThatThing.Core.Imaging;
using DrawThatThing.Core.Interfaces;
using DrawThatThing.Core.Models;

namespace DrawThatThing.Core.Readers;

/// <summary>
/// Clicks every pixel individually, grouped by the closest palette color. White is skipped.
/// </summary>
[DefaultSetting("MixPoints", false)]
public class PointReader : IBitmapReader
{
    private readonly string _bitmapPath;

    public PointReader(string path)
    {
        _bitmapPath = path;
    }

    public IEnumerable<MouseDragAction> GetDrawInstructions(List<ColorSpot> colorPalette, IDictionary<string, string>? options = null, IBrushChanger? brushChanger = null)
    {
        if (colorPalette.Count == 0)
        {
            throw new ArgumentException("I need some colors. Give me some colors. Thanks!");
        }
        var output = new List<MouseDragAction>();
        var colorPixels = colorPalette.ToDictionary(colorSpot => colorSpot, _ => new List<Point>());
        var bitmap = PixelBitmap.Load(_bitmapPath);
        bitmap.LoopThroughPixels((point, color) =>
        {
            var selectedColor = colorPalette.OrderBy(c => c.Color.DifferenceTo(color)).FirstOrDefault();
            if (selectedColor == null || !colorPixels.ContainsKey(selectedColor) ||
                selectedColor.Color.DifferenceTo(Color.White) == 0)
            {
                return;
            }
            colorPixels[selectedColor].Add(point);
        });
        foreach (var colorPixel in colorPixels.Where(colorPixel => colorPixel.Value.Count > 0))
        {
            output.Add(new MouseDragAction([colorPixel.Key.Point], true, colorPixel.Key.Color));
            IList<MouseDragAction> actions = colorPixel.Value.Select(point => new MouseDragAction([point])).ToList();
            if (options.GetBoolOrDefault("MixPoints", false))
            {
                actions = actions.Shuffle();
            }
            output.AddRange(actions);
        }
        return output;
    }
}
