namespace LowLevelTools
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.Runtime.InteropServices;

	public class ColoredBitmapReader
	{
		private string _bitmapPath;

		public IEnumerable<MouseAction> getDrawInstructions(PixelAcceptanceArgs args, ColorSpot[] color)
		{
			PixelMeta[][] pixelMatrix;
			List<MouseAction> output = new List<MouseAction>();

			using (var bitmap = new Bitmap(_bitmapPath))
			{
				int width = bitmap.Width;
				int height = bitmap.Height;
				pixelMatrix = new PixelMeta[width][];
				for (int i = 0; i < width; i++)
				{
					pixelMatrix[i] = new PixelMeta[height];
				}

				Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
				BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
				IntPtr ptr = bmpData.Scan0;
				int bytes = bmpData.Stride * bitmap.Height;
				byte[] rgbValues = new byte[bytes];
				Marshal.Copy(ptr, rgbValues, 0, bytes);
				for (int x = 0; x < bitmap.Width; x++)
				{
					for (int y = 0; y < bitmap.Height; y++)
					{
						int position = (y * bmpData.Stride) + (x * 3);
						int blue = rgbValues[position];
						int green = rgbValues[position + 1];
						int red = rgbValues[position + 2];
						pixelMatrix[x][y] = new PixelMeta
							{
								color = Color.FromArgb(red, green, blue),
								expired = false
							};
					}
				}
				bitmap.UnlockBits(bmpData);
			}



			return output;
		}

		public ColoredBitmapReader(string path)
		{
			_bitmapPath = path;
		}

	}
}