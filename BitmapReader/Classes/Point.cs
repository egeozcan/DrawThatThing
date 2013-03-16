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