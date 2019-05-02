using System.Collections.Generic;
using System.IO;
using System.Linq;
using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace BaseLibrary
{
	public class BaseLibrary : Mod
	{
		internal static BaseLibrary Instance;

		private LegacyGameInterfaceLayer MouseInterface;

		public static GUI<PanelUI> PanelGUI;
		internal List<IHasUI> ClosedUICache = new List<IHasUI>();

		public override void Load()
		{
			Instance = this;

			Language.Load();

			Utility.Input.Load();
			Hooking.Load();

			Scheduler.Load();

			if (!Main.dedServ)
			{
				Utility.Font = Main.instance.OurLoad<DynamicSpriteFont>("Fonts" + Path.DirectorySeparatorChar + "Mouse_Text");
				typeof(DynamicSpriteFont).SetValue("_characterSpacing", 1f, Utility.Font);

				PanelGUI = Utility.SetupGUI<PanelUI>();

				MouseInterface = new LegacyGameInterfaceLayer("BaseLibrary: MouseText", Utility.DrawMouseText, InterfaceScaleType.UI);
			}
		}

		public override void Unload()
		{
			Utility.Input.Unload();

			Scheduler.Unload();

			Utility.UnloadNullableTypes();
		}

		public override void PostSetupContent() => Utility.Cache.Load();

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));

			if (MouseTextIndex != -1)
			{
				layers.Insert(MouseTextIndex + 1, MouseInterface);
				layers.Insert(MouseTextIndex + 1, PanelGUI.InterfaceLayer);
			}
		}

		public override void UpdateUI(GameTime gameTime)
		{
			if (!Main.playerInventory)
			{
				List<BaseUIPanel> bagPanels = PanelGUI.UI.Elements.Cast<BaseUIPanel>().ToList();
				for (int i = 0; i < bagPanels.Count; i++)
				{
					ClosedUICache.Add(bagPanels[i].Container);
					PanelGUI.UI.CloseUI(bagPanels[i].Container);
				}
			}
			else
			{
				for (int i = 0; i < ClosedUICache.Count; i++) PanelGUI.UI.OpenUI(ClosedUICache[i]);
				ClosedUICache.Clear();
			}

			PanelGUI?.Update(gameTime);
		}

		public override void PreSaveAndQuit()
		{
			PanelGUI?.UI.Elements.Clear();
		}
	}
}