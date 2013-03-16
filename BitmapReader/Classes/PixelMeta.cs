namespace BitmapReader.Classes
{
	using System.Drawing;

	public class PixelMeta
	{
		public Color color;
		public bool expired;
		public ColorSpot closestColor;
	}
}