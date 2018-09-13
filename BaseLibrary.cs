using System;
using System.Collections.Generic;
using System.Linq;
using BaseLibrary.ModBook;
using BaseLibrary.Utility.Swapping;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using static BaseLibrary.Utility.Utility;

namespace BaseLibrary
{
	public class BaseLibrary : Mod
	{
		private LegacyGameInterfaceLayer MouseInterface;

		//private GUI<ModBookUI> BookUI;
		private ModBookUI UI;
		private UserInterface userInterface;
		private LegacyGameInterfaceLayer InterfaceLayer;

		public override void Load()
		{
			this.LoadTextures();
			ModBookLoader.Load();

			Swap(typeof(Player), "HandleHotbar", typeof(SwappingHotbar), "HandleHotbar");

			MouseInterface = new LegacyGameInterfaceLayer("BaseLibrary: MouseText", DrawMouseText, InterfaceScaleType.UI);

			#region temp
			userInterface = new UserInterface();
			UI = Activator.CreateInstance<ModBookUI>();
			UI.Activate();
			userInterface.SetState(UI);
			InterfaceLayer = new LegacyGameInterfaceLayer("BaseLibrary: ModBook", delegate
			{
				userInterface.Update(Main._drawInterfaceGameTime);
				UI.Draw(Main.spriteBatch);
				return true;
			});
			UI.Load(ModBookLoader.modBooks.First().Value);
			#endregion

			//BookUI = SetupGUI<ModBookUI>();
		}

		public override void Unload()
		{
			ModBookLoader.Unload();
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			int HotbarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Hotbar"));

			if (MouseTextIndex != -1) layers.Insert(MouseTextIndex + 1, MouseInterface);
			if (HotbarIndex != -1) layers.Insert(HotbarIndex + 1, InterfaceLayer);
		}
	}
}