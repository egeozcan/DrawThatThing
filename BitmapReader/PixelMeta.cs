namespace BitmapReader
{
	using System.Drawing;

	using Classes;

	public class PixelMeta
	{
		public Color color;
		public bool expired;
		public ColorSpot closestColor;
	}
}