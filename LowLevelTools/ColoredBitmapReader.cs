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
		private string _bitmapPath;
		private Dictionary<Point, PixelMeta> _pixels = new Dictionary<Point, PixelMeta>(); 

		public IEnumerable<MouseAction> getDrawInstructions(PixelAcceptanceArgs args, ColorSpot[] color)
		{
			List<MouseAction> output = new List<MouseAction>();

			using (var bitmap = new Bitmap(_bitmapPath))
			{
				Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
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
						_pixels.Add
							(
								new Point(x, y),
								new PixelMeta
									{
										color = Color.FromArgb(red, green, blue),
										expired = false
									}
							);
					}
				}
				bitmap.UnlockBits(bmpData);
			}

			foreach (var selectedColor in color)
			{
				var changeColor = new MouseAction();
				changeColor.setClick(selectedColor.point.X, selectedColor.point.Y);
				output.Add(changeColor);
				var nextMoves = this.getNextMoves(selectedColor);
				if (nextMoves != null)
				{
					output.AddRange(nextMoves);
				}
			}



			return output;
		}

		private IEnumerable<MouseAction> getNextMoves(ColorSpot color)
		{
			List<MouseAction> output = new List<MouseAction>();
			const int maxDifference = 20;
			var spotsToDraw = _pixels.Where(x => !x.Value.expired && x.Value.color.DifferenceTo(color.color) < maxDifference).ToDictionary(x => x.Key, y => y.Value);
			foreach (var spot in spotsToDraw)
			{
				if (spot.Value.expired)
				{
					continue;
				}
				spot.Value.expired = true;
				var x = spot.Key.X;
				var y = spot.Key.Y;
				KeyValuePair<Point, PixelMeta> unexpiredNeighbor;
				//do
				//{
				//    unexpiredNeighbor = spotsToDraw.FirstOrDefault(p => Math.Abs(x - p.Key.X) <= 1 && Math.Abs(y - p.Key.Y) <= 1);
				//    x = 
				//}
				//while (unexpiredNeighbor != null);
			}
			return output;
		}

		public ColoredBitmapReader(string path)
		{
			_bitmapPath = path;
		}

	}
}