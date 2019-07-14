using BaseLibrary.UI.Elements;
using System.Diagnostics;
using Terraria;
using Terraria.ModLoader;

namespace BaseLibrary.UI
{
	public class UIIntroMessage : BaseState
	{
		public override void OnInitialize()
		{
			UIPanel panel = new UIPanel
			{
				Width = (0, 0.25f),
				Height = (0, 0.5f),
				Padding = (12, 12, 12, 12)
			};
			panel.Center();
			Append(panel);

			UIText introText = new UIText("A message from Itorius...\n\nHi, thanks for downloading my mods!\nIn the future this window will include changelogs\nand credits but for now please consider supporting\nme over at Patreon :)");
			panel.Append(introText);

			UITexture texture = new UITexture(ModContent.GetTexture("BaseLibrary/Textures/UI/PatreonLogo"), ScaleMode.Stretch)
			{
				Width = (120, 0),
				Height = (120, 0),
				HAlign = 0.5f,
				VAlign = 0.7f
			};
			texture.OnClick += (evt, element) =>
			{
				Process.Start("https://www.patreon.com/Itorius");
				Main.menuMode = 0;
			};
			texture.GetHoverText += () => "https://www.patreon.com/Itorius";
			panel.Append(texture);

			UITextButton button = new UITextButton("Return to main menu")
			{
				Width = (0, 1),
				Height = (40, 0),
				VAlign = 1
			};
			button.OnClick += (evt, element) => Main.menuMode = 0;
			panel.Append(button);
		}
	}
}