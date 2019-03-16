using System;
using System.Globalization;
using Microsoft.Xna.Framework;

namespace BaseLibrary
{
	public static partial class Utility
	{
		public static readonly Color ColorPanel = new Color(73, 94, 171);
		public static readonly Color ColorPanel_Selected = new Color(51, 65, 119);
		public static readonly Color ColorPanel_Hovered = new Color(94, 120, 221);
		public static readonly Color ColorSlot = new Color(63, 65, 151);

		public static readonly Color[] ColorCoins =
		{
			new Color(246, 138, 96),
			new Color(181, 192, 193),
			new Color(224, 201, 92),
			new Color(220, 220, 198),
			new Color(120, 120, 120)
		};

		public static Color ToColor(this int value) => new Color((value >> 16) & 255, (value >> 8) & 255, (value >> 0) & 255, (value >> 24) & 255);

		public static Color ToColor(this string s) => int.Parse(s.Replace("#", ""), NumberStyles.HexNumber).ToColor();

		public static int ToInt(this Color color) => (color.A << 24) | (color.R << 16) | (color.G << 8) | (color.B << 0);

		public static string ToHex(this Color color) => $"{color.R:X2}{color.G:X2}{color.B:X2}";

		public static Color DoubleLerp(Color c1, Color c2, Color c3, float step) => step < .5f ? Color.Lerp(c1, c2, step * 2f) : Color.Lerp(c2, c3, (step - .5f) * 2f);

		public static Color HSL2RGB(float h, float sl, float l)
		{
			float r = l;
			float g = l;
			float b = l;

			float v = l <= 0.5f ? l * (1f + sl) : l + sl - l * sl;

			if (v > 0)
			{
				float m = l + l - v;
				float sv = (v - m) / v;
				h *= 6f;
				int sextant = (int)h;
				float fract = h - sextant;
				float vsf = v * sv * fract;
				float mid1 = m + vsf;
				float mid2 = v - vsf;

				if (h < 1f || Math.Abs(h - 6f) < 0.0)
				{
					r = v;
					g = mid1;
					b = m;
				}
				else if (h < 2f)
				{
					r = mid2;
					g = v;
					b = m;
				}
				else if (h < 3f)
				{
					r = m;
					g = v;
					b = mid1;
				}
				else if (h < 4f)
				{
					r = m;
					g = mid2;
					b = v;
				}
				else if (h < 5f)
				{
					r = mid1;
					g = m;
					b = v;
				}
				else if (h < 6f)
				{
					r = v;
					g = m;
					b = mid2;
				}
			}

			return new Color(r, g, b);
		}
	}
}