using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace BaseLibrary.Utility
{
	public static partial class Utility
	{
		public static string ToSI<T>(this T value, string format = null)
		{
			double castValue = (double)Convert.ChangeType(value, TypeCode.Double);

			char[] incPrefixes = { 'k', 'M', 'G', 'T', 'P', 'E', 'Z', 'Y' };
			char[] decPrefixes = { 'm', '\u03bc', 'n', 'p', 'f', 'a', 'z', 'y' };

			if (Math.Abs(castValue) > 0.0)
			{
				int degree = (int)Math.Floor(Math.Log10(Math.Abs(castValue)) / 3);
				double scaled = castValue * Math.Pow(1000, -degree);

				char? prefix = null;
				switch (Math.Sign(degree))
				{
					case 1:
						prefix = incPrefixes[degree - 1];
						break;
					case -1:
						prefix = decPrefixes[-degree - 1];
						break;
				}

				return scaled.ToString(format) + prefix;
			}

			return castValue.ToString(format);
		}

		public static Point16 Min(this Point16 point, Point16 compareTo) => new Point16(point.X > compareTo.X ? compareTo.X : point.X, point.Y > compareTo.Y ? compareTo.Y : point.Y);

		public static T Min<T>(params T[] values) => values.Min();

		public static T Max<T>(params T[] values) => values.Max();

		public static T Clamp<T>(this T value, T min, T max)
		{
			double castValue = (double)Convert.ChangeType(value, TypeCode.Double);
			double castMin = (double)Convert.ChangeType(min, TypeCode.Double);
			double castMax = (double)Convert.ChangeType(max, TypeCode.Double);

			if (castValue < castMin) castValue = castMin;
			if (castValue > castMax) castValue = castMax;
			return (T)Convert.ChangeType(castValue, value.GetType());
		}

		public static int ToNearest(this float value, int step)
		{
			int output = (int)Math.Round((double)value / step);
			if (Math.Abs(output) < 0.0 && value > 0) output += 1;
			return output * step;
		}

		public static bool InTriangle(this Point point, Point t0, Point t1, Point t2)
		{
			var s = t0.Y * t2.X - t0.X * t2.Y + (t2.Y - t0.Y) * point.X + (t0.X - t2.X) * point.Y;
			var t = t0.X * t1.Y - t0.Y * t1.X + (t0.Y - t1.Y) * point.X + (t1.X - t0.X) * point.Y;

			if (s < 0 != t < 0) return false;

			var A = -t1.Y * t2.X + t0.Y * (t2.X - t1.X) + t0.X * (t1.Y - t2.Y) + t1.X * t2.Y;
			if (A < 0.0)
			{
				s = -s;
				t = -t;
				A = -A;
			}

			return s > 0 && t > 0 && s + t <= A;
		}

		public static bool InTriangle(this Point point, Point[] array) => point.InTriangle(array[0], array[1], array[2]);

		public static float ToRadians(this float angle) => MathHelper.Pi / 180f * angle;

		public static T Remap<T>(this T value, double low1, double high1, double low2, double high2)
		{
			double castValue = (double)Convert.ChangeType(value, TypeCode.Double);
			return (T)Convert.ChangeType(low2 + (castValue - low1) * (high2 - low2) / (high1 - low1), value.GetType());
		}
	}
}