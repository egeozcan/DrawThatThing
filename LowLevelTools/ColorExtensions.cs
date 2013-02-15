namespace LowLevelTools
{
	using System;
	using System.Drawing;

	public static class ColorExtensions
	{
		public static int DifferenceTo(this Color color, Color toColor)
		{
			return Math.Abs(color.R - toColor.R) + Math.Abs(color.G - toColor.G) + Math.Abs(color.B - toColor.B);
		}
	}
}