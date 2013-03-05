namespace LowLevelTools
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Threading.Tasks;

	using MiniProfiler.Windows;

	using StackExchange.Profiling;

	public class ColoredBitmapReader
	{
		private readonly string _bitmapPath;
		private ColorSpot[,] _colors;
		private bool[,] _drawStatus;

		public IEnumerable<MouseDragAction> getDrawInstructions(ColorSpot[] PaletteColorSpots)
		{
			if (!PaletteColorSpots.Any())
			{
				return new List<MouseDragAction>();
			}

			int bitmapHeight;
			int bitmapWidth;

			List<MouseDragAction> output = new List<MouseDragAction>();

			using (var bitmap = new Bitmap(_bitmapPath))
			{
				bitmapHeight = bitmap.Height;
				bitmapWidth = bitmap.Width;

				_colors = new ColorSpot[bitmapWidth,bitmapHeight];
				_drawStatus = new bool[bitmapWidth,bitmapHeight];

				Rectangle rect = new Rectangle(0, 0, bitmapWidth, bitmapHeight);
				BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
				IntPtr ptr = bmpData.Scan0;
				int bytes = bmpData.Stride * bitmapHeight;
				byte[] rgbValues = new byte[bytes];
				Marshal.Copy(ptr, rgbValues, 0, bytes);
				for (int x = 0; x < bitmapWidth; x++)
				{
					for (int y = 0; y < bitmapHeight; y++)
					{
						int position = (y * bmpData.Stride) + (x * 3);
						int blue = rgbValues[position];
						int green = rgbValues[position + 1];
						int red = rgbValues[position + 2];
						var currentColor = Color.FromArgb(red, green, blue);
						var matchingColor = PaletteColorSpots.OrderBy(c => c.Color.DifferenceTo(currentColor)).First();
						matchingColor.Count++;
						_colors[x, y] = matchingColor;
					}
				}
				bitmap.UnlockBits(bmpData);
			}

			Parallel.ForEach(PaletteColorSpots, delegate(ColorSpot spot)
			{
				
			});

			const string ignoreColor = "ffffffff";
			ConsoleProfiling.Start();
			var groupedPixels = _pixels.AsParallel().Where(x => !x.Key.IsEmpty).GroupBy(x => x.Value.closestColor);
			Parallel.ForEach(
				groupedPixels,
				delegate(IGrouping<ColorSpot, KeyValuePair<Point, PixelMeta>> pixelGroup)
					{
						if (pixelGroup.Key.Color.Name == ignoreColor)
						{
							return;
						}
						List<MouseDragAction> colorOutput = new List<MouseDragAction>();
						//Change PaletteColorSpots

						var colorChangeAction = new MouseDragAction(
							new[] { new Point(pixelGroup.Key.Point.X, pixelGroup.Key.Point.Y) }, true) { Color = pixelGroup.Key.Color };
						colorOutput.Add(colorChangeAction);
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
								KeyValuePair<Point, PixelMeta> closestPixel;
								//closestPixel = pixelGroup.FirstOrDefault(x => !x.Value.expired && x.Key.IsANeighborOf(Point));
								//closestPixel = Point.GetNeighboringPoints(bitmapWidth, bitmapHeight).Where(p => p.X >= 0 && p.Y >= 0 && p.X < bitmapWidth && p.Y < bitmapHeight).Select(p => _pixels[(p.X * bitmapHeight) + p.Y]).FirstOrDefault();
								if (closestPixel.Key.IsEmpty)
								{
									break;
								}
								currentPoint = closestPixel.Key;
								points.Add(closestPixel.Key);
								closestPixel.Value.expired = true;
							}
							var ma = new MouseDragAction(points.ToArray());
							colorOutput.Add(ma);
						}
						lock (output)
						{
							output.AddRange(colorOutput);
						}
					});
			var friendlyString = ConsoleProfiling.StopAndGetConsoleFriendlyOutputStringWithSqlTimings();
			Console.WriteLine(friendlyString);
			Debug.WriteLine(friendlyString);
			return output;
		}

		public ColoredBitmapReader(string path)
		{
			_bitmapPath = path;
		}
	}
}