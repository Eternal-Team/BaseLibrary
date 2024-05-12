using System.Numerics;

namespace BaseLibrary;

public static class MathUtility
{
	public static T Min<T>(T a, T b, T c) where T : IComparisonOperators<T, T, bool>
	{
		T m = a;
		if (m > b) m = b;
		if (m > c) m = c;
		return m;
	}

	public static T Max<T>(T a, T b, T c) where T : IComparisonOperators<T, T, bool>
	{
		T m = a;
		if (m < b) m = b;
		if (m < c) m = c;
		return m;
	}
}