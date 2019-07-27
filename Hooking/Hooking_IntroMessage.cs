using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace BaseLibrary
{
	public static partial class Hooking
	{
		private static UIIntroMessage uiIntroMessage;

		private static List<Mod> newOrUpdated = new List<Mod>();

		private static string LastVersionsPath => ModLoader.ModPath + "/LastVersionCache.json";

		public static void OnPostDraw(GameTime gameTime)
		{
			if (Main.menuMode == 4040)
			{
				Main.menuMode = 888;

				uiIntroMessage = new UIIntroMessage();
				UserInterface.ActiveInstance.SetState(uiIntroMessage);
			}
		}
	}
}