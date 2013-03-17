namespace BitmapReader.Extensions
{
	using System;
	using System.Drawing;
	using System.Runtime.InteropServices;

	using Point = Classes.Point;

	public static class BitmapExtensions
	{
		public static void LoopThroughPixels(this Bitmap bitmap, Action<Point, Color> action)
		{
			Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

			System.Drawing.Imaging.BitmapData bmpData = bitmap.LockBits(
				rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

			IntPtr ptr = bmpData.Scan0;

			int bytes = bmpData.Stride * bitmap.Height;
			byte[] rgbValues = new byte[bytes];

			Marshal.Copy(ptr, rgbValues, 0, bytes);

			for (int x = 0; x < bitmap.Width; x++)
			{
				for (int y = 0; y < bitmap.Height; y++)
				{
					int position = (y * bmpData.Stride) + (x * 3);
					int red = rgbValues[position + 2];
					int green = rgbValues[position + 1];
					int blue = rgbValues[position];
					action.Invoke(new Point(x,y), Color.FromArgb(red, green, blue));
				}
			}
			bitmap.UnlockBits(bmpData);
		}
	}
}