using BaseLibrary.Tiles.TileEntites;
using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using Terraria.UI;

namespace BaseLibrary
{
	// note: update checker via github releases

	public class BaseLibrary : Mod
	{
		internal static List<IHasUI> ClosedUICache = new List<IHasUI>();

		public static Effect ColorSelectionShader { get; private set; }
		public static Effect DesaturateShader { get; private set; }
		public static Effect RoundedRectShader { get; private set; }

		public static Texture2D texturePanelBackground;
		public static Texture2D texturePanelBorder;

		public static GUI<PanelUI> PanelGUI { get; private set; }
		internal static GUI<TestUI> TestGUI { get; private set; }

		private LegacyGameInterfaceLayer MouseInterface;

		public override void Load()
		{
			TagSerializer.AddSerializer(new GUIDSerializer());

			Dispatcher.Load();
			Utility.Input.Load();
			Hooking.Load();

			if (!Main.dedServ)
			{
				texturePanelBackground = ModContent.GetTexture("Terraria/UI/PanelBackground");
				texturePanelBorder = ModContent.GetTexture("Terraria/UI/PanelBorder");

				Utility.Font = GetFont("Fonts/Mouse_Text");
				typeof(DynamicSpriteFont).SetValue("_characterSpacing", 1f, Utility.Font);

				ColorSelectionShader = GetEffect("Effects/ColorSelectionShader");
				DesaturateShader = GetEffect("Effects/DesaturateShader");
				RoundedRectShader = GetEffect("Effects/BorderRadius");

				PanelGUI = Utility.SetupGUI<PanelUI>();
				TestGUI = Utility.SetupGUI<TestUI>();

				MouseInterface = new LegacyGameInterfaceLayer("BaseLibrary: MouseText", Utility.DrawMouseText, InterfaceScaleType.UI);
			}
		}

		public override void Unload()
		{
			Dispatcher.Unload();

			this.UnloadNullableTypes();
		}

		public override void PostSetupContent() => Utility.Cache.Load();

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));

			if (MouseTextIndex != -1)
			{
				layers.Insert(MouseTextIndex + 1, MouseInterface);
				layers.Insert(MouseTextIndex, PanelGUI.InterfaceLayer);
				//layers.Insert(MouseTextIndex, TestGUI.InterfaceLayer);
			}
		}

		public override void UpdateUI(GameTime gameTime)
		{
			var gui = PanelGUI;

			for (int i = 0; i < gui.Elements.Count; i++)
			{
				UIElement element = gui.Elements[i];
				if (element is BaseUIPanel panel && panel.Container is BaseTE tileEntity)
				{
					TileObjectData data = TileObjectData.GetTileData(tileEntity.mod.GetTile(tileEntity.TileType.Name).Type, 0);
					Vector2 offset = data != null ? new Vector2(data.Width * 8f, data.Height * 8f) : Vector2.Zero;

					if (Vector2.DistanceSquared(tileEntity.Position.ToWorldCoordinates(offset), Main.LocalPlayer.Center) > 160 * 160) gui.UI.CloseUI(panel.Container);
				}
			}

			if (!Main.playerInventory)
			{
				List<BaseUIPanel> bagPanels = gui.Elements.Cast<BaseUIPanel>().ToList();
				foreach (BaseUIPanel ui in bagPanels)
				{
					ClosedUICache.Add(ui.Container);
					gui.UI.CloseUI(ui.Container);
				}
			}
			else
			{
				foreach (IHasUI ui in ClosedUICache) gui.UI.OpenUI(ui);

				ClosedUICache.Clear();
			}

			gui?.Update(gameTime);
			//TestGUI?.Update(gameTime);
		}

		public override void PreSaveAndQuit()
		{
			ClosedUICache.Clear();
			PanelGUI.UI.CloseAllUIs();
		}
	}
}