using BaseLibrary.UI.Elements;

namespace BaseLibrary.UI
{
	public class TestUI : BaseUI
	{
		public override void Initialize()
		{
			panelMain = new UIDraggablePanel
			{
				Width = (0, 0.25f),
				Height = (0, 0.25f)
			};
			panelMain.Center();

			UITextButton button = new UITextButton("Reinitialize")
			{
				Width = (0, 1),
				Height = (28, 0)
			};
			button.SetPadding(4);
			button.OnClick += (evt, element) =>
			{
				RemoveAllChildren();
				OnInitialize();
			};
			panelMain.Append(button);

			UIScrollbar scrollbar = new UIScrollbar
			{
				Width = (0, 0.5f),
				Height = (-36, 1),
				Top = (36, 0)
			};
			scrollbar.SetView(1000f, 1000f);
			panelMain.Append(scrollbar);
		}
	}
}