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
		private int[,] _colors;
		private bool[,] _colorsProcessed;
		private int _bitmapHeight;
		private int _bitmapWidth;

		public IEnumerable<MouseDragAction> getDrawInstructions(ColorSpot[] color)
		{
			var knownColors = color.ToList();
			var ignoredColor = knownColors.IndexOf(knownColors.First(x => x.color.Name == "ffffffff"));
			// prevent drawing of single pixels etc
			const int colorGroupMinSize = 15;

			List<MouseDragAction> output = new List<MouseDragAction>();

			using (var bitmap = new Bitmap(_bitmapPath))
			{
				_bitmapHeight = bitmap.Height;
				_bitmapWidth = bitmap.Width;
				Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
				_colors = new int[bitmap.Width, bitmap.Height];
				_colorsProcessed = new bool[bitmap.Width, bitmap.Height];
				//_pixels = new KeyValuePair<Point, PixelMeta>[bitmap.Width * bitmap.Height];
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
						var closestColor = knownColors.OrderBy(c => c.color.DifferenceTo(currentColor)).FirstOrDefault();
						_colors[x, y] = knownColors.IndexOf(closestColor);
					}
				}
				bitmap.UnlockBits(bmpData);
			}



			var colorGroups = new List<List<SimplePoint>>();
			for (int x = 0; x < _bitmapWidth; x++)
			{
				for (int y = 0; y < _bitmapHeight; y++)
				{
					if (_colors[x, y] != ignoredColor && !_colorsProcessed[x, y])
					{
						_colorsProcessed[x, y] = true;
						var colorGroup = new List<SimplePoint> { new SimplePoint { X = x, Y = y } };
						GetNeighboringColors(x, y, colorGroup);
						colorGroups.Add(colorGroup);
					}

				}
			}

			int? lastColorIndex = null;
			var colorGroupsOrderedByColor = colorGroups.Where(x => x.Count >= colorGroupMinSize).OrderBy(x => _colors[x.First().X, x.First().Y]);
			foreach (var colorGroup in colorGroupsOrderedByColor)
			{
				var colorOutput = new List<MouseDragAction>();
				var currentColorIndex = _colors[colorGroup.First().X, colorGroup.First().Y];
				if (!lastColorIndex.HasValue || lastColorIndex.Value != currentColorIndex)
				{
					lastColorIndex = currentColorIndex;
					var currentColor = knownColors[_colors[colorGroup.First().X, colorGroup.First().Y]];
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
				_colorsProcessed[neighbor.X, neighbor.Y] = true;
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
			if (newY > 0 && !_colorsProcessed[newX, newY] && _colors[x, y] == _colors[newX, newY])
			{
				directNeighbors.Add(new SimplePoint { X = newX, Y = newY });
			}
			// right
			newX = x + 1;
			newY = y;
			if (newX < _bitmapWidth && !_colorsProcessed[newX, newY] && _colors[x, y] == _colors[newX, newY])
			{
				directNeighbors.Add(new SimplePoint { X = newX, Y = newY });
			}
			// bottom
			newX = x;
			newY = y + 1;
			if (newY < _bitmapHeight && !_colorsProcessed[newX, newY] && _colors[x, y] == _colors[newX, newY])
			{
				directNeighbors.Add(new SimplePoint { X = newX, Y = newY });
			}
			// left
			newX = x - 1;
			newY = y;
			if (newX > 0 && !_colorsProcessed[newX, newY] && _colors[x, y] == _colors[newX, newY])
			{
				directNeighbors.Add(new SimplePoint { X = newX, Y = newY });
			}
			return directNeighbors;
		}

		public ColoredBitmapReader(string path)
		{
			_bitmapPath = path;
		}

		public class SimplePoint
		{
			public int X { get; set; }
			public int Y { get; set; }
		}

	}
}