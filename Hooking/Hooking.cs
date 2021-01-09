namespace BaseLibrary
{
	internal static partial class Hooking
	{
		internal static void Initialize()
		{
			IL.Terraria.Recipe.FindRecipes += Recipe_FindRecipes;
			IL.Terraria.Recipe.Create += Recipe_Create;
			IL.Terraria.Main.DrawInterface_36_Cursor += DrawCursor;

			// if (ModLoader.GetMod("BaseLibrary") != null) ItemSlot.OverrideHover += ItemSlot_OverrideHover;
		}
	}
}