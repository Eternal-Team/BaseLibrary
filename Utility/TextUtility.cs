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
	}
}