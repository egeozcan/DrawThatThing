using System;

namespace LowLevelTools
{
	using System.Collections.Generic;
	using System.Drawing;
	using System.Linq;

	public class MouseAction
	{
		public enum MouseState
		{
			Clicked,
			Released
		}

		public struct PointInfo
		{
			public readonly Point? Point;
			public readonly MouseState MouseState;
			public readonly bool ChangeMouseState;
			public PointInfo(Point? point, MouseState mouseState, bool changeMouseState = true)
			{
				this.Point = point;
				this.MouseState = mouseState;
				this.ChangeMouseState = changeMouseState;
			}
			public override string ToString()
			{
				return !this.Point.HasValue ? 
					String.Format("keep keep {0}", this.MouseState) : 
					String.Format("{0} {1} {2}", this.Point.Value.X, this.Point.Value.Y, this.MouseState);
			}
			public void Play(Point offset)
			{
				if (Point.HasValue)
				{
					MouseOperations.SetCursorPosition(
						new MouseOperations.MousePoint(Point.Value.X + offset.X, Point.Value.Y + offset.Y));
				}
				if (ChangeMouseState)
				{
					MouseOperations.MouseEvent(MouseState == MouseState.Clicked ? MouseOperations.MouseEventFlags.LeftDown : MouseOperations.MouseEventFlags.LeftUp);					
				}
			}
		}

		public readonly IList<PointInfo> _points = new List<PointInfo>();

		private void releaseMouse()
		{
			if (_points.Count > 0 && _points[_points.Count - 1].MouseState == MouseState.Released)
			{
				return;
			}
			_points.Add(new PointInfo(null, MouseState.Released));
		}

		private void clickMouse()
		{
			this.releaseMouse();
			_points.Add(new PointInfo(null, MouseState.Clicked));
			this.releaseMouse();
		}

		public MouseAction addClick(int x, int y)
		{
			this.releaseMouse();
			_points.Add(new PointInfo(new Point(x, y), MouseState.Clicked));
			this.releaseMouse();
			return this;
		}

		public MouseAction addClick()
		{
			this.clickMouse();
			return this;
		}

		public MouseAction addDrag(int ax, int ay, int bx, int by)
		{
			this.releaseMouse();
			_points.Add(new PointInfo(new Point(ax, ay), MouseState.Clicked));
			_points.Add(new PointInfo(new Point(bx, by), MouseState.Clicked));
			this.releaseMouse();
			return this;
		}

		public MouseAction addJump(int x, int y)
		{
			this.releaseMouse();
			_points.Add(new PointInfo(new Point(x, y), MouseState.Released));
			return this;
		}

		public MouseAction addPath(IList<Point> points)
		{
			this.releaseMouse();
			this._points.Add(new PointInfo(points.FirstOrDefault(), MouseState.Clicked));
			foreach (Point point in points)
			{
				this._points.Add(new PointInfo(point, MouseState.Clicked, false));
			}
			this.releaseMouse();
			return this;
		}

		public void Play(Point offset)
		{
			foreach (var pointInfo in _points)
			{
				pointInfo.Play(offset);
			}
			System.Threading.Thread.Sleep(50);
		}

		public override string ToString()
		{
			return String.Join("\n", _points.Select(x => x.ToString()));
		}
	}
}
