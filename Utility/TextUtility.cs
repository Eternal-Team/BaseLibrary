using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BaseLibrary.Utility;

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

	// public static string Get(this ModTranslation translation) => translation.GetTranslation(Language.ActiveCulture);

	public static IEnumerable<int> Search(string text, string pattern)
	{
		int M = pattern.Length;
		int N = text.Length;

		int[] lps = new int[M];
		int j = 0;

		computeLPSArray();

		int i = 0;
		while (i < N)
		{
			if (pattern[j] == text[i])
			{
				j++;
				i++;
			}

			if (j == M)
			{
				yield return i - j;
				j = lps[j - 1];
			}
			else if (i < N && pattern[j] != text[i])
			{
				if (j != 0) j = lps[j - 1];
				else i += 1;
			}
		}

		void computeLPSArray()
		{
			int len = 0;
			int index = 1;
			lps[0] = 0;

			while (index < M)
			{
				if (pattern[index] == pattern[len])
				{
					len++;
					lps[index] = len;
					index++;
				}
				else
				{
					if (len != 0)
					{
						len = lps[len - 1];
					}
					else
					{
						lps[index] = len;
						index++;
					}
				}
			}
		}
	}
}