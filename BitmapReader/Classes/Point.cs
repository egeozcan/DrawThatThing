namespace BitmapReader.Classes
{
    public class Point
    {
        public static Point Empty = new Point(true);
        public readonly bool IsEmpty;
        public int X;
        public int Y;

        public Point()
        {
        }

		public static implicit operator System.Drawing.Point (Point p)
		{
			return new System.Drawing.Point(p.X, p.Y);
		}

		public static Point operator +(Point p1, Point p2)
		{
			return new Point(p1.X + p2.X, p1.Y + p2.Y);
		}

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
    }
}