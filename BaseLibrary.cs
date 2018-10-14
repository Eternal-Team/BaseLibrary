using System.Collections.Generic;
using BaseLibrary.ModBook;
using BaseLibrary.UI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using static BaseLibrary.Utility.Utility;
using Player = On.Terraria.Player;

namespace BaseLibrary
{
	// todo: use caching for reflectionutility

	public class BaseLibrary : Mod
	{
		internal static BaseLibrary Instance;

		public static List<Item> itemsCache;

		public static bool InUI;
		private LegacyGameInterfaceLayer MouseInterface;
		public GUI<ModBookUI> BookUI;

		public static ModHotKey hotkeyOpenBook;

		public static (string key, RecipeGroup group) recipeGroupT2HMBars;

		public override void Load()
		{
			Instance = this;

			ModBookLoader.Load();

			Player.HandleHotbar += (orig, player) =>
			{
				if (!InUI) orig(player);
				InUI = false;
			};

			hotkeyOpenBook = RegisterHotKey("Open Mod Book", "M");

			if (!Main.dedServ)
			{
				this.LoadTextures();

				MouseInterface = new LegacyGameInterfaceLayer("BaseLibrary: MouseText", DrawMouseText, InterfaceScaleType.UI);
				BookUI = SetupGUI<ModBookUI>();
			}
		}

		public override void Unload()
		{
			ModBookLoader.Unload();
			UnloadNullableTypes();
		}

		public override void PostSetupContent()
		{
			itemsCache = new List<Item>();

			for (int type = 0; type < ItemLoader.ItemCount; type++)
			{
				Item item = new Item();
				item.SetDefaults(type, false);
				itemsCache.Add(item);
			}
		}

		public override void AddRecipeGroups()
		{
			recipeGroupT2HMBars = ("BaseLibrary:T2HMBars", new RecipeGroup(() => "T2HMBars", ItemID.MythrilBar, ItemID.OrichalcumBar));
			RecipeGroup.RegisterGroup(recipeGroupT2HMBars.key, recipeGroupT2HMBars.group);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			int HotbarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Hotbar"));

			if (MouseTextIndex != -1) layers.Insert(MouseTextIndex + 1, MouseInterface);
			if (HotbarIndex != -1) layers.Insert(HotbarIndex + 1, BookUI.InterfaceLayer);
		}
	}
}