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
		const int ColorGroupMinSize = 1;
		private readonly string _bitmapPath;
		private int[,] _pixelColorIndex;
		private bool[,] _pixelIsProcessed;
		private int _bitmapHeight;
		private int _bitmapWidth;

		public IEnumerable<MouseDragAction> getDrawInstructions(ColorSpot[] ColorPalette)
		{
			var knownColors = ColorPalette.ToList();
			var ignoredColor = knownColors.IndexOf(knownColors.First(x => x.color.Name == "ffffffff"));
			// prevent drawing of single pixels etc

			List<MouseDragAction> output = new List<MouseDragAction>();

			using (var bitmap = new Bitmap(_bitmapPath))
			{
				_bitmapHeight = bitmap.Height;
				_bitmapWidth = bitmap.Width;
				Rectangle rect = new Rectangle(0, 0, _bitmapWidth, _bitmapHeight);
				this._pixelColorIndex = new int[_bitmapWidth, _bitmapHeight];
				this._pixelIsProcessed = new bool[_bitmapWidth, _bitmapHeight];
				BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
				IntPtr ptr = bmpData.Scan0;
				int bytes = bmpData.Stride * _bitmapHeight;
				byte[] rgbValues = new byte[bytes];
				Marshal.Copy(ptr, rgbValues, 0, bytes);
				for (int x = 0; x < _bitmapWidth; x++)
				{
					for (int y = 0; y < _bitmapHeight; y++)
					{
						int position = (y * bmpData.Stride) + (x * 3);
						int blue = rgbValues[position];
						int green = rgbValues[position + 1];
						int red = rgbValues[position + 2];
						var currentColor = Color.FromArgb(red, green, blue);
						var closestColor = knownColors.OrderBy(c => c.color.DifferenceTo(currentColor)).FirstOrDefault();
						this._pixelColorIndex[x, y] = knownColors.IndexOf(closestColor);
					}
				}
				bitmap.UnlockBits(bmpData);
			}

			var colorGroups = new List<List<SimplePoint>>();
			for (int x = 0; x < _bitmapWidth; x++)
			{
				for (int y = 0; y < _bitmapHeight; y++)
				{
					if (this._pixelColorIndex[x, y] == ignoredColor || this._pixelIsProcessed[x, y])
					{
						continue;
					}
					this._pixelIsProcessed[x, y] = true;
					var colorGroup = new List<SimplePoint> { new SimplePoint { X = x, Y = y } };
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
					var colorChangeAction = new MouseDragAction(new[] { currentColor.point }, true) { Color = currentColor.color };
					colorOutput.Add(colorChangeAction);
				}
				var ma = new MouseDragAction(colorGroup.Select(x => new Point(x.X, x.Y)).ToArray());
				colorOutput.Add(ma);
				output.AddRange(colorOutput);
			}

			return output;
		}

		private void GetNeighboringColors(int x, int y, List<SimplePoint> neighboringColors)
		{
			var neighbors = GetMatchingUnprocessedDirectNeighbors(x, y);
			foreach (var neighbor in neighbors)
			{
				this._pixelIsProcessed[neighbor.X, neighbor.Y] = true;
				neighboringColors.Add(neighbor);
			}
			foreach (var neighbor in neighbors)
			{
				GetNeighboringColors(neighbor.X, neighbor.Y, neighboringColors);
			}
		}

		private List<SimplePoint> GetMatchingUnprocessedDirectNeighbors(int x, int y)
		{
			var directNeighbors = new List<SimplePoint>();
			// top
			var newX = x;
			var newY = y - 1;
			if (newY > 0 && !this._pixelIsProcessed[newX, newY] && this._pixelColorIndex[x, y] == this._pixelColorIndex[newX, newY])
			{
				directNeighbors.Add(new SimplePoint { X = newX, Y = newY });
			}
			// right
			newX = x + 1;
			newY = y;
			if (newX < _bitmapWidth && !this._pixelIsProcessed[newX, newY] && this._pixelColorIndex[x, y] == this._pixelColorIndex[newX, newY])
			{
				directNeighbors.Add(new SimplePoint { X = newX, Y = newY });
			}
			// bottom
			newX = x;
			newY = y + 1;
			if (newY < _bitmapHeight && !this._pixelIsProcessed[newX, newY] && this._pixelColorIndex[x, y] == this._pixelColorIndex[newX, newY])
			{
				directNeighbors.Add(new SimplePoint { X = newX, Y = newY });
			}
			// left
			newX = x - 1;
			newY = y;
			if (newX > 0 && !this._pixelIsProcessed[newX, newY] && this._pixelColorIndex[x, y] == this._pixelColorIndex[newX, newY])
			{
				directNeighbors.Add(new SimplePoint { X = newX, Y = newY });
			}
			return directNeighbors;
		}

		public ColoredBitmapReader(string path)
		{
			_bitmapPath = path;
		}
	}
}