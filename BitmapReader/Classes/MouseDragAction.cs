namespace BitmapReader.Classes
{
	using System.Collections.Generic;
	using System.Drawing;
	using System.Linq;

	using Win32;

	using Point = Classes.Point;

	public class MouseDragAction
	{
		public readonly List<Classes.Point> Points;
		public readonly bool DiscardOffset;
		public readonly Color Color;

		public MouseDragAction(List<Classes.Point> points, bool discardOffset = false, Color? color = null)
		{
			this.Points = points;
			this.DiscardOffset = discardOffset;
			this.Color = color.HasValue ? color.Value : Color.Empty;
		}

		public void PushPoint(Classes.Point point)
		{
			this.Points.Add(point);
		}

		public void AddPoint(Classes.Point point)
		{
			this.Points.Insert(0, point);
		}

		public IEnumerable<bool> Play(Classes.Point offset)
		{
			if (this.DiscardOffset)
			{
				offset = new Classes.Point(0, 0);
				System.Threading.Thread.Sleep(100);
			}
			if (this.Points.Count == 0)
			{
				yield return new bool();
			}
			else
			{
				if (this.DiscardOffset)
				{
					System.Threading.Thread.Sleep(1000);
				}
				int waitTime = this.DiscardOffset ? 100 : 10;
				int loopWaitTime = this.DiscardOffset ? 100 : 1;
				MouseOperations.SetCursorPosition(
						new MouseOperations.MousePoint(this.Points[0].X + offset.X, this.Points[0].Y + offset.Y));
				MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
				System.Threading.Thread.Sleep(waitTime);
				foreach (var point in this.Points.Where(point => !point.IsEmpty))
				{
					MouseOperations.SetCursorPosition(
						new MouseOperations.MousePoint(point.X + offset.X, point.Y + offset.Y));
					yield return true;
					System.Threading.Thread.Sleep(loopWaitTime);
				}
				System.Threading.Thread.Sleep(waitTime);
				MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
			}
		}
	}
}
