using System;
using Microsoft.Xna.Framework;

namespace BaseLibrary.Utility
{
	public static class MathUtility
	{
		public const float Rad2Deg = 180f / MathHelper.Pi;
		public const float Deg2Rad = MathHelper.Pi / 180f;

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

		public static bool Contains(this Rectangle rectangle, Vector2 point) => rectangle.X <= point.X && point.X < rectangle.X + rectangle.Width && rectangle.Y <= point.Y && point.Y < rectangle.Y + rectangle.Height;
	}
}