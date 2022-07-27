using On.Terraria.GameContent.UI.Elements;
using On.Terraria.GameInput;

namespace BaseLibrary;

internal static partial class Hooking
{
	internal static void Initialize()
	{
		Input.Load();
		PlayerInput.UpdateInput += PlayerInputOnUpdateInput;
		UIKeybindingListItem.OnClickMethod += UIKeybindingListItemOnOnClickMethod;
	}
}