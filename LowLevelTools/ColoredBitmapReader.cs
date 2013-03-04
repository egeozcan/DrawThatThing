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
		private KeyValuePair<Point, PixelMeta>[] _pixels;

		public IEnumerable<MouseAction> getDrawInstructions(ColorSpot[] color)
		{
			List<MouseAction> output = new List<MouseAction>();

			using (var bitmap = new Bitmap(_bitmapPath))
			{
				Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
				_pixels = new KeyValuePair<Point, PixelMeta>[bitmap.Width * bitmap.Height];
				BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
				IntPtr ptr = bmpData.Scan0;
				int bytes = bmpData.Stride * bitmap.Height;
				byte[] rgbValues = new byte[bytes];
				Marshal.Copy(ptr, rgbValues, 0, bytes);
				int count = 0;
				for (int x = 0; x < bitmap.Width; x++)
				{
					for (int y = 0; y < bitmap.Height; y++)
					{
						int position = (y * bmpData.Stride) + (x * 3);
						int blue = rgbValues[position];
						int green = rgbValues[position + 1];
						int red = rgbValues[position + 2];
						if (red + green + blue > 750)
						{
							continue;
						}
						var currentColor = Color.FromArgb(red, green, blue);
						_pixels[(x * bitmap.Height) + y] = new KeyValuePair<Point, PixelMeta>
							(
								new Point(x, y),
								new PixelMeta
									{
										expired = false,
										closestColor = color
												.OrderBy(c => c.color.DifferenceTo(currentColor))
												.FirstOrDefault()
									}
							);
						count++;
					}
				}
				bitmap.UnlockBits(bmpData);
			}

			var groupedPixels = _pixels.Where(x => !x.Key.IsEmpty).GroupBy(x => x.Value.closestColor);
			foreach (var pixelGroup in groupedPixels)
			{
				//Change color
				output.Add(new MouseAction().addClick(pixelGroup.Key.point.X, pixelGroup.Key.point.Y));
				while (pixelGroup.Any(x => !x.Value.expired))
				{
					var points = new List<Point>();
					KeyValuePair<Point, PixelMeta> currentPair = pixelGroup.First(x => !x.Value.expired);
					Point currentPoint = currentPair.Key;
					currentPair.Value.expired = true;
					points.Add(currentPoint);
					while (true)
					{
						Point point = currentPoint;
						KeyValuePair<Point, PixelMeta> closestPixel = pixelGroup.AsParallel().FirstOrDefault(x => x.Key.IsANeighborOf(point) && !x.Value.expired);
						if (closestPixel.Key.IsEmpty)
						{
							break;
						}
						currentPoint = closestPixel.Key;
						points.Add(closestPixel.Key);
						closestPixel.Value.expired = true;
					}
					var ma = new MouseAction();
					ma.addPath(points);
					output.Add(ma);
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