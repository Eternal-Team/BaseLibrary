using System;
using System.Reflection;
using BaseLibrary.UI;
using BaseLibrary.Utility;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using On.Terraria.GameContent.UI.Elements;
using On.Terraria.GameInput;
using Terraria;
using Terraria.ModLoader;

namespace BaseLibrary;

internal static partial class Hooking
{
	internal static void Initialize()
	{
		Input.Load();
		PlayerInput.UpdateInput += PlayerInputOnUpdateInput;
		UIKeybindingListItem.OnClickMethod += UIKeybindingListItemOnOnClickMethod;

		MethodInfo methodInfo = typeof(ItemLoader).GetMethod("RightClick", ReflectionUtility.DefaultFlags_Static);
		HookEndpointManager.Modify(methodInfo, ItemLoaderRightClick);
	}

	private static void ItemLoaderRightClick(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);
		ILLabel label = cursor.DefineLabel();

		if (!cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdcI4(7), i => i.MatchLdcI4(-1), i => i.MatchLdcI4(-1)))
			throw new Exception("IL edit failed");

		cursor.Emit(OpCodes.Ldarg, 0);
		cursor.EmitDelegate((Item item) => item.ModItem is not IHasUI);
		cursor.Emit(OpCodes.Brfalse, label);

		if (!cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchPop()))
			throw new Exception("IL edit failed");

		cursor.Index++;
		cursor.MarkLabel(label);
	}
}