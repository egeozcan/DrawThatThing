using System;

namespace LinearReader
{
	using System.Collections.Generic;
	using System.Drawing;

	using BitmapReader.Classes;
	using BitmapReader.Extensions;
	using BitmapReader.Interface;
	using BitmapReader.Attributes;

	using System.Linq;

	using Point = BitmapReader.Classes.Point;

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

		public LinearReader(String path)
		{
			_bitmapPath = path;
		}

		public IEnumerable<MouseDragAction> GetDrawInstructions(List<ColorSpot> colorPalette, IDictionary<string, string> settings = null)
		{
			if (colorPalette.All(c => c.Color.R + c.Color.G + c.Color.B != 0))
			{
				throw new ArgumentException("I need a black color in the palette!");
			}
			var selectedColor = colorPalette.First(c => c.Color.R + c.Color.G + c.Color.B == 0);
			if (settings == null)
			{
				settings = new Dictionary<string, string>();
			}
			var args = new PixelAcceptanceArgs
				{
					MaxLight = settings.GetIntValueOrDefault("MaxLight", 700),
					BlueEnabled = settings.GetBoolValueOrDefault("BlueEnabled", false),
					BlueMax = settings.GetIntValueOrDefault("BlueMax", 255),
					BlueMin = settings.GetIntValueOrDefault("BlueMin", 0),
					GreenEnabled = settings.GetBoolValueOrDefault("GreenEnabled", false),
					GreenMax = settings.GetIntValueOrDefault("GreenMax", 255),
					GreenMin = settings.GetIntValueOrDefault("GreenMin", 0),
					RedEnabled = settings.GetBoolValueOrDefault("RedEnabled", false),
					RedMin = settings.GetIntValueOrDefault("RedMin", 0),
					RedMax = settings.GetIntValueOrDefault("RedMax", 255)
				};
			if (String.IsNullOrEmpty(_bitmapPath))
			{
				return null;
			}
			var output = new List<MouseDragAction>
				{
					new MouseDragAction(new List<Point> { selectedColor.Point }, true, selectedColor.Color)
				};
			using (var bitmap = new Bitmap(_bitmapPath))
			{
				var currentAction = new List<Point>();
				bitmap.LoopThroughPixels((point, color) =>
				{
					if (!pixelFits(color.R, color.G, color.B, args))
					{
						return;
					}
					if (currentAction.Any() && !currentAction.Last().IsANeighborOf(point))
					{
						output.Add(new MouseDragAction(currentAction));
						currentAction = new List<Point>();
					}
					currentAction.Add(point);
				});
			}
			return output.ToArray();
		}

		private static bool pixelFits(int red, int green, int blue, PixelAcceptanceArgs args)
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

	}
}
