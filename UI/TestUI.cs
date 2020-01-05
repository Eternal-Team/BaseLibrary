using BaseLibrary.UI.New;

namespace BaseLibrary.UI
{
	public class TestUI : BaseElement
	{
		private Ref<string> text = new Ref<string>("");

		public TestUI()
		{
			Width.Percent = 30;
			Height.Pixels = 400;
			X.Pixels = Y.Pixels = 300;

			//UITextInput input = new UITextInput(ref text)
			//{
			//	Width = { Percent = 10000)
			//	Height = { Pixels = 40)
			//	RenderPanel = true
			//};
			//Add(input);

			//UIButton button = new UIButton
			//{
			//	Width = { Percent = 10000)
			//	Height = { Percent = 50)
			//	Y = { Pixels = 48)
			//	RenderPanel = true
			//};
			//Add(button);

			//UIGrid<UIButton> grid = new UIGrid<UIButton>(2)
			//{
			//	Width = { Percent = 10004)
			//	Height = { Percent = 10000 }
			//};
			//Add(grid);

			//for (int i = 0; i < 50; i++)
			//{
			//	UIButton button = new UIButton
			//	{
			//		Width = { Pixels = 50)
			//		Height = { Pixels = 50)
			//		RenderPanel = true
			//	};
			//	grid.Add(button);
			//}

			//UIButton but = new UIButton
			//{
			//	Width = { Pixels = 150)
			//	Height = { Pixels = 150)
			//	X = { Percent = 50)
			//	Y = { Percent = 50)
			//	RenderPanel = true
			//};
			//Add(but);
		}
	}
}