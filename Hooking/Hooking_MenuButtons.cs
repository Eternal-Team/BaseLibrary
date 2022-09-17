using System;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BaseLibrary;

internal static partial class Hooking
{
	private static void MainOnDrawVersionNumber(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);

		if (!cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdcI4(6), i => i.MatchNewarr<string>()))
			throw new Exception("IL edit failed");

		cursor.Remove();
		cursor.Emit(OpCodes.Ldc_I4, 7);

		cursor.Index++;

		cursor.Emit(OpCodes.Dup);
		cursor.Emit(OpCodes.Ldc_I4, 0);

		string text = "Itorius' Mods" + Environment.NewLine;

		var mods = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.Core.ModOrganizer")?.GetMethod("FindMods", ReflectionUtility.DefaultFlags_Static)?.InvokeStatic<Array>(true);
		foreach (object mod in mods!)
		{
			if (!mod.GetValue<bool>("Enabled")) continue;

			object properties = mod.GetValue<object>("properties")!;
			if (!properties!.GetValue<string>("author")!.Contains("Itorius")) continue;

			string name = mod.GetValue<string>("DisplayName")!;
			if (name.EndsWith("Library")) continue;
				
			text += $"* {name} v{properties.GetValue<Version>("version")}{Environment.NewLine}";
		}

		cursor.EmitDelegate(() => text + (Main.menuMode == 0 ? Environment.NewLine : ""));
		cursor.Emit(OpCodes.Stelem_Ref);

		int index = 0;
		while (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchDup(), i => i.MatchLdcI4(out index)))
		{
			cursor.Index++;
			cursor.Remove();
			cursor.Emit(OpCodes.Ldc_I4, index + 1);
		}
	}

	private static TitleLinkButton MakeSimpleButton(string textKey, string linkUrl, int horizontalFrameIndex)
	{
		Asset<Texture2D> asset = Terraria.ModLoader.UI.UICommon.tModLoaderTitleLinkButtonsTexture;
		Rectangle value = asset.Frame(8, 2, horizontalFrameIndex);
		Rectangle value2 = asset.Frame(8, 2, horizontalFrameIndex, 1);
		value.Width--;
		value.Height--;
		value2.Width--;
		value2.Height--;
		return new TitleLinkButton
		{
			TooltipTextKey = textKey,
			LinkUrl = linkUrl,
			FrameWehnSelected = value2,
			FrameWhenNotSelected = value,
			Image = asset
		};
	}

	private static void MainOnDrawMenu(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);

		if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCall<Main>("DrawtModLoaderSocialMediaButtons")))
			throw new Exception("IL edit failed");

		cursor.Emit(OpCodes.Ldloc, 30);
		cursor.EmitDelegate((float offset) =>
		{
			Vector2 anchorPosition = new Vector2(18f, Main.screenHeight - 26 - 22 - offset - 32f - 20f);
			for (int i = 0; i < TitleLinks.Count; i++)
			{
				TitleLinks[i].Draw(Main.spriteBatch, anchorPosition);
				anchorPosition.X += 30f;
			}
		});
	}
}