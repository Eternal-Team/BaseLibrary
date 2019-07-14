using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace BaseLibrary
{
	public class BaseLibrary : Mod
	{
		internal static BaseLibrary Instance;
		public static GUI<PanelUI> PanelGUI;
		internal static GUI<TestUI> TestGUI;

		internal List<IHasUI> ClosedUICache = new List<IHasUI>();

		private LegacyGameInterfaceLayer MouseInterface;

		public override void Load()
		{
			Instance = this;

			Utility.Input.Load();
			Hooking.Load();
			Scheduler.Load();

			if (!Main.dedServ)
			{
				Utility.Font = Main.instance.OurLoad<DynamicSpriteFont>("Fonts" + Path.DirectorySeparatorChar + "Mouse_Text");
				typeof(DynamicSpriteFont).SetValue("_characterSpacing", 1f, Utility.Font);

				PanelGUI = Utility.SetupGUI<PanelUI>();
				TestGUI = Utility.SetupGUI<TestUI>();

				MouseInterface = new LegacyGameInterfaceLayer("BaseLibrary: MouseText", Utility.DrawMouseText, InterfaceScaleType.UI);
			}
		}

		public override void Unload()
		{
			Hooking.Unload();

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
				//layers.Insert(MouseTextIndex + 1, TestGUI.InterfaceLayer);
				layers.Insert(MouseTextIndex , PanelGUI.InterfaceLayer);
			}
		}

		public override void UpdateUI(GameTime gameTime)
		{
			if (!Main.playerInventory)
			{
				List<BaseUIPanel> bagPanels = PanelGUI.UI.Elements.Cast<BaseUIPanel>().ToList();
				foreach (BaseUIPanel ui in bagPanels)
				{
					ClosedUICache.Add(ui.Container);
					PanelGUI.UI.CloseUI(ui.Container);
				}
			}
			else
			{
				foreach (IHasUI ui in ClosedUICache) PanelGUI.UI.OpenUI(ui);

				ClosedUICache.Clear();
			}

			//TestGUI?.Update(gameTime);
			PanelGUI?.Update(gameTime);
		}

		public override void PreSaveAndQuit()
		{
			foreach (UIElement element in PanelGUI.UI.Elements)
			{
				if (element is BaseUIPanel panel) PanelGUI.UI.CloseUI(panel.Container);
			}
		}
	}
}