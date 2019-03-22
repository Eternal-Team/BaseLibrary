using System.Collections.Generic;
using System.IO;
using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace BaseLibrary
{
	// todo: abstract Starbound.Input behind my own code

	public class BaseLibrary : Mod
	{
		internal static BaseLibrary Instance;
		
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
				Utility.TexturePanelBackground = ModContent.GetTexture("Terraria/UI/PanelBackground");
				Utility.TexturePanelBorder = ModContent.GetTexture("Terraria/UI/PanelBorder");

				Utility.Font = Main.instance.OurLoad<DynamicSpriteFont>("Fonts" + Path.DirectorySeparatorChar + "Mouse_Text");
				typeof(DynamicSpriteFont).SetValue("_characterSpacing", 1f, Utility.Font);

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

		public override void PostSetupContent() => Utility.Cache.Load();

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