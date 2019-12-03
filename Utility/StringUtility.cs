using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Terraria;

namespace BaseLibrary
{
	public static partial class Utility
	{
		private static Regex _colorGetText;
		private static Regex ColorGetText => _colorGetText = _colorGetText ?? new Regex(@"(?<=\[c\/\w{6}:)[^]]*(?=\])");

		private static Regex _colorGetTag;
		private static Regex ColorGetTag => _colorGetTag = _colorGetTag ?? new Regex(@"\[c\/\w{6}:[^]]*\]");

		public static DynamicSpriteFont Font { get; internal set; }

		public static string ReplaceTagWithText(Match m) => ColorGetText.Match(ColorGetTag.Match(m.Value).Value).Value;

		public static string ExtractText(string withTag) => ColorGetTag.Replace(withTag, ReplaceTagWithText);

		public static Vector2 Measure(this string text, DynamicSpriteFont font = null) => (font ?? Main.fontMouseText).MeasureString(text) - new Vector2(text.Count(x => x == ' ') * 2, 0);

		public static IEnumerable<string> WrapText(string text, float width, DynamicSpriteFont font = null)
		{
			if (font == null) font = Main.fontMouseText;
			StringBuilder actualLine = new StringBuilder();
			float actualWidth = 0;

			text = text.Replace("\r\n", "\n");

			foreach (string newLine in text.Split('\n'))
			{
				string[] split = newLine.Split(' ');
				for (var i = 0; i < split.Length; i++)
				{
					string item = split[i];
					if (i != split.Length - 1) item += " ";

					float itemWidth = font.MeasureString(item).X;
					if (actualWidth + itemWidth > width && actualLine.Length > 0)
					{
						yield return actualLine.ToString();
						actualLine.Clear();
						actualWidth = 0;
					}

					actualLine.Append(item);
					actualWidth += itemWidth;
				}

				yield return actualLine.ToString();
				actualLine.Clear();
				actualWidth = 0;
			}

			if (actualLine.Length > 0) yield return actualLine.ToString();
		}
	}
}