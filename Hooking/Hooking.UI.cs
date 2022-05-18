using System;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace BaseLibrary;

internal static partial class Hooking
{
	private const int CustomCursorOverride = 1000;
	private static Asset<Texture2D> CursorTexture;
	private static Vector2 CursorOffset;
	private static bool Pulse;

	public static void SetCursor(string texture, Vector2? offset = null, bool pulse = true)
	{
		Main.cursorOverride = CustomCursorOverride;
		CursorTexture = ModContent.Request<Texture2D>(texture);
		CursorOffset = offset ?? Vector2.Zero;
		Pulse = pulse;
	}

	private static void DrawCursor(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);
		ILLabel label = cursor.DefineLabel();

		if (cursor.TryGotoNext(i => i.MatchLdsfld(typeof(Main).GetField("cursorOverride", ReflectionUtility.DefaultFlags_Static))))
		{
			cursor.Emit(OpCodes.Ldsfld, typeof(Main).GetField("cursorOverride", ReflectionUtility.DefaultFlags_Static));
			cursor.Emit(OpCodes.Ldc_I4, CustomCursorOverride);
			cursor.Emit(OpCodes.Ceq);
			cursor.Emit(OpCodes.Brfalse, label);

			cursor.Emit(OpCodes.Ldloc, 5);
			cursor.Emit(OpCodes.Ldloc, 7);

			cursor.EmitDelegate<Action<float, float>>((rotation, scale) =>
			{
				if (CursorTexture == null) return;

				float texScale = Math.Min(20f / CursorTexture.Value.Width, 20f / CursorTexture.Value.Height);
				float s = Pulse ? Main.cursorScale * texScale : texScale;
				Main.spriteBatch.Draw(CursorTexture.Value, new Vector2(Main.mouseX, Main.mouseY), null, Color.White, rotation, CursorOffset, s, SpriteEffects.None, 0f);
			});
			cursor.Emit(OpCodes.Ret);

			cursor.MarkLabel(label);
		}
	}

	// todo: container lib
	/*private static void ItemSlot_OverrideHover(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);

		if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdarg(1), i => i.MatchBrtrue(out _), i => i.MatchLdsfld<Main>("InReforgeMenu")))
		{
			ILLabel label = cursor.Instrs[cursor.Index - 2].Operand as ILLabel;

			cursor.Emit(OpCodes.Ldloc, 0);
			cursor.EmitDelegate<Func<Item, bool>>(item =>
			{
				foreach (BaseElement element in PanelUI.Instance.Children)
				{
					if (element.Display == Display.Visible && element is IItemStorageUI ui && ui.GetItemStorage().CanInsertItem(Main.LocalPlayer, item))
					{
						string texture = ui.GetTexture(item);

						if (!string.IsNullOrWhiteSpace(texture))
						{
							SetCursor(texture);
							return true;
						}
					}
				}

				return false;
			});

			cursor.Emit(OpCodes.Brtrue, label);
		}

		if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchCall<Main>("get_npcShop")))
		{
			ILLabel label = cursor.Instrs[cursor.Index - 1].Operand as ILLabel;

			cursor.Emit(OpCodes.Ldloc, 0);
			cursor.EmitDelegate<Func<Item, bool>>(item =>
			{
				foreach (BaseElement element in PanelUI.Instance.Children)
				{
					if (element.Display == Display.Visible && element is IItemStorageUI ui && ui.GetItemStorage().CanInsertItem(Main.LocalPlayer, item))
					{
						string texture = ui.GetTexture(item);

						if (!string.IsNullOrWhiteSpace(texture))
						{
							SetCursor(texture);
							return true;
						}
					}
				}

				return false;
			});

			cursor.Emit(OpCodes.Brtrue, label);
		}
	}*/
}