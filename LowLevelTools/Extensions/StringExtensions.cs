namespace Helpers.Extensions
{
	using System;
	using System.Drawing;

	public static class StringExtensions
	{
		public static int ToInt(this string str)
		{
			int res;
			int.TryParse(str, out res);
			return res;
		}

		public static Color ToColor(this string str)
		{
			try
			{
				return ColorTranslator.FromHtml(str);
			}
			catch (Exception)
			{
				return Color.Empty;
			}
		}
	}
}