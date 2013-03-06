using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers.BitmapReaders
{
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.Runtime.InteropServices;

	using Classes;

	using Extensions;

	using Interface;

	using Point = Point;

	internal class AlternativeColoredBitmapReader : IColoredBitmapReader
	{
		private readonly string _bitmapPath;
		private int[,] _colors;
		private const string IgnoreColor = "ffffffff";

		public IEnumerable<MouseDragAction> getDrawInstructions(List<ColorSpot> PaletteColorSpots)
		{
			int ignoreColorIndex = PaletteColorSpots.IndexOf(PaletteColorSpots.FirstOrDefault(x => x.Color.Name == IgnoreColor));

			if (!PaletteColorSpots.Any())
			{
				PaletteColorSpots = new List<ColorSpot> { new ColorSpot { Color = Color.Black, Point = Point.Empty } };
			}

			int bitmapHeight;
			int bitmapWidth;

			List<MouseDragAction> output = new List<MouseDragAction>();

			using (var bitmap = new Bitmap(_bitmapPath))
			{
				bitmapHeight = bitmap.Height;
				bitmapWidth = bitmap.Width;

				_colors = new int[bitmapWidth,bitmapHeight];

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
						_colors[x, y] = PaletteColorSpots.IndexOf(matchingColor);
					}
				}
				bitmap.UnlockBits(bmpData);
			}

			var colorCount = PaletteColorSpots.Count;
			List<MouseDragAction>[] strokesForEachColor = new List<MouseDragAction>[colorCount];

			for (int i = 0; i < colorCount; i++)
			{
				var paletteColorSpot = PaletteColorSpots[i];
				strokesForEachColor[i] = new List<MouseDragAction>
					{ new MouseDragAction(new List<Point> { paletteColorSpot.Point }, true, paletteColorSpot.Color) };
			}

			for (int x = 0; x < bitmapWidth; x++)
			{
				for (int y = 0; y < bitmapHeight; y++)
				{
					var currentPixelColorIndex = _colors[x, y];
					if (currentPixelColorIndex == ignoreColorIndex)
					{
						continue;
					}
					var currentPixelPoint = new Point(x, y);
					var strokesForCurrentColor = strokesForEachColor[currentPixelColorIndex];
					bool found = false;

					foreach (var stroke in strokesForCurrentColor)
					{
						if (stroke.DiscardOffset)
						{
							continue;
						}
						var lastPoint = stroke.Points[stroke.Points.Count - 1];
						if (PointsAreNeighbors(lastPoint.X, lastPoint.Y, x, y))
						{
							stroke.PushPoint(new Point(x, y));
							found = true;
							break;
						}
						var firstPoint = stroke.Points[0];
						if (PointsAreNeighbors(firstPoint.X, firstPoint.Y, x, y))
						{
							stroke.AddPoint(new Point(x, y));
							found = true;
							break;
						}
					}
					if (found)
					{
						continue;
					}

					strokesForEachColor[currentPixelColorIndex].Add(new MouseDragAction(new List<Point> { currentPixelPoint }));
				}
			}

			foreach (var mouseDragActions in strokesForEachColor)
			{
				output.AddRange(mouseDragActions);
			}

			return output;
		}

		private static bool PointsAreNeighbors(int x1, int y1, int x2, int y2)
		{
			return Math.Abs(x1 - x2) <= 1 && Math.Abs(y1 - y2) <= 1;
		}

		public AlternativeColoredBitmapReader(string path)
		{
			_bitmapPath = path;
		}
	}
}