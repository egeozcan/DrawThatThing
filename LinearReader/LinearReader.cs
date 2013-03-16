using System;

namespace LinearReader
{
	using System.Collections.Generic;
	using System.Drawing;
	using System.Runtime.InteropServices;

	using BitmapReader.Classes;
	using BitmapReader.Extensions;
	using BitmapReader.Interface;

	using System.Linq;

	using Point = BitmapReader.Point;

	public class LinearReader : IBitmapReader
	{
		private readonly string _bitmapPath;

		public LinearReader(String path)
		{
			_bitmapPath = path;
		}

		private PixelAcceptanceArgs _defaultArgs = new PixelAcceptanceArgs
			{
				MaxLight = 700,
				BlueEnabled = false,
				BlueMax = 255,
				BlueMin = 0,
				GreenEnabled = false,
				GreenMax = 255,
				GreenMin = 0,
				RedEnabled = false,
				RedMin = 0,
				RedMax = 255
			};

		public IEnumerable<MouseDragAction> GetDrawInstructions(List<ColorSpot> colorPalette, IDictionary<string, string> settings = null)
		{
			if (colorPalette.All(c => c.Color.R + c.Color.G + c.Color.B != 0))
			{
				throw new ArgumentException("I need a black color in the palette!");
			}
			var selectedColor = colorPalette.First(c => c.Color.R + c.Color.G + c.Color.B != 0);
			var args = settings != null ? new PixelAcceptanceArgs
				{
					MaxLight = settings.GetIntValueOrDefault("MaxLight", _defaultArgs.MaxLight),
					BlueEnabled = settings.GetBoolValueOrDefault("BlueEnabled", _defaultArgs.BlueEnabled),
					BlueMax = settings.GetIntValueOrDefault("BlueMax", _defaultArgs.BlueMax),
					BlueMin = settings.GetIntValueOrDefault("BlueMin", _defaultArgs.BlueMin),
					GreenEnabled = settings.GetBoolValueOrDefault("GreenEnabled", _defaultArgs.GreenEnabled),
					GreenMax = settings.GetIntValueOrDefault("GreenMax", _defaultArgs.GreenMax),
					GreenMin = settings.GetIntValueOrDefault("GreenMin", _defaultArgs.GreenMin),
					RedEnabled = settings.GetBoolValueOrDefault("RedEnabled", _defaultArgs.RedEnabled),
					RedMin = settings.GetIntValueOrDefault("RedMin", _defaultArgs.RedMin),
					RedMax = settings.GetIntValueOrDefault("BlueMax", _defaultArgs.RedMax)
				} : _defaultArgs;
			if (String.IsNullOrEmpty(_bitmapPath))
			{
				return null;
			}
			using (var bitmap = new Bitmap(_bitmapPath))
			{
				Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

				System.Drawing.Imaging.BitmapData bmpData =
					bitmap.LockBits(rect,
						System.Drawing.Imaging.ImageLockMode.ReadOnly,
						bitmap.PixelFormat);

				IntPtr ptr = bmpData.Scan0;

				int bytes = bmpData.Stride * bitmap.Height;
				byte[] rgbValues = new byte[bytes];

				Marshal.Copy(ptr, rgbValues, 0, bytes);

				int red;
				int green;
				int blue;
				var output = new List<MouseDragAction>
					{ new MouseDragAction(new List<Point> { selectedColor.Point }, true, selectedColor.Color) };
				// y1 x1 y2 x2
				int[] lc = new[] { 0, 0, 0, 0 };

				for (int x = 0; x < bitmap.Width; x++)
				{
					bool lineInProgress = false;
					for (int y = 0; y < bitmap.Height; y++)
					{
						//See the link above for an explanation 
						//of this calculation (assumes 24bppRgb format)
						int position = (y * bmpData.Stride) + (x * 3);
						blue = rgbValues[position];
						green = rgbValues[position + 1];
						red = rgbValues[position + 2];
						if (pixelFits(red, green, blue, args))
						{
							if (lineInProgress)
							{
								lc[2] = y;
								lc[3] = x;
							}
							else
							{
								lineInProgress = true;
								lc[0] = lc[2] = y;
								lc[1] = lc[3] = x;
							}
						}
						else if (lineInProgress)
						{
							noteCoordinates(lc, output);
							lineInProgress = false;
						}
					}
					if (lineInProgress)
					{
						noteCoordinates(lc, output);
					}
				}
				bitmap.UnlockBits(bmpData);
				return output.ToArray();
			}
		}

		private static void noteCoordinates(int[] lc, List<MouseDragAction> output)
		{
			MouseDragAction action;
			if (lc[1] != lc[3] || lc[0] != lc[2])
			{
				action = new MouseDragAction(new List<Point> { new Point(lc[0], lc[1]), new Point(lc[2], lc[3]) });
			}
			else
			{
				action = new MouseDragAction(new List<Point> { new Point(lc[0], lc[1]) });
			}
			output.Add(action);
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
