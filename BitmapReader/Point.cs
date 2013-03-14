namespace BitmapReader
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

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        private Point(bool setEmpty)
        {
            IsEmpty = setEmpty;
            X = Y = 0;
        }
    }
}