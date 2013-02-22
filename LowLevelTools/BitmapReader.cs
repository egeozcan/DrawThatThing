using System;
using System.Drawing;

namespace LowLevelTools
{
	using System.Collections.Generic;
	using System.Runtime.InteropServices;


	public class BitmapReader
	{
		private string _bitmapPath;

		public void Read(String path)
		{
			_bitmapPath = path;
		}

		public MouseAction[] getFilteredPixels(PixelAcceptanceArgs args)
		{
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

				var output = new List<MouseAction>();
				// y1 x1 y2 x2
				int[] lc = new[] { 0, 0, 0, 0 };

				for (int x = 0; x < bitmap.Width; x++)
				{
					bool lineInProgress = false;
					for (int y = 0; y < bitmap.Height; y++)
					{
						int position = (y * bmpData.Stride) + (x * 3);
						int blue = rgbValues[position];
						int green = rgbValues[position + 1];
						int red = rgbValues[position + 2];
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

		private static void noteCoordinates(int[] lc, List<MouseAction> output)
		{
			var action = new MouseAction();
			if (lc[1] != lc[3] || lc[0] != lc[2])
			{
				action.addDrag(lc[0], lc[1], lc[2], lc[3]);
			}
			else
			{
				action.addClick(lc[0], lc[1]);
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
