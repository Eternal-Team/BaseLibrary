using Terraria.GameInput;
using Terraria.ModLoader;

namespace BaseLibrary
{
	public class BaseLibraryPlayer : ModPlayer
	{
		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			if (BaseLibrary.hotkeyOpenBook.JustPressed) BaseLibrary.Instance.BookUI.Visible = !BaseLibrary.Instance.BookUI.Visible;
		}
	}
}