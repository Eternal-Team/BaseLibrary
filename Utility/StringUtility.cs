using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using System.Linq;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BaseLibrary
{
	public static partial class Utility
	{
		private static Regex _colorGetText;
		private static Regex ColorGetText => _colorGetText ?? (_colorGetText = new Regex(@"(?<=\[c\/\w{6}:)[^]]*(?=\])"));

		private static Regex _colorGetTag;
		private static Regex ColorGetTag => _colorGetTag ?? (_colorGetTag = new Regex(@"\[c\/\w{6}:[^]]*\]"));

		public static DynamicSpriteFont Font { get; internal set; }

		public static string ReplaceTagWithText(Match m) => ColorGetText.Match(ColorGetTag.Match(m.Value).Value).Value;

		public static string ExtractText(string withTag) => ColorGetTag.Replace(withTag, ReplaceTagWithText);

		public static Vector2 Measure(this string text, DynamicSpriteFont font = null) => (font ?? Main.fontMouseText).MeasureString(text);

		public static string Subscript(this int number)
		{
			var intList = number.ToString().Select(digit => int.Parse(digit.ToString()));
			return intList.Aggregate("", (current, i) => current + ("\\u832" + i));
		}

		public static string GetTranslation(this ModTranslation translation, GameCulture culture = null) => translation.GetTranslation(culture ?? Language.ActiveCulture);
	}
}