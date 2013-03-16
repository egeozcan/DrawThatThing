namespace BitmapReader.Extensions
{
	using System;
	using System.Drawing;
	using System.Linq;

	public static class StringExtensions
	{
		public static int ToInt(this string str, int defVal = 0)
		{
			int res;
			return int.TryParse(str, out res) ? res : defVal;
		}

		public static bool ToBool(this string str, bool defVal = false)
		{
			return str.EqualsToAny("true", "1");
		}

		public static bool EqualsToAny(this string str, params string[] values)
		{
			return values.Any(x => x == str);
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