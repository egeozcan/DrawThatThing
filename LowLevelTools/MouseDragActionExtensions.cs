namespace LowLevelTools
{
	using System.Drawing;
	using System.Linq;

	public static class MouseDragActionExtensions
	{
		public static void Play(this MouseDragAction action, Point offset)
		{
			var Points = action.Points;
			if (action.DiscardOffset)
			{
				offset = new Point(0, 0);
			}
			if (Points.Length < 0)
			{
				return;
			}
			MouseOperations.SetCursorPosition(
					new MouseOperations.MousePoint(Points[0].X + offset.X, Points[0].Y + offset.Y));
			MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
			System.Threading.Thread.Sleep(10);
			foreach (var point in Points.Where(point => !point.IsEmpty))
			{
				MouseOperations.SetCursorPosition(
					new MouseOperations.MousePoint(point.X + offset.X, point.Y + offset.Y));
				System.Threading.Thread.Sleep(1);
			}
			System.Threading.Thread.Sleep(10);
			MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
		}
	}
}
