namespace BitmapReader.Classes
{
	using System.Drawing;

	using Enum;

	public class Brush
	{
		public Color Color { get; set; }
		public BrushSize Size { get; set; }
		public BrushShape Shape { get; set; }

		public Brush()
		{
		}

		public Brush(Color color, BrushSize size, BrushShape shape)
		{
			this.Color = color;
			this.Size = size;
			this.Shape = shape;
		}
	}
}