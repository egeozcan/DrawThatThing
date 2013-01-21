using System;
using System.Text;
using System.Drawing;

namespace LowLevelTools
{
	public class BitmapReader
	{
		private string _bitmapPath;

		public void Read(String path)
		{
			_bitmapPath = path;
		}

		public struct PixelAcceptanceArgs
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

		public string getNonWhite(PixelAcceptanceArgs args)
		{
			if (String.IsNullOrEmpty(_bitmapPath))
			{
				return String.Empty;
			}
			using (var _bitmap = new Bitmap(_bitmapPath))
			{
				Rectangle rect = new Rectangle(0, 0, _bitmap.Width, _bitmap.Height);

				System.Drawing.Imaging.BitmapData bmpData =
					_bitmap.LockBits(rect,
						System.Drawing.Imaging.ImageLockMode.ReadOnly,
						_bitmap.PixelFormat);

				IntPtr ptr = bmpData.Scan0;

				int bytes = bmpData.Stride * _bitmap.Height;
				byte[] rgbValues = new byte[bytes];

				System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

				int red;
				int green;
				int blue;
				var output = new StringBuilder();
				// y1 x1 y2 x2
				int[] lc = new[] { 0, 0, 0, 0 };

				for (int x = 0; x < _bitmap.Width; x++)
				{
					bool lineInProgress = false;
					for (int y = 0; y < _bitmap.Height; y++)
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
				_bitmap.UnlockBits(bmpData);
				return output.ToString();
			}
		}

		private static void noteCoordinates(int[] lc, StringBuilder output)
		{
			if (lc[1] != lc[3] || lc[0] != lc[2])
			{
				output.AppendLine(String.Format("{0} {1} {2} {3}", lc[0], lc[1], lc[2], lc[3]));
			}
			else
			{
				output.AppendLine(String.Format("{0} {1}", lc[0], lc[1]));
			}
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
