using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BaseLibrary.Utility
{
	public static partial class Utility
	{
		public static readonly Regex colorGetText = new Regex(@"(?<=\[c\/\w{6}:)[^]]*(?=\])");
		public static readonly Regex colorGetTag = new Regex(@"\[c\/\w{6}:[^]]*\]");
		public static string ReplaceTagWithText(Match m) => colorGetText.Match(colorGetTag.Match(m.Value).Value).Value;
		public static string ExtractText(string withTag) => colorGetTag.Replace(withTag, ReplaceTagWithText);
		public static Vector2 Measure(this string text, DynamicSpriteFont font = null) => (font ?? Main.fontMouseText).MeasureString(text);

		public static string Subscript(this int number)
		{
			var intList = number.ToString().Select(digit => int.Parse(digit.ToString()));
			return intList.Aggregate("", (current, i) => current + ("\\u832" + i));
		}

		public static LocalizedText TextFromTranslation(this ModTranslation translation)
		{
			Type type = typeof(LocalizedText);
			return (LocalizedText)type.Assembly.CreateInstance(type.FullName, false, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { translation.Key, translation.GetTranslation(Language.ActiveCulture.LegacyId) }, null, null);
		}
	}
}