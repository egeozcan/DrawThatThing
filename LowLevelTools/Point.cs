namespace BitmapReader
{
	public class Point
	{
		public int X;
		public int Y;
		public readonly bool IsEmpty;

		public Point(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		private Point(bool setEmpty)
		{
			this.IsEmpty = setEmpty;
			this.X = this.Y = 0;
		}

		public static Point Empty = new Point(true);
	}
}