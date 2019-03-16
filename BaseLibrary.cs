using System.Collections.Generic;
using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace BaseLibrary
{
	public class BaseLibrary : Mod
	{
		internal static BaseLibrary Instance;

		public static List<Item> itemCache;
		public static List<NPC> npcCache;
		public static List<Projectile> projectileCache;

		private LegacyGameInterfaceLayer MouseInterface;

		private GUI<TestUI> ui;

		public override void Load()
		{
			Instance = this;

			Language.Load();

			Utility.Input.Load();
			Hooking.Load();

			Scheduler.Load();
			
			if (!Main.dedServ)
			{
				ui = Utility.SetupGUI<TestUI>();
				ui.Visible = true;

				MouseInterface = new LegacyGameInterfaceLayer("BaseLibrary: MouseText", Utility.DrawMouseText, InterfaceScaleType.UI);
			}
		}

		public override void Unload()
		{
			Utility.Input.Unload();

			Scheduler.Unload();

			Utility.UnloadNullableTypes();
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
				Projectile projectile = new Projectile();
				projectile.SetDefaults(type);
				projectileCache.Add(projectile);
			}
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));

			if (MouseTextIndex != -1)
			{
				layers.Insert(MouseTextIndex + 1, MouseInterface);
				layers.Insert(MouseTextIndex + 1, ui.InterfaceLayer);
			}
		}

		public override void UpdateUI(GameTime gameTime)
		{
			ui.Update(gameTime);
		}
	}
}