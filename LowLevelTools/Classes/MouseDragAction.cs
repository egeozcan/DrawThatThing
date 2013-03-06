namespace Helpers
{
	using System.Drawing;
	using System.Linq;

	public class MouseDragAction
	{
		public readonly Point[] Points;
		public readonly bool DiscardOffset;
		public Color Color;

		public MouseDragAction(Point[] points, bool discardOffset = false, Color? color = null)
		{
			this.Points = points;
			this.DiscardOffset = discardOffset;
			this.Color = color.HasValue ? color.Value : Color.Empty;
		}

		public void Play(Point offset)
		{
			if (this.DiscardOffset)
			{
				offset = new Point(0,0);
			}
			if (this.Points.Length < 0)
			{
				return;
			}
			MouseOperations.SetCursorPosition(
					new MouseOperations.MousePoint(this.Points[0].X + offset.X, this.Points[0].Y + offset.Y));
			MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
			System.Threading.Thread.Sleep(10);
			foreach (var point in this.Points.Where(point => !point.IsEmpty))
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
