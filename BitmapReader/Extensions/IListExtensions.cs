using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitmapReader.Extensions
{
	public static class IListExtensions
	{
		static readonly Random Random = new Random();
		public static IList<T> Shuffle<T>(this IList<T> list)
		{
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = Random.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
			return list;
		}

	}
}
