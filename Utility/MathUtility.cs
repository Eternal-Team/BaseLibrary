using System.Numerics;
using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;

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

	public static void Clamp<T>(ref T value, T min, T max) where T : IComparisonOperators<T, T, bool>
	{
		if (value < min) value = min;
		else if (value > max) value = max;
	}

	public static bool Contains(this Rectangle rectangle, Vector2 point)
	{
		return rectangle.X <= point.X && point.X < rectangle.X + rectangle.Width && rectangle.Y <= point.Y && point.Y < rectangle.Y + rectangle.Height;
	}

	public static Vector2 OffsetBy(this Vector2 vector, float x, float y) => new(vector.X + x, vector.Y + y);
}