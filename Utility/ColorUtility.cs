using System;
using Microsoft.Xna.Framework;

namespace BaseLibrary.Utility;

public class ColorUtility
{
	public static Color FromHSL(float h, float s, float l)
	{
		float r = l;
		float g = l;
		float b = l;

		float v = l <= 0.5f ? l * (1f + s) : l + s - l * s;

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

	public static Color FromHSV(float h, float s, float v)
	{
		h *= 6f;

		int i = (int)Math.Floor(h);
		float f = h - i;

		float p = 1f - s;
		float q = 1f - s * f;
		float t = 1f - s * (1 - f);

		Color RGB;
		if (i == 0) RGB = new Color(1, t, p);
		else if (i == 1) RGB = new Color(q, 1, p);
		else if (i == 2) RGB = new Color(p, 1, t);
		else if (i == 3) RGB = new Color(p, q, 1);
		else if (i == 4) RGB = new Color(t, p, 1);
		else RGB = new Color(1, p, q);

		RGB *= v;

		return RGB;
	}

	public static Vector3 ToHSV(Color rgb)
	{
		float r = rgb.R / 255f;
		float g = rgb.G / 255f;
		float b = rgb.B / 255f;

		float cMin = MathUtility.Min(r, g, b);
		float cMax = MathUtility.Max(r, g, b);

		float delta = cMax - cMin;

		float h = 0f;
		if (Math.Abs(cMax - r) < float.Epsilon)
			h = (g - b) / delta % 6;
		else if (Math.Abs(cMax - g) < float.Epsilon)
			h = (b - r) / delta + 2f;
		else if (Math.Abs(cMax - b) < float.Epsilon)
			h = (r - g) / delta + 4f;

		h = h * 60f / 360f;

		float s = cMax == 0f ? 0f : delta / cMax;

		return new Vector3(h, s, cMax);
	}

	public static Color Invert(Color color)
	{
		Vector3 x = ToHSV(color);
		return FromHSV((x.X + 0.5f) % 1, x.Y, x.Z);
	}
}