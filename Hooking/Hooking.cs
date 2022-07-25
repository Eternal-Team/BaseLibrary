using IL.Terraria;
using On.Terraria.GameInput;

namespace BaseLibrary;

internal static partial class Hooking
{
	internal static void Initialize()
	{
		Input.Load();
		PlayerInput.UpdateInput += PlayerInputOnUpdateInput;
	}
}