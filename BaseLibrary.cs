using System.Collections.Generic;
using BaseLibrary.InputInterceptor;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using static BaseLibrary.Utility.Utility;

namespace BaseLibrary
{
	// todo: use caching for reflectionutility

	public class BaseLibrary : Mod
	{
		internal static BaseLibrary Instance;

		public static List<Item> itemCache;
		public static List<NPC> npcCache;
		public static List<Projectile> projectileCache;

		public static bool InUI;

		private LegacyGameInterfaceLayer MouseInterface;
		//public GUI<ModBookUI> BookUI;

		public override void Load()
		{
			Instance = this;

			//ModBookLoader.Load();

			Hooking.Hooking.Initialize();

			Scheduler.Load();
			InputInterceptor.InputInterceptor.Load();
			
			On.Terraria.Main.DoUpdate_HandleInput += (orig, self) =>
			{
				if (!InputInterceptor.InputInterceptor.InterceptInput()) orig(self);
			};

			if (!Main.dedServ)
			{
				this.LoadTextures();

				MouseInterface = new LegacyGameInterfaceLayer("BaseLibrary: MouseText", DrawMouseText, InterfaceScaleType.UI);
				//BookUI = SetupGUI<ModBookUI>();
			}
		}

		void HandleInput()
		{

		}

		public override void Unload()
		{
			InputInterceptor.InputInterceptor.Unload();
			Scheduler.Unload();

			//ModBookLoader.Unload();
			UnloadNullableTypes();
		}

		public override void PostSetupContent()
		{
			itemCache = new List<Item>();
			npcCache = new List<NPC>();
			projectileCache = new List<Projectile>();

			for (int type = 0; type < ItemLoader.ItemCount; type++)
			{
				Item item = new Item();
				item.SetDefaults(type, false);
				itemCache.Add(item);
			}

			for (int type = 0; type < NPCLoader.NPCCount; type++)
			{
				NPC npc = new NPC();
				npc.SetDefaults(type);
				npcCache.Add(npc);
			}

			for (int type = 0; type < ProjectileLoader.ProjectileCount; type++)
			{
				try
				{
					Projectile projectile = new Projectile();
					projectile.SetDefaults(type);
					projectileCache.Add(projectile);
				}
				catch
				{
				}
			}
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			//int HotbarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Hotbar"));

			if (MouseTextIndex != -1) layers.Insert(MouseTextIndex + 1, MouseInterface);
			//if (HotbarIndex != -1) layers.Insert(HotbarIndex + 1, BookUI.InterfaceLayer);
		}

		public override void UpdateUI(GameTime gameTime)
		{
		}
	}
}