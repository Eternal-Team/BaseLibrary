using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
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

		public static IEnumerable<int> DistributeInteger(int total, int divider)
		{
			if (divider == 0) yield return 0;
			else
			{
				int rest = total % divider;
				double result = total / (double)divider;

				for (int i = 0; i < divider; i++)
				{
					if (rest-- > 0) yield return (int)Math.Ceiling(result);
					else yield return (int)Math.Floor(result);
				}
			}
		}

		public static bool IsInTriangle(this Point point, Point t0, Point t1, Point t2)
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

		public static bool IsInTriangle(this Point point, Point[] array) => point.IsInTriangle(array[0], array[1], array[2]);

		public static bool IsInCircularSector(this Vector2 point, Vector2 center, float radius, float startAngle, float endAngle)
		{
			Vector2 distance = point - center;
			double angle = Math.Atan(distance.X / distance.Y);
			return Vector2.Distance(center, point) <= radius && angle >= startAngle && angle <= endAngle;
		}

		public static Vector2[] CreatePolygon(Vector2 dimensions, Vector2 origin = default(Vector2), float scale = 1f) => new[] { new Vector2(-origin.X, -origin.Y) * scale, new Vector2(dimensions.X - origin.X, -origin.Y) * scale, new Vector2(dimensions.X - origin.X, dimensions.Y - origin.Y) * scale, new Vector2(-origin.X, dimensions.Y - origin.Y) * scale };

		public static Vector2[] Transform(this Vector2[] polygon, Matrix matrix) => polygon.Select(point => Vector2.Transform(point, matrix)).ToArray();

		public static bool IsInPolygon4(this Vector2 point, params Vector2[] polygon)
		{
			bool result = false;
			int j = polygon.Length - 1;
			for (int i = 0; i < polygon.Length; i++)
			{
				if (polygon[i].Y < point.Y && polygon[j].Y >= point.Y || polygon[j].Y < point.Y && polygon[i].Y >= point.Y)
				{
					if (polygon[i].X + (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < point.X)
					{
						result = !result;
					}
				}

				j = i;
			}

			return result;
		}

		public static float ToRadians(this float angle) => MathHelper.Pi / 180f * angle;

		public static float ToDegrees(this float angle) => angle * 180f / MathHelper.Pi;

		public static T Remap<T>(this T value, double low1, double high1, double low2, double high2)
		{
			double castValue = (double)Convert.ChangeType(value, TypeCode.Double);
			return (T)Convert.ChangeType(low2 + (castValue - low1) * (high2 - low2) / (high1 - low1), value.GetType());
		}

		public static Vector2 ToScreenCoordinates(this Point16 point) => point.ToVector2() * 16 - Main.screenPosition;

		public static Vector2 WithOffscreenRange(this Vector2 vector) => vector + (Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange));

		public static Matrix OffscreenMatrix => Main.drawToScreen ? Matrix.CreateTranslation(0, 0, 0) : Matrix.CreateTranslation(Main.offScreenRange, Main.offScreenRange, 0);
	}
}