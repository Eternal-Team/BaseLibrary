using System;
using System.Collections.Generic;
using System.Reflection;
using BaseLibrary.UI;
using BaseLibrary.Utility;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace BaseLibrary;

internal static partial class Hooking
{
	private static List<TitleLinkButton> TitleLinks;

	internal static void Initialize()
	{
		Input.Load();

		On_PlayerInput.UpdateInput += PlayerInputOnUpdateInput;
		On_UIKeybindingListItem.OnClickMethod += UIKeybindingListItemOnOnClickMethod;

		MethodInfo? methodInfo = typeof(ItemLoader).GetMethod("RightClick", ReflectionUtility.DefaultFlags_Static);
		MonoModHooks.Modify(methodInfo, ItemLoaderRightClick);

		TitleLinks = new List<TitleLinkButton>
		{
			MakeSimpleButton("TitleLinks.Discord", "https://terraria.itorius.com/discord", 0),
			MakeSimpleButton("TitleLinks.Patreon", "https://www.itorius.com/patreon", 7)
		};

		IL_Main.DrawMenu += MainOnDrawMenu;
		IL_Main.DrawVersionNumber += MainOnDrawVersionNumber;
	}

	private static void ItemLoaderRightClick(ILContext il)
	{
		// Prevents the click sound from playing when rightclicking a IHasUI item

		try
		{
			ILCursor cursor = new ILCursor(il);
			ILLabel label = cursor.DefineLabel();

			cursor.GotoNext(MoveType.AfterLabel, i => i.MatchLdcI4(7), i => i.MatchLdcI4(-1), i => i.MatchLdcI4(-1));

			cursor.Emit(OpCodes.Ldarg, 0);
			cursor.EmitDelegate((Item item) => item.ModItem is not IHasUI);
			cursor.Emit(OpCodes.Brfalse, label);

			cursor.GotoNext(MoveType.AfterLabel, i => i.MatchPop());

			cursor.Index++;
			cursor.MarkLabel(label);
		}
		catch (Exception e)
		{
			throw new ILPatchFailureException(ModContent.GetInstance<BaseLibrary>(), il, e);
		}
	}
}