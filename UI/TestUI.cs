namespace BaseLibrary.UI
{
	public class TestUI : New.UIDraggablePanel
	{
		private Ref<string> text = new Ref<string>("");

		public TestUI()
		{
			Width.Percent = 30;
			Height.Pixels = 400;
			X.Pixels = Y.Pixels = 300;

			New.UITextInput input = new New.UITextInput(ref text)
			{
				Width = { Percent = 100 },
				Height = { Pixels = 40 },
				RenderPanel = true
			};
			Add(input);

			//int i = 0;
			//foreach (var pair in typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.HotKeyLoader").GetValue<Dictionary<string, ModHotKey>>("modHotKeys"))
			//{
			//	New.UIText text = new New.UIText(pair.Value.GetValue<string>("uniqueName"))
			//	{
			//		Width = { Percent = 100 },
			//		Y = { Pixels = 28 * i++ },
			//		HorizontalAlignment = HorizontalAlignment.Center
			//	};
			//	panel.Add(text);
			//}
		}
	}
}