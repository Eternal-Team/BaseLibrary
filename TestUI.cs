using BaseLibrary.UI;
using BaseLibrary.UI.Elements;
using Microsoft.Xna.Framework;
using Terraria;

namespace BaseLibrary
{
	public class TestUI : BaseUI
	{
		private Ref<string> text = new Ref<string>("");

		public override void Initialize()
		{
			panelMain = new UIDraggablePanel
			{
				Width = (0, 0.25f),
				Height = (0, 0.3f),
				Position = new Vector2(100)
			};

			UITextInput input = new UITextInput(text)
			{
				Width = (-16, 1),
				Height = (40, 0),
				Position = new Vector2(8)
			};
			panelMain.Append(input);
		}
	}
}