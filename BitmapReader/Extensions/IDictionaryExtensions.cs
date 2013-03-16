using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitmapReader.Extensions
{
	public static class IDictionaryExtensions
	{
		public static int GetIntValueOrDefault(this IDictionary<string, string> dict, string key, int defVal)
		{
			string output;
			return dict.TryGetValue(key, out output) ? output.ToInt(defVal) : defVal;
		}
	
		public static bool GetBoolValueOrDefault(this IDictionary<string, string> dict, string key, bool defVal)
		{
			string output;
			return dict.TryGetValue(key, out output) ? output.ToBool(defVal) : defVal;
		}
	}
}
