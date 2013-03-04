namespace LowLevelTools
{
	using System;
	using System.Drawing;

	public static class PointExtensions
	{
		public static bool IsANeighborOf(this Point point, Point toPoint)
		{
			return Math.Abs(point.X - toPoint.X) <= 1 && Math.Abs(point.Y - toPoint.Y) <= 1;
		}

		public static Point[] GetNeighboringPoints(this Point point)
		{
			return new[]
				{
					new Point(point.X, point.Y + 1), 
					new Point(point.X, point.Y - 1), 
					new Point(point.X + 1, point.Y + 1), 
					new Point(point.X + 1, point.Y - 1), 
					new Point(point.X + 1, point.Y), 
					new Point(point.X - 1, point.Y + 1), 
					new Point(point.X - 1, point.Y - 1), 
					new Point(point.X - 1, point.Y), 
				};
		}
	}
}