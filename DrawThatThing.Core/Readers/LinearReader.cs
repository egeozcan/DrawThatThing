using DrawThatThing.Core.Attributes;
using DrawThatThing.Core.Extensions;
using DrawThatThing.Core.Imaging;
using DrawThatThing.Core.Interfaces;
using DrawThatThing.Core.Models;

namespace DrawThatThing.Core.Readers;

/// <summary>
/// Draws every "dark enough" pixel with the black palette color, column by column.
/// </summary>
[DefaultSetting("MaxLight", 700)]
[DefaultSetting("BlueEnabled", false)]
[DefaultSetting("BlueMax", 255)]
[DefaultSetting("BlueMin", 0)]
[DefaultSetting("GreenEnabled", false)]
[DefaultSetting("GreenMax", 255)]
[DefaultSetting("GreenMin", 0)]
[DefaultSetting("RedEnabled", false)]
[DefaultSetting("RedMax", 255)]
[DefaultSetting("RedMin", 0)]
public class LinearReader : IBitmapReader
{
    private readonly string _bitmapPath;

    public LinearReader(string path)
    {
        _bitmapPath = path;
    }

    public IEnumerable<MouseDragAction> GetDrawInstructions(List<ColorSpot> colorPalette, IDictionary<string, string>? settings = null, IBrushChanger? brushChanger = null)
    {
        if (colorPalette.All(c => c.Color.R + c.Color.G + c.Color.B != 0))
        {
            throw new ArgumentException("I need a black color in the palette!");
        }
        var selectedColor = colorPalette.First(c => c.Color.R + c.Color.G + c.Color.B == 0);
        settings ??= new Dictionary<string, string>();
        var args = new PixelAcceptanceArgs
        {
            MaxLight = settings.GetIntOrDefault("MaxLight", 700),
            BlueEnabled = settings.GetBoolOrDefault("BlueEnabled", false),
            BlueMax = settings.GetIntOrDefault("BlueMax", 255),
            BlueMin = settings.GetIntOrDefault("BlueMin", 0),
            GreenEnabled = settings.GetBoolOrDefault("GreenEnabled", false),
            GreenMax = settings.GetIntOrDefault("GreenMax", 255),
            GreenMin = settings.GetIntOrDefault("GreenMin", 0),
            RedEnabled = settings.GetBoolOrDefault("RedEnabled", false),
            RedMin = settings.GetIntOrDefault("RedMin", 0),
            RedMax = settings.GetIntOrDefault("RedMax", 255)
        };
        if (string.IsNullOrEmpty(_bitmapPath))
        {
            return [];
        }
        var output = new List<MouseDragAction>
        {
            new([selectedColor.Point], true, selectedColor.Color)
        };
        var bitmap = PixelBitmap.Load(_bitmapPath);
        var currentAction = new List<Point>();
        bitmap.LoopThroughPixels((point, color) =>
        {
            if (!PixelFits(color.R, color.G, color.B, args))
            {
                return;
            }
            if (currentAction.Count > 0 && !currentAction[^1].IsANeighborOf(point))
            {
                output.Add(new MouseDragAction(currentAction));
                currentAction = [];
            }
            currentAction.Add(point);
        });
        if (currentAction.Count > 0)
        {
            output.Add(new MouseDragAction(currentAction));
        }
        return output;
    }

    private static bool PixelFits(int red, int green, int blue, PixelAcceptanceArgs args)
    {
        if (args.RedEnabled && (red < args.RedMin || red > args.RedMax))
        {
            return false;
        }
        if (args.GreenEnabled && (green < args.GreenMin || green > args.GreenMax))
        {
            return false;
        }
        if (args.BlueEnabled && (blue < args.BlueMin || blue > args.BlueMax))
        {
            return false;
        }
        return red + green + blue <= args.MaxLight;
    }

    private sealed class PixelAcceptanceArgs
    {
        public int RedMin;
        public int RedMax;
        public int BlueMin;
        public int BlueMax;
        public int GreenMin;
        public int GreenMax;
        public bool RedEnabled;
        public bool GreenEnabled;
        public bool BlueEnabled;
        public int MaxLight;
    }
}
