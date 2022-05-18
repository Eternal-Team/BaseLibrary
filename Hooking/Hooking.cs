using IL.Terraria;
using On.Terraria.GameInput;

namespace BaseLibrary;

internal static partial class Hooking
{
	internal static void Initialize()
	{
		// Recipe.FindRecipes += Recipe_FindRecipes;
		// Recipe.Create += Recipe_Create;
		Main.DrawInterface_36_Cursor += DrawCursor;

		Input.Load();
		PlayerInput.UpdateInput += PlayerInputOnUpdateInput;

		// ItemSlot.OverrideHover_ItemArray_int_int += ItemSlot_OverrideHover;
	}
}