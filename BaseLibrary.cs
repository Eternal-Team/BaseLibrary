using BaseLibrary.Input;
using BaseLibrary.Input.Mouse;
using BaseLibrary.Tiles;
using BaseLibrary.Tiles.TileEntites;
using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
	// todo: update checker via github releases

	public class BaseLibraryLayer : Layer
	{
		public override void OnClick(MouseButtonEventArgs args)
		{
			if (args.Button != MouseButton.Left || !Main.LocalPlayer.GetHeldItem().IsAir) return;

			args.Handled = true;

			int type = Main.tile[Player.tileTargetX, Player.tileTargetY].type;
			ModTile modTile = TileLoader.GetTile(type);
			if (modTile != null && modTile is BaseTile baseTile) baseTile.LeftClick(Player.tileTargetX, Player.tileTargetY);
		}
	}

	public class BaseLibrary : Mod
	{
		internal static List<IHasUI> ClosedUICache = new List<IHasUI>();
		public static LayerStack Layers;

		public static Effect ColorSelectionShader { get; private set; }
		public static Effect DesaturateShader { get; private set; }
		public static Effect RoundedRectShader { get; private set; }

		public static Texture2D texturePanelBackground;
		public static Texture2D texturePanelBorder;

		private LegacyGameInterfaceLayer MouseInterface;
		internal static ModHotKey hotkey;

		public override void Load()
		{
			TagSerializer.AddSerializer(new GUIDSerializer());

			Dispatcher.Load();
			Hooking.Load();

			if (!Main.dedServ)
			{
				texturePanelBackground = ModContent.GetTexture("Terraria/UI/PanelBackground");
				texturePanelBorder = ModContent.GetTexture("Terraria/UI/PanelBorder");

				hotkey = RegisterHotKey("Test hotkey", Keys.OemOpenBrackets.ToString());

				Utility.Font = GetFont("Fonts/Mouse_Text");
				typeof(DynamicSpriteFont).SetValue("_characterSpacing", 1f, Utility.Font);

				ColorSelectionShader = GetEffect("Effects/ColorSelectionShader");
				DesaturateShader = GetEffect("Effects/DesaturateShader");
				RoundedRectShader = GetEffect("Effects/BorderRadius");

				Layers = new LayerStack();
				Layers.PushLayer(new TerrariaLayer());
				Layers.PushLayer(new BaseLibraryLayer());
				Layers.PushOverlay(new UILayer());

				MouseInterface = new LegacyGameInterfaceLayer("BaseLibrary: MouseText", Utility.DrawMouseText, InterfaceScaleType.UI);
			}
		}

		public override void Unload()
		{
			Dispatcher.Unload();

			this.UnloadNullableTypes();
		}

		public override void PostSetupContent()
		{
			Input.Input.Load();
			Utility.Cache.Load();
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));

			if (MouseTextIndex != -1)
			{
				layers.Insert(MouseTextIndex + 1, MouseInterface);
				layers.Insert(MouseTextIndex, new LegacyGameInterfaceLayer("Layers", delegate
				{
					foreach (Layer layer in Layers)
					{
						if (layer.Enabled) layer.OnDraw(Main.spriteBatch);
					}

					return true;
				}, InterfaceScaleType.UI));
			}
		}

		public override void UpdateUI(GameTime gameTime)
		{
			var gui = PanelUI.Instance;

			for (int i = 0; i < gui.Children.Count; i++)
			{
				BaseElement element = gui.Children[i];
				if (element is BaseUIPanel panel && panel.Container is BaseTE tileEntity)
				{
					TileObjectData data = TileObjectData.GetTileData(tileEntity.mod.GetTile(tileEntity.TileType.Name).Type, 0);
					Vector2 offset = data != null ? new Vector2(data.Width * 8f, data.Height * 8f) : Vector2.Zero;

					if (Vector2.DistanceSquared(tileEntity.Position.ToWorldCoordinates(offset), Main.LocalPlayer.Center) > 160 * 160) gui.CloseUI(panel.Container);
				}
			}

			if (!Main.playerInventory)
			{
				List<BaseUIPanel> panels = gui.Children.Cast<BaseUIPanel>().ToList();
				foreach (BaseUIPanel ui in panels)
				{
					ClosedUICache.Add(ui.Container);
					gui.CloseUI(ui.Container);
				}
			}
			else
			{
				foreach (IHasUI ui in ClosedUICache) gui.OpenUI(ui);

				ClosedUICache.Clear();
			}

			foreach (Layer layer in Layers)
				if (layer.Enabled)
					layer.OnUpdate(gameTime);
		}

		public override void PreSaveAndQuit()
		{
			ClosedUICache.Clear();

			PanelUI.Instance.CloseAllUIs();
		}
	}
}