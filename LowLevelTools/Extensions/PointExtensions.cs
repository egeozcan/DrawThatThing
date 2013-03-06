namespace Helpers.Extensions
{
	using System;
	using System.Linq;

	public static class PointExtensions
	{
		public static bool IsANeighborOf(this Point point, Point toPoint)
		{
			return Math.Abs(point.X - toPoint.X) <= 1 && Math.Abs(point.Y - toPoint.Y) <= 1;
		}

		public static Point[] GetNeighboringPoints(this Point point, int? maxX = null, int? maxY = null)
		{
			var neighboringPoints = new[]
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
			if (maxX.HasValue)
			{
				neighboringPoints = neighboringPoints.Where(x => x.X <= maxX.Value).ToArray();
			}
			if (maxY.HasValue)
			{
				neighboringPoints = neighboringPoints.Where(x => x.Y <= maxY.Value).ToArray();
			}
			return neighboringPoints;
		}

		public static System.Drawing.Point ToStandardPoint(this Point point)
		{
			return new System.Drawing.Point(point.X, point.Y);
		}
	}
}