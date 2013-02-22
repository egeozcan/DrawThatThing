namespace LowLevelTools
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.Linq;
	using System.Runtime.InteropServices;

	public class ColoredBitmapReader
	{
		private readonly string _bitmapPath;
		private readonly Dictionary<Point, PixelMeta> _pixels = new Dictionary<Point, PixelMeta>();
		private readonly Random _random = new Random();

		public IEnumerable<MouseAction> getDrawInstructions(PixelAcceptanceArgs args, ColorSpot[] color)
		{
			List<MouseAction> output = new List<MouseAction>();

			using (var bitmap = new Bitmap(_bitmapPath))
			{
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
						var currentColor = Color.FromArgb(red, green, blue);
						_pixels.Add
							(
								new Point(x, y),
								new PixelMeta
									{
										color = currentColor,
										expired = false,
										closestColor = color
												.OrderBy(c => c.color.DifferenceTo(currentColor))
												.FirstOrDefault()
									}
							);
					}
				}
				bitmap.UnlockBits(bmpData);
			}

			foreach (var colorSpot in color)
			{
				output.Add(new MouseAction().addClick(colorSpot.point.X, colorSpot.point.Y));
				var filteredPixels = color.Where(x => x.color.Equals(colorSpot));
				foreach (var pixel in filteredPixels)
				{
					output.Add(new MouseAction().addClick(pixel.point.X, pixel.point.Y));
				}
			}
			

			return output;
		}

		public ColoredBitmapReader(string path)
		{
			_bitmapPath = path;
		}

	}
}