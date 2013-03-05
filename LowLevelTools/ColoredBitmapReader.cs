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
		private KeyValuePair<Point, PixelMeta>[] _pixels;
		private ColorSpot[,] _colors;
		private bool[,] _drawStatus;

		public IEnumerable<MouseDragAction> getDrawInstructions(ColorSpot[] color)
		{
			int bitmapHeight;
			int bitmapWidth;

			List<MouseDragAction> output = new List<MouseDragAction>();

			using (var bitmap = new Bitmap(_bitmapPath))
			{
				bitmapHeight = bitmap.Height;
				bitmapWidth = bitmap.Width;

				_colors = new ColorSpot[bitmapWidth,bitmapHeight];
				_drawStatus = new bool[bitmapWidth,bitmapHeight];

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
						_colors[x, y] = color.OrderBy(c => c.color.DifferenceTo(currentColor)).FirstOrDefault();
						
						/*_pixels[(x * bitmap.Height) + y] = new KeyValuePair<Point, PixelMeta>
							(
								new Point(x, y),
								new PixelMeta
									{
										expired = false,
										closestColor = color
												.OrderBy(c => c.color.DifferenceTo(currentColor))
												.FirstOrDefault()
									}
							);*/
					}
				}
				bitmap.UnlockBits(bmpData);
			}
			const string ignoreColor = "ffffffff";
			ConsoleProfiling.Start();
			

			var groupedPixels = _pixels.AsParallel().Where(x => !x.Key.IsEmpty).GroupBy(x => x.Value.closestColor);
			using (MiniProfiler.Current.Step("parsing image"))
			{
				Parallel.ForEach(
					groupedPixels,
					delegate(IGrouping<ColorSpot, KeyValuePair<Point, PixelMeta>> pixelGroup)
						{
							using (MiniProfiler.Current.Step("delegate"))
							{
								if (pixelGroup.Key.color.Name == ignoreColor)
								{
									return;
								}
								List<MouseDragAction> colorOutput = new List<MouseDragAction>();
								//Change color
								using (MiniProfiler.Current.Step("creating new mousedragaction"))
								{
									var colorChangeAction = new MouseDragAction(
										new[] { new Point(pixelGroup.Key.point.X, pixelGroup.Key.point.Y) }, true) { Color = pixelGroup.Key.color };
									colorOutput.Add(colorChangeAction);
								}
								while (pixelGroup.Any(x => !x.Value.expired))
								{
									using (MiniProfiler.Current.Step("looping over pixelgroup"))
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
											//using (MiniProfiler.Current.Step("finding closest pixel"))
											//{
												//closestPixel = pixelGroup.FirstOrDefault(x => !x.Value.expired && x.Key.IsANeighborOf(point));
												//closestPixel = point.GetNeighboringPoints(bitmapWidth, bitmapHeight).Where(p => p.X >= 0 && p.Y >= 0 && p.X < bitmapWidth && p.Y < bitmapHeight).Select(p => _pixels[(p.X * bitmapHeight) + p.Y]).FirstOrDefault();
											//}
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
								}
								lock (output)
								{
									output.AddRange(colorOutput);
								}
							}
						});
			}
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