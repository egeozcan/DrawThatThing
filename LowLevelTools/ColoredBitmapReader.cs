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

		public IEnumerable<MouseDragAction> getDrawInstructions(ColorSpot[] color)
		{
			int bitmapHeight;
			int bitmapWidth;

			List<MouseDragAction> output = new List<MouseDragAction>();

			using (var bitmap = new Bitmap(_bitmapPath))
			{
				bitmapHeight = bitmap.Height;
				bitmapWidth = bitmap.Width;
				Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
				_pixels = new KeyValuePair<Point, PixelMeta>[bitmap.Width * bitmap.Height];
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
					}
				}
				bitmap.UnlockBits(bmpData);
			}

			var groupedPixels = _pixels.Where(x => !x.Key.IsEmpty).GroupBy(x => x.Value.closestColor);
			foreach (var pixelGroup in groupedPixels.Where(pixelGroup => pixelGroup.Key.color != Color.White))
			{
				//Change color
				var colorChangeAction = new MouseDragAction(
					new[] { new Point(pixelGroup.Key.point.X, pixelGroup.Key.point.Y) }, true) { Color = pixelGroup.Key.color };
				output.Add(colorChangeAction);
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
						KeyValuePair<Point, PixelMeta> closestPixel =
							pixelGroup.AsParallel().FirstOrDefault(x => !x.Value.expired && x.Key.IsANeighborOf(point));
						if (closestPixel.Key.IsEmpty)
						{
							break;
						}
						currentPoint = closestPixel.Key;
						points.Add(closestPixel.Key);
						closestPixel.Value.expired = true;
					}
					var ma = new MouseDragAction(points.ToArray());
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