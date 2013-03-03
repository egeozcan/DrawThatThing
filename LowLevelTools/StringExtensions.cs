namespace LowLevelTools
{
	public static class StringExtensions
	{
		public static int AsInt(this string str)
		{
			int res;
			int.TryParse(str, out res);
			return res;
		}
	}
}