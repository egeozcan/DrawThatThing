using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using BitmapReader.Classes;
using BitmapReader.Extensions;
using BitmapReader.Interface;
using Point = BitmapReader.Point;

namespace AbstractReader
{
	public class AbstractReader : IBitmapReader
	{
		private readonly string _bitmapPath;
		private int _bitmapHeight;
		private int _bitmapWidth;
		private int[,] _colors;
		private bool[,] _colorsProcessed;

		public AbstractReader(string path)
		{
			this._bitmapPath = path;
		}

		public IEnumerable<MouseDragAction> GetDrawInstructions(List<ColorSpot> color, IDictionary<string, string> settings = null)
		{
			List<ColorSpot> knownColors = color.ToList();
			int ignoredColor = knownColors.IndexOf(knownColors.First(x => x.Color.Name == "ffffffff"));
			// prevent drawing of single pixels etc
			const int colorGroupMinSize = 15;

			var output = new List<MouseDragAction>();

			using (var bitmap = new Bitmap(this._bitmapPath))
			{
				this._bitmapHeight = bitmap.Height;
				this._bitmapWidth = bitmap.Width;
				var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
				this._colors = new int[bitmap.Width,bitmap.Height];
				this._colorsProcessed = new bool[bitmap.Width,bitmap.Height];
				//_pixels = new KeyValuePair<Point, PixelMeta>[bitmap.Width * bitmap.Height];
				BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
				IntPtr ptr = bmpData.Scan0;
				int bytes = bmpData.Stride * bitmap.Height;
				var rgbValues = new byte[bytes];
				Marshal.Copy(ptr, rgbValues, 0, bytes);
				for (int x = 0; x < bitmap.Width; x++)
				{
					for (int y = 0; y < bitmap.Height; y++)
					{
						int position = (y * bmpData.Stride) + (x * 3);
						int blue = rgbValues[position];
						int green = rgbValues[position + 1];
						int red = rgbValues[position + 2];
						Color currentColor = Color.FromArgb(red, green, blue);
						ColorSpot closestColor = knownColors.OrderBy(c => c.Color.DifferenceTo(currentColor)).FirstOrDefault();
						this._colors[x, y] = knownColors.IndexOf(closestColor);
					}
				}
				bitmap.UnlockBits(bmpData);
			}

			var colorGroups = new List<List<Point>>();
			for (int x = 0; x < this._bitmapWidth; x++)
			{
				for (int y = 0; y < this._bitmapHeight; y++)
				{
					if (this._colors[x, y] != ignoredColor && !this._colorsProcessed[x, y])
					{
						colorGroups.AddRange(this.GetNeighboringColors(x, y));
					}
				}
			}

			int? lastColorIndex = null;
			IOrderedEnumerable<List<Point>> colorGroupsOrderedByColor =
				colorGroups.Where(x => x.Count >= colorGroupMinSize).OrderBy(x => this._colors[x.First().X, x.First().Y]);
			foreach (var colorGroup in colorGroupsOrderedByColor)
			{
				var colorOutput = new List<MouseDragAction>();
				int currentColorIndex = this._colors[colorGroup.First().X, colorGroup.First().Y];
				if (!lastColorIndex.HasValue || lastColorIndex.Value != currentColorIndex)
				{
					lastColorIndex = currentColorIndex;
					ColorSpot currentColor = knownColors[this._colors[colorGroup.First().X, colorGroup.First().Y]];
					var colorChangeAction = new MouseDragAction(new List<Point> { currentColor.Point }, true, currentColor.Color);
					colorOutput.Add(colorChangeAction);
				}
				var ma = new MouseDragAction(colorGroup.Select(x => new Point(x.X, x.Y)).ToList());
				colorOutput.Add(ma);
				output.AddRange(colorOutput);
			}

			return output;
		}

		private static bool AreDirectNeighbors(Point point1, Point point2)
		{
			return Math.Abs(point1.X - point2.X) <= 15 && Math.Abs(point1.Y - point2.Y) <= 15;
		}

		private IEnumerable<Point> GetMatchingUnprocessedDirectNeighbors(int x, int y)
		{
			var directNeighbors = new List<Point>();
			// top
			int newX = x;
			int newY = y - 1;
			if (newY > 0 && !this._colorsProcessed[newX, newY] && this._colors[x, y] == this._colors[newX, newY])
			{
				directNeighbors.Add(new Point { X = newX, Y = newY });
			}
			// right
			newX = x + 1;
			newY = y;
			if (newX < this._bitmapWidth && !this._colorsProcessed[newX, newY] && this._colors[x, y] == this._colors[newX, newY])
			{
				directNeighbors.Add(new Point { X = newX, Y = newY });
			}
			// bottom
			newX = x;
			newY = y + 1;
			if (newY < this._bitmapHeight && !this._colorsProcessed[newX, newY] && this._colors[x, y] == this._colors[newX, newY])
			{
				directNeighbors.Add(new Point { X = newX, Y = newY });
			}
			// left
			newX = x - 1;
			newY = y;
			if (newX > 0 && !this._colorsProcessed[newX, newY] && this._colors[x, y] == this._colors[newX, newY])
			{
				directNeighbors.Add(new Point { X = newX, Y = newY });
			}
			return directNeighbors;
		}
		private IEnumerable<List<Point>> GetNeighboringColors(int x, int y)
		{
			var results = new List<List<Point>>();
			var result = new List<Point>();
			results.Add(result);
			var currentPoint = new Point { X = x, Y = y };
			this._colorsProcessed[x, y] = true;
			result.Add(currentPoint);

			var processQueue = new List<Point> { currentPoint };
			int i = 0;

			while (processQueue.Count > i)
			{
				Point neighbor = processQueue.ElementAt(i);
				i++;
				foreach (Point newNeighbor in this.GetMatchingUnprocessedDirectNeighbors(neighbor.X, neighbor.Y))
				{
					this._colorsProcessed[newNeighbor.X, newNeighbor.Y] = true;
					if (!AreDirectNeighbors(result.Last(), newNeighbor))
					{
						result = new List<Point>();
						results.Add(result);
					}
					processQueue.Insert(i, newNeighbor);
					result.Add(newNeighbor);
				}
			}
			return results;
		}
	}
}
