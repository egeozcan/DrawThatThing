﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PointReader
{
	using System.Drawing;

	using BitmapReader.Attributes;
	using BitmapReader.Classes;
	using BitmapReader.Extensions;
	using BitmapReader.Interface;

	using Point = BitmapReader.Classes.Point;

	[DefaultSetting("MixPoints", false)]
	public class PointReader : IBitmapReader
	{
		private readonly string _bitmapPath;

		public PointReader(String path)
		{
			_bitmapPath = path;
		}

		public IEnumerable<MouseDragAction> GetDrawInstructions(List<ColorSpot> colorPalette, IDictionary<string, string> options = null, IBrushChanger brushChanger = null)
		{
			if (!colorPalette.Any())
			{
				throw new ArgumentException("I need some colors. Give me some colors. Thanks!");
			}
			var output = new List<MouseDragAction>();
			var colorPixels = colorPalette.ToDictionary(colorSpot => colorSpot, colorSpot => new List<Point>());
			using (var bitmap = new Bitmap(_bitmapPath))
			{
				bitmap.LoopThroughPixels((point, color) =>
					{
						var selectedColor = colorPalette.OrderBy(c => c.Color.DifferenceTo(color)).FirstOrDefault();
						if (selectedColor == null || !colorPixels.ContainsKey(selectedColor) ||
						    selectedColor.Color.DifferenceTo(Color.White) == 0)
						{
							return;
						}
						colorPixels[selectedColor].Add(point);
					});
			}
			foreach (var colorPixel in colorPixels.Where(colorPixel => colorPixel.Value.Any()))
			{
				output.Add(new MouseDragAction(new List<Point> { colorPixel.Key.Point }, true, colorPixel.Key.Color));
				var actions = colorPixel.Value.Select(point => new MouseDragAction(new List<Point> { point })).ToList();
				if (options.GetBoolValueOrDefault("MixPoints", false))
				{
					actions = actions.Shuffle().ToList();
				}
				output.AddRange(actions);
			}
			return output.ToArray();
		}
	}
}
