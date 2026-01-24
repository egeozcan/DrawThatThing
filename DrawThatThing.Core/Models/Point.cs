namespace DrawThatThing.Core.Models;

public class Point
{
    public static Point Empty { get; } = new Point(true);

    public bool IsEmpty { get; }
    public int X { get; set; }
    public int Y { get; set; }

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

    public static Point operator +(Point p1, Point p2)
    {
        return new Point(p1.X + p2.X, p1.Y + p2.Y);
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }

    public override bool Equals(object? obj)
    {
        if (obj is Point other)
        {
            return X == other.X && Y == other.Y && IsEmpty == other.IsEmpty;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, IsEmpty);
    }
}
