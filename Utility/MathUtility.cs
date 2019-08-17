using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;

namespace BaseLibrary
{
	public static partial class Utility
	{
		private static Lazy<char[]> incSuffixes = new Lazy<char[]>(() => new[] { 'k', 'M', 'G', 'T', 'P', 'E', 'Z', 'Y' });
		private static readonly char[] IncSuffixes = incSuffixes.Value;

		private static Lazy<char[]> decSuffixes = new Lazy<char[]>(() => new[] { 'm', '\u03bc', 'n', 'p', 'f', 'a', 'z', 'y' });
		private static readonly char[] DecSuffixes = decSuffixes.Value;

		public static string ToSI<T>(this T value, string format = null)
		{
			if (value == null) throw new ArgumentNullException();

			double castValue = (double)Convert.ChangeType(value, TypeCode.Double);

			if (Math.Abs(castValue) > 0.0)
			{
				int degree = (int)Math.Floor(Math.Log10(Math.Abs(castValue)) / 3);
				double scaled = castValue * Math.Pow(1000, -degree);

				char? suffix = null;
				switch (Math.Sign(degree))
				{
					case 1:
						suffix = IncSuffixes[degree - 1];
						break;
					case -1:
						suffix = DecSuffixes[-degree - 1];
						break;
				}

				return scaled.ToString(format) + suffix;
			}

			return castValue.ToString(format);
		}

		public static Point16 Min(this Point16 point, Point16 compareTo) => new Point16(point.X > compareTo.X ? compareTo.X : point.X, point.Y > compareTo.Y ? compareTo.Y : point.Y);

		public static T Min<T>(params T[] values) => values.Min();

		public static T Max<T>(params T[] values) => values.Max();

		public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T>
		{
			if (value.CompareTo(max) > 0) return max;
			return value.CompareTo(min) < 0 ? min : value;
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

		public static bool InRange(this double value, double min, double max) => value >= min && value <= max;

		public static float ToRadians(this float angle) => MathHelper.Pi / 180f * angle;
		public static double ToRadians(this double angle) => Math.PI / 180.0 * angle;

		public static float ToDegrees(this float angle) => angle * 180f / MathHelper.Pi;
		public static double ToDegrees(this double angle) => angle * 180.0 / Math.PI;

		public static T Remap<T>(this T value, double fromLow, double fromHigh, double toLow, double toHigh)
		{
			double castValue = (double)Convert.ChangeType(value, TypeCode.Double);
			return (T)Convert.ChangeType(toLow + (castValue - fromLow) * (toHigh - toLow) / (fromHigh - fromLow), value.GetType());
		}

		public static Vector2 ToScreenCoordinates(this Point point, bool addOffscreenRange = true) => point.ToVector2() * 16 - Main.screenPosition + (Main.drawToScreen || !addOffscreenRange ? Vector2.Zero : new Vector2(Main.offScreenRange));

		public static Vector2 ToScreenCoordinates(this Point16 point, bool addOffscreenRange = true) => point.ToVector2() * 16 - Main.screenPosition + (Main.drawToScreen || !addOffscreenRange ? Vector2.Zero : new Vector2(Main.offScreenRange));
	}
}