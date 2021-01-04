using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace BaseLibrary.Utility
{
	public static class TextUtility
	{
		public static string WithColor(string s, Color color)
		{
			return $"[c/{color.Hex3()}:{s}]";
		}

		private static char[] IncSuffixes;
		private static char[] DecSuffixes;

		public static string ToSI(double value, string format = null)
		{
			if (IncSuffixes == null) IncSuffixes = new[] { 'k', 'M', 'G', 'T', 'P', 'E', 'Z', 'Y' };
			if (DecSuffixes == null) DecSuffixes = new[] { 'm', '\u03bc', 'n', 'p', 'f', 'a', 'z', 'y' };

			if (Math.Abs(value) > 0.0)
			{
				int degree = (int)Math.Floor(Math.Log10(Math.Abs(value)) / 3);
				double scaled = value * Math.Pow(1000, -degree);

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

			return value.ToString(format);
		}

		public static string ToSI(float value, string format = null) => ToSI((double)value, format);

		public static string ToSI(int value, string format = null) => ToSI((double)value, format);
	}
}