using System.Collections.Generic;
using BaseLibrary.ModBook;
using BaseLibrary.UI;
using On.Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using static BaseLibrary.Utility.Utility;
using Main = Terraria.Main;

namespace BaseLibrary
{
	// todo: use caching for reflectionutility

	public class BaseLibrary : Mod
	{
		internal static BaseLibrary Instance;

		internal static bool InUI;
		private LegacyGameInterfaceLayer MouseInterface;
		private GUI<ModBookUI> BookUI;

		public override void Load()
		{
			Instance = this;

			ModBookLoader.Load();

			Player.HandleHotbar += Player_HandleHotbar;

			if (!Main.dedServ)
			{
				this.LoadTextures();

				MouseInterface = new LegacyGameInterfaceLayer("BaseLibrary: MouseText", DrawMouseText, InterfaceScaleType.UI);
				BookUI = SetupGUI<ModBookUI>();
			}
		}

		private void Player_HandleHotbar(Player.orig_HandleHotbar orig, Terraria.Player player)
		{
			if (InUI) return;
			orig(player);
		}

		public override void Unload()
		{
			ModBookLoader.Unload();
			UnloadNullableTypes();
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			int HotbarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Hotbar"));

			if (MouseTextIndex != -1) layers.Insert(MouseTextIndex + 1, MouseInterface);
			//if (HotbarIndex != -1) layers.Insert(HotbarIndex + 1, BookUI.InterfaceLayer);
		}
	}
}