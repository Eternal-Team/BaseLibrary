using System;
using Microsoft.Xna.Framework;

namespace BaseLibrary.Utility;

public static class MathUtility
{
	public const float E = 2.718282f;
	public const float PI = 3.141593f;
	public const float Tau = 6.283185f;
	public const float PiOver2 = 1.570796f;
	public const float PiOver4 = 0.7853982f;

	public const float Rad2Deg = 180f / PI;
	public const float Deg2Rad = PI / 180f;

	public static ulong Min(ulong a, ulong b, ulong c)
	{
		return Math.Min(a, Math.Min(b, c));
	}

	public static float Min(float a, float b, float c, float d)
	{
		return Math.Min(a, Math.Min(b, Math.Min(c, d)));
	}

	public static float Map(float value, float fromLow, float fromHigh, float toLow, float toHigh)
	{
		return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
	}

	public static float Lerp(float value, float min, float max)
	{
		return min * (1 - value) + max * value;
	}

	public static bool CheckAABBvAABBCollision(Rectangle a, Rectangle b) => a.X < b.X + b.Width && a.X + a.Width > b.X && a.Y < b.Y + b.Height && a.Y + a.Height > b.Y;

	// public static bool Contains(this Rectangle rectangle, Vector2 point) => rectangle.X <= point.X && point.X < rectangle.X + rectangle.Width && rectangle.Y <= point.Y && point.Y < rectangle.Y + rectangle.Height;

	public static int Clamp(int value, int min, int max)
	{
		if (value < min) return min;
		return value > max ? max : value;
	}

	public static float AngleBetween(Vector2 vector1, Vector2 vector2)
	{
		double sin = vector1.X * vector2.Y - vector2.X * vector1.Y;
		double cos = vector1.X * vector2.X + vector1.Y * vector2.Y;

		return (float)Math.Atan2(sin, cos);
	}

	public static void SetBit(ref byte value, int pos)
	{
		value = (byte)(value | (1 << pos));
	}

	public static void SetBit(ref ushort value, int pos)
	{
		value = (ushort)(value | (1 << pos));
	}

	public static void SetBit(ref uint value, int pos)
	{
		value = (uint)(value | (1 << pos));
	}

	public static void ResetBit(ref byte value, int pos)
	{
		value = (byte)(value & ~(1 << pos));
	}

	public static void ResetBit(ref ushort value, int pos)
	{
		value = (ushort)(value & ~(1 << pos));
	}

	public static void ResetBit(ref uint value, int pos)
	{
		value = (uint)(value & ~(1 << pos));
	}

	public static bool IsBitSet(ushort value, int pos)
	{
		return (value & (1 << pos)) != 0;
	}
}