namespace Helpers.BitmapReaders
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.Linq;
	using System.Runtime.InteropServices;

	using Classes;

	using Extensions;

	using Interface;

	using Point = Point;

	public class ColoredBitmapReader : IColoredBitmapReader
	{
		const int ColorGroupMinSize = 1;
		private readonly string _bitmapPath;
		private int[,] _pixelColorIndex;
		private bool[,] _pixelIsProcessed;
		private int _bitmapHeight;
		private int _bitmapWidth;

		public IEnumerable<MouseDragAction> getDrawInstructions(List<ColorSpot> ColorPalette)
		{
			var knownColors = ColorPalette.ToList();
			var ignoredColor = knownColors.IndexOf(knownColors.First(x => x.Color.Name == "ffffffff"));
			// prevent drawing of single pixels etc

			List<MouseDragAction> output = new List<MouseDragAction>();

			using (var bitmap = new Bitmap(this._bitmapPath))
			{
				this._bitmapHeight = bitmap.Height;
				this._bitmapWidth = bitmap.Width;
				Rectangle rect = new Rectangle(0, 0, this._bitmapWidth, this._bitmapHeight);
				this._pixelColorIndex = new int[this._bitmapWidth, this._bitmapHeight];
				this._pixelIsProcessed = new bool[this._bitmapWidth, this._bitmapHeight];
				BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
				IntPtr ptr = bmpData.Scan0;
				int bytes = bmpData.Stride * this._bitmapHeight;
				byte[] rgbValues = new byte[bytes];
				Marshal.Copy(ptr, rgbValues, 0, bytes);
				for (int x = 0; x < this._bitmapWidth; x++)
				{
					for (int y = 0; y < this._bitmapHeight; y++)
					{
						int position = (y * bmpData.Stride) + (x * 3);
						int blue = rgbValues[position];
						int green = rgbValues[position + 1];
						int red = rgbValues[position + 2];
						var currentColor = Color.FromArgb(red, green, blue);
						var closestColor = knownColors.OrderBy(c => c.Color.DifferenceTo(currentColor)).FirstOrDefault();
						this._pixelColorIndex[x, y] = knownColors.IndexOf(closestColor);
					}
				}
				bitmap.UnlockBits(bmpData);
			}

			var colorGroups = new List<List<Point>>();
			for (int x = 0; x < this._bitmapWidth; x++)
			{
				for (int y = 0; y < this._bitmapHeight; y++)
				{
					if (this._pixelColorIndex[x, y] == ignoredColor || this._pixelIsProcessed[x, y])
					{
						continue;
					}
					this._pixelIsProcessed[x, y] = true;
					var colorGroup = new List<Point> { new Point (x,y) };
					this.GetNeighboringColors(x, y, colorGroup);
					colorGroups.Add(colorGroup);
				}
			}

			int? lastColorIndex = null;
			var colorGroupsOrderedByColor = colorGroups.Where(x => x.Count >= ColorGroupMinSize).OrderBy(x => this._pixelColorIndex[x.First().X, x.First().Y]);
			foreach (var colorGroup in colorGroupsOrderedByColor)
			{
				var colorOutput = new List<MouseDragAction>();
				var currentColorIndex = this._pixelColorIndex[colorGroup.First().X, colorGroup.First().Y];
				if (!lastColorIndex.HasValue || lastColorIndex.Value != currentColorIndex)
				{
					lastColorIndex = currentColorIndex;
					var currentColor = knownColors[this._pixelColorIndex[colorGroup.First().X, colorGroup.First().Y]];
					var colorChangeAction = new MouseDragAction(new List<Point> { currentColor.Point }, true, currentColor.Color);
					colorOutput.Add(colorChangeAction);
				}
				var ma = new MouseDragAction(colorGroup.Select(x => new Point(x.X, x.Y)).ToList());
				colorOutput.Add(ma);
				output.AddRange(colorOutput);
			}

			return output;
		}

		private void GetNeighboringColors(int x, int y, List<Point> neighboringColors)
		{
			var neighbors = this.GetMatchingUnprocessedDirectNeighbors(x, y);
			foreach (var neighbor in neighbors)
			{
				this._pixelIsProcessed[neighbor.X, neighbor.Y] = true;
				neighboringColors.Add(neighbor);
			}
			foreach (var neighbor in neighbors)
			{
				this.GetNeighboringColors(neighbor.X, neighbor.Y, neighboringColors);
			}
		}

		private List<Point> GetMatchingUnprocessedDirectNeighbors(int x, int y)
		{
			var directNeighbors = new List<Point>();
			// top
			var newX = x;
			var newY = y - 1;
			if (newY > 0 && !this._pixelIsProcessed[newX, newY] && this._pixelColorIndex[x, y] == this._pixelColorIndex[newX, newY])
			{
				directNeighbors.Add(new Point(newX, newY));
			}
			// right
			newX = x + 1;
			newY = y;
			if (newX < this._bitmapWidth && !this._pixelIsProcessed[newX, newY] && this._pixelColorIndex[x, y] == this._pixelColorIndex[newX, newY])
			{
				directNeighbors.Add(new Point(newX, newY));
			}
			// bottom
			newX = x;
			newY = y + 1;
			if (newY < this._bitmapHeight && !this._pixelIsProcessed[newX, newY] && this._pixelColorIndex[x, y] == this._pixelColorIndex[newX, newY])
			{
				directNeighbors.Add(new Point(newX, newY));
			}
			// left
			newX = x - 1;
			newY = y;
			if (newX > 0 && !this._pixelIsProcessed[newX, newY] && this._pixelColorIndex[x, y] == this._pixelColorIndex[newX, newY])
			{
				directNeighbors.Add(new Point(newX, newY));
			}
			return directNeighbors;
		}

		public ColoredBitmapReader(string path)
		{
			this._bitmapPath = path;
		}
	}
}