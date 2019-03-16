using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria;

namespace BaseLibrary
{
	public static partial class Utility
	{
		private static readonly Regex colorGetText = new Regex(@"(?<=\[c\/\w{6}:)[^]]*(?=\])");
		private static readonly Regex colorGetTag = new Regex(@"\[c\/\w{6}:[^]]*\]");

		public static DynamicSpriteFont Font { get; internal set; }

		public static string ReplaceTagWithText(Match m) => colorGetText.Match(colorGetTag.Match(m.Value).Value).Value;

		public static string ExtractText(string withTag) => colorGetTag.Replace(withTag, ReplaceTagWithText);

		public static Vector2 Measure(this string text, DynamicSpriteFont font = null) => (font ?? Main.fontMouseText).MeasureString(text);

		public static string Subscript(this int number)
		{
			var intList = number.ToString().Select(digit => int.Parse(digit.ToString()));
			return intList.Aggregate("", (current, i) => current + ("\\u832" + i));
		}
	}
}