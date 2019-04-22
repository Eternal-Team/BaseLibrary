using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseLibrary
{
	public static partial class Utility
	{
		public static string Concat<T>(this IEnumerable<T> source, string joinSequence = ";")
		{
			if (source == null || joinSequence == null) throw new ArgumentNullException();

			List<T> temp = source.ToList();

			return temp.Count > 0 ? temp.Select(x => x.ToString()).Aggregate((x, y) => x + joinSequence + y) : string.Empty;
		}

		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			if (source == null || action == null) throw new ArgumentNullException();

			foreach (T element in source) action(element);
		}

		public static IEnumerable<TResult> OfType<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			if (source == null || selector == null) throw new ArgumentNullException();

			foreach (TSource element in source)
			{
				if (selector(element) is TResult result) yield return result;
			}
		}

		public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> source, TSource element)
		{
			if (source == null || element == null) throw new ArgumentNullException();

			foreach (TSource tSource in source) yield return tSource;
			yield return element;
		}
	}
}