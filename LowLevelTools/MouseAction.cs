using System;

namespace LowLevelTools
{
	public class MouseAction
	{
		public enum MouseState
		{
			Clicked,
			Released
		}

		public int x1 { get; set; }
		public int x2 { get; set; }
		public int y1 { get; set; }
		public int y2 { get; set; }
		public MouseState state;

		public MouseAction setClick(int x, int y)
		{
			x1 = x2 = x;
			y1 = y2 = y;
			state = MouseState.Clicked;
			return this;
		}

		public MouseAction setDrag(int ax, int ay, int bx, int by)
		{
			x1 = ax;
			y1 = ay;
			x2 = bx;
			y2 = by;
			state = MouseState.Clicked;
			return this;
		}

		public MouseAction setJump(int x, int y)
		{
			setClick(x, y);
			state = MouseState.Released;
			return this;
		}

		public MouseAction setMove(int ax, int ay, int bx, int by)
		{
			setDrag(ax, ay, bx, by);
			state = MouseState.Released;
			return this;
		}

		public override string ToString()
		{
			if (x1 != x2 || y1 != y2)
			{
				return String.Format("{0} {1} {2} {3}", y1, x1, y2, x2);				
			}
			return String.Format("{0} {1}", y1, x1);
		}
	}
}
