using BaseLibrary.UI.New;

namespace BaseLibrary.UI
{
	public class TestUI : UIDraggablePanel
	{
		private Ref<string> text = new Ref<string>("");

		public TestUI()
		{
			Width.Percent = 30;
			Height.Pixels = 400;
			X.Pixels = Y.Pixels = 300;

			UITextInput input = new UITextInput(ref text)
			{
				Width = { Percent = 100 },
				Height = { Pixels = 40 },
				RenderPanel = true
			};
			Add(input);

			UIButton button = new UIButton
			{
				Width = { Percent = 100 },
				Height = { Percent = 50 },
				Y = { Pixels = 48 },
				RenderPanel = true
			};
			Add(button);
		}
	}
}