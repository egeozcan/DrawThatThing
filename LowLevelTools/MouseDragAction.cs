namespace LowLevelTools
{
	using System.Drawing;

	public struct MouseDragAction
	{
		public readonly Point[] Points;
		public readonly bool DiscardOffset;
		public readonly Color Color;

		public MouseDragAction(Point[] points, bool discardOffset = false, Color? color = null)
		{
			this.Points = points;
			this.DiscardOffset = discardOffset;
			this.Color = color.HasValue ? color.Value : Color.Empty;
		}
	}
}
