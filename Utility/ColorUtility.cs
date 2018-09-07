using System.Globalization;
using Microsoft.Xna.Framework;

namespace BaseLibrary.Utility
{
	public static partial class Utility
	{
		public static readonly Color PanelColor_Selected = new Color(51, 65, 119);
		public static readonly Color PanelColor_Hover = new Color(94, 120, 221);
		public static readonly Color PanelColor = new Color(73, 94, 171);

		public static readonly Color[] CoinColors =
		{
			new Color(246, 138, 96),
			new Color(181, 192, 193),
			new Color(224, 201, 92),
			new Color(220, 220, 198),
			new Color(120, 120, 120)
		};

		public static Color ColorFromInt(this int value) => new Color((value >> 16) & 255, (value >> 8) & 255, (value >> 0) & 255, (value >> 24) & 255);

		public static int ColorToInt(this Color color) => (color.A << 24) | (color.R << 16) | (color.G << 8) | (color.B << 0);

		public static string ColorToHex(this Color color) => $"{color.R:X2}{color.G:X2}{color.B:X2}";

		public static Color ColorFromHex(this string s) => int.Parse(s.Replace("#", ""), NumberStyles.HexNumber).ColorFromInt();

		public static Color DoubleLerp(Color c1, Color c2, Color c3, float step) => step < .5f ? Color.Lerp(c1, c2, step * 2f) : Color.Lerp(c2, c3, (step - .5f) * 2f);
	}
}