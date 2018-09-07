using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseLibrary.Utility
{
	public static partial class Utility
	{
		public static bool ContainsAll<T>(this List<T> superset, List<T> subset) => superset.Except(subset).Any();

		public static bool IsEqual<T>(this List<T> list1, List<T> list2) => list1.All(list2.Contains) && list1.Count == list2.Count;

		public static int[,] To2DArray(this IList<int> arr, int width, int height)
		{
			if (arr.Count < width * height) throw new Exception("List does not contain enough elements");

			int[,] newArr = new int[width, height];
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					newArr[i, j] = arr.ElementAt(i * height + j);
				}
			}

			return newArr;
		}

		public static string Aggregate(this IEnumerable<string> enumerable, string joinSequence = ";") => enumerable.Any() ? enumerable.Aggregate((x, y) => x + joinSequence + y) : string.Empty;
	}
}