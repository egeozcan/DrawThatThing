using DrawThatThing.Core.Models;

namespace DrawThatThing.Core.Extensions;

public static class PointExtensions
{
    public static bool IsANeighborOf(this Point point, Point toPoint)
    {
        return Math.Abs(point.X - toPoint.X) <= 1 && Math.Abs(point.Y - toPoint.Y) <= 1;
    }
}
