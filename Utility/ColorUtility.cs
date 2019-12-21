using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Terraria;

namespace BaseLibrary
{
	public static partial class Utility
	{
		public static readonly Color ColorPanel = new Color(73, 94, 171);
		public static readonly Color ColorPanel_Selected = new Color(51, 65, 119);
		public static readonly Color ColorPanel_Hovered = new Color(94, 120, 221);
		public static readonly Color ColorSlot = new Color(63, 65, 151);
		public static readonly Color ColorOutline = new Color(18, 18, 38);

		public static readonly Color[] ColorCoins =
		{
			new Color(246, 138, 96),
			new Color(181, 192, 193),
			new Color(224, 201, 92),
			new Color(220, 220, 198),
			new Color(120, 120, 120)
		};

		public static Color RandomColor() => new Color(Main.rand.NextFloat(), Main.rand.NextFloat(), Main.rand.NextFloat());

		public static Color ToColor(this int value) => new Color((value >> 16) & 255, (value >> 8) & 255, value & 255, (value >> 24) & 255);

		public static Color ToColor(this string s) => int.Parse(s.Replace("#", ""), NumberStyles.HexNumber).ToColor();

		public static int ToInt(this Color color) => (color.A << 24) | (color.R << 16) | (color.G << 8) | color.B;

		public static string ToHex(this Color color) => $"{color.R:X2}{color.G:X2}{color.B:X2}";

		public static Color DoubleLerp(Color c1, Color c2, Color c3, float step) => step < 0.5f ? Color.Lerp(c1, c2, step * 2f) : Color.Lerp(c2, c3, (step - 0.5f) * 2f);

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

		public static Vector3 HSV2RGB(Vector3 HSV)
		{
			Vector3 RGB = new Vector3(HSV.Z);

			float h = HSV.X / 60f;
			float s = HSV.Y;
			float v = HSV.Z;

			int i = (int)Math.Floor(h);
			float f = h - i;

			float p = 1f - s;
			float q = 1f - s * f;
			float t = 1f - s * (1 - f);

			if (i == 0) RGB = new Vector3(1, t, p);
			else if (i == 1) RGB = new Vector3(q, 1, p);
			else if (i == 2) RGB = new Vector3(p, 1, t);
			else if (i == 3) RGB = new Vector3(p, q, 1);
			else if (i == 4) RGB = new Vector3(t, p, 1);
			else RGB = new Vector3(1, p, q);

			RGB *= v;

			return RGB;
		}

		public static Color Invert(this Color color) => new Color(255 - color.R, 255 - color.G, 255 - color.B);

		public static Color AverageColor(this IEnumerable<Color> colors)
		{
			List<Color> list = colors.ToList();

			int r = 0, g = 0, b = 0, a = 0;
			foreach (Color color in list)
			{
				r += color.R;
				g += color.G;
				b += color.B;
				a += color.A;
			}

			int count = list.Count;
			return new Color(r / count, g / count, b / count, a / count);
		}
	}
}