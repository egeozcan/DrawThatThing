namespace BitmapReader.Classes
{
	using System.Drawing;

	public class BrushSize
	{
		public float Width { get; set; }
		public float Height { get; set; }

		public static implicit operator SizeF(BrushSize bs)
		{
			return new SizeF(bs.Width, bs.Height);
		}

		public static explicit operator Size(BrushSize bs)
		{
			return new Size((int)bs.Width, (int)bs.Height);
		}
	}
}