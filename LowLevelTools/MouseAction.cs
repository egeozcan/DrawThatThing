using System;

namespace LowLevelTools
{
	using System.Collections.Generic;
	using System.Drawing;

	public class MouseAction
	{
		public enum MouseState
		{
			Clicked,
			Released
		}

		public struct PointInfo
		{
			public readonly Point? _point;
			public readonly MouseState _mouseState;
			public PointInfo(Point? point, MouseState mouseState)
			{
				this._point = point;
				this._mouseState = mouseState;
			}
			public override string ToString()
			{
				return !this._point.HasValue ? 
					String.Format("keep keep {0}", this._mouseState) : 
					String.Format("{0} {1} {2}", this._point.Value.X, this._point.Value.Y, this._mouseState);
			}
			public void Play()
			{
				
			}
		}

		private readonly IList<PointInfo> _points = new List<PointInfo>();

		private void releaseMouse()
		{
			if (_points.Count > 0 && _points[_points.Count - 1]._mouseState == MouseState.Released)
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
			foreach (Point point in points)
			{
				this._points.Add(new PointInfo(point, MouseState.Clicked));
			}
			this.releaseMouse();
			return this;
		}

		public override string ToString()
		{
			return String.Join("\n", _points);
		}
	}
}
