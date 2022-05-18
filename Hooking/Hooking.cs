using IL.Terraria;
using On.Terraria.GameInput;

namespace BaseLibrary;

internal static partial class Hooking
{
	internal static void Initialize()
	{
		Main.DrawInterface_36_Cursor += DrawCursor;

		Input.Load();
		PlayerInput.UpdateInput += PlayerInputOnUpdateInput;
	}
}