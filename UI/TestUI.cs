using BaseLibrary.UI.Elements;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace BaseLibrary.UI
{
	public class TestUI : New.UIDraggablePanel
	{
		public TestUI()
		{
			Width.Percent = 30;
			Height.Pixels = 400;
			X.Pixels = Y.Pixels = 300;

			New.UIDraggablePanel panel = new New.UIDraggablePanel
			{
				Width = { Percent = 80 },
				Height = { Percent = 50 }
			};
			Add(panel);

			int i = 0;
			foreach (var pair in typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.HotKeyLoader").GetValue<Dictionary<string, ModHotKey>>("modHotKeys"))
			{
				New.UIText text = new New.UIText(pair.Value.GetValue<string>("uniqueName"))
				{
					Width = { Percent = 100 },
					Y = { Pixels = 28 * i++ },
					HorizontalAlignment = HorizontalAlignment.Center
				};
				panel.Add(text);
			}
		}
	}
}