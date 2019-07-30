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
	public class BaseLibrary : Mod
	{
		internal static BaseLibrary Instance;

		public static Effect ColorSelectionShader { get; private set; }
		public static Effect DesaturateShader { get; private set; }
		internal static Texture2D texturePanelBackground;
		internal static Texture2D texturePanelBorder;

		public static GUI<PanelUI> PanelGUI { get; private set; }

		internal List<IHasUI> ClosedUICache = new List<IHasUI>();
		private LegacyGameInterfaceLayer MouseInterface;

		public override void Load()
		{
			Instance = this;

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

				PanelGUI = Utility.SetupGUI<PanelUI>();

				MouseInterface = new LegacyGameInterfaceLayer("BaseLibrary: MouseText", Utility.DrawMouseText, InterfaceScaleType.UI);
			}
		}

		public override void Unload()
		{
			Utility.Input.Unload();
			Dispatcher.Unload();

			Utility.UnloadNullableTypes();
		}

		public override void PostSetupContent() => Utility.Cache.Load();

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));

			if (MouseTextIndex != -1)
			{
				layers.Insert(MouseTextIndex + 1, MouseInterface);
				layers.Insert(MouseTextIndex, PanelGUI.InterfaceLayer);
			}
		}

		public override void UpdateUI(GameTime gameTime)
		{
			for (int i = 0; i < PanelGUI.UI.Elements.Count; i++)
			{
				UIElement element = PanelGUI.UI.Elements[i];
				if (element is BaseUIPanel panel && panel.Container is BaseTE tileEntity)
				{
					TileObjectData data = TileObjectData.GetTileData(tileEntity.mod.GetTile(tileEntity.TileType.Name).Type, 0);
					Vector2 offset = data != null ? new Vector2(data.Width * 8f, data.Height * 8f) : Vector2.Zero;

					if (Vector2.DistanceSquared(tileEntity.Position.ToWorldCoordinates(offset), Main.LocalPlayer.Center) > 160 * 160) PanelGUI.UI.CloseUI(panel.Container);
				}
			}

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

			PanelGUI?.Update(gameTime);
		}

		public override void PreSaveAndQuit()
		{
			ClosedUICache.Clear();
			foreach (UIElement element in PanelGUI.UI.Elements)
			{
				if (element is BaseUIPanel panel) PanelGUI.UI.CloseUI(panel.Container);
			}
		}
	}
}