using BaseLibrary.UI.Elements;
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
				Height = (0, 0.5f)
			};
			panel.Center();
			Append(panel);

			UITexture texture = new UITexture(ModContent.GetTexture("BaseLibrary/Textures/UI/PatreonLogo"))
			{
				Width = (120, 0),
				Height = (120, 0),
				HAlign = 0.5f,
				VAlign = 0.25f
			};
			panel.Append(texture);

			UIText text = new UIText("Give us money!", 2f)
			{
				HAlign = 0.5f,
				VAlign = 0.65f
			};
			panel.Append(text);

			UITextButton button = new UITextButton("Back")
			{
				Width = (0, 1),
				Height = (60, 0),
				VAlign = 1
			};
			button.OnClick += (s, b) => Main.menuMode = 0;
			panel.Append(button);
		}
	}
}