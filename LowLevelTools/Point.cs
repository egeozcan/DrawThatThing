namespace Helpers
{
	public class Point
	{
		public int X;
		public int Y;
		public readonly bool IsEmpty;

		public Point(int x, int y)
		{
			X = x;
			Y = y;
		}

		private Point(bool setEmpty)
		{
			this.IsEmpty = setEmpty;
			X = Y = 0;
		}

		public static Point Empty = new Point(true);
	}
}