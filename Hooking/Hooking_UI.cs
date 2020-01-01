using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace BaseLibrary
{
	public static partial class Hooking
	{
		private const int CustomCursorOverride = 1000;
		private static Texture2D CursorTexture;
		private static Vector2 CursorOffset;
		private static bool Pulse;

		public static void SetCursor(string texture, Vector2? offset = null, bool pulse = true)
		{
			Main.cursorOverride = CustomCursorOverride;
			CursorTexture = ModContent.GetTexture(texture);
			CursorOffset = offset ?? Vector2.Zero;
			Pulse = pulse;
		}

		private static void CloseUI_ItemSlot(On.Terraria.UI.ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
		{
			if (Main.mouseItem.modItem is IHasUI mouse) BaseLibrary.PanelGUI.UI.CloseUI(mouse);

			if (inv[slot].modItem is IHasUI hasUI) BaseLibrary.PanelGUI.UI.CloseUI(hasUI);

			orig(inv, context, slot);
		}

		private static void CloseUI_Drop(On.Terraria.Player.orig_DropSelectedItem orig, Player self)
		{
			if (self.HeldItem.modItem is IHasUI hasUI) BaseLibrary.PanelGUI.UI.CloseUI(hasUI);

			orig(self);
		}

		private static UIElement UIElement_GetElementAt(On.Terraria.UI.UIElement.orig_GetElementAt orig, UIElement self, Vector2 point)
		{
			if (self is PanelUI ui)
			{
				UIElement uiElement = null;
				for (int i = ui.Elements.Count - 1; i >= 0; i--)
				{
					UIElement current = ui.Elements[i];
					if (current.ContainsPoint(point))
					{
						uiElement = current;
						break;
					}
				}

				return uiElement?.GetElementAt(point);
			}

			return orig(self, point);
		}

		private static void DrawCursor(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);
			ILLabel label = cursor.DefineLabel();

			if (cursor.TryGotoNext(i => i.MatchLdsfld(typeof(Main).GetField("cursorOverride", Utility.defaultFlags))))
			{
				cursor.Emit(OpCodes.Ldsfld, typeof(Main).GetField("cursorOverride", Utility.defaultFlags));
				cursor.Emit(OpCodes.Ldc_I4, CustomCursorOverride);
				cursor.Emit(OpCodes.Ceq);
				cursor.Emit(OpCodes.Brfalse, label);

				cursor.Emit(OpCodes.Ldloc, 4);
				cursor.Emit(OpCodes.Ldloc, 6);

				cursor.EmitDelegate<Action<float, float>>((rotation, scale) =>
				{
					if (CursorTexture == null) return;

					float texScale = Math.Min(20f / CursorTexture.Width, 20f / CursorTexture.Height);
					float s = Pulse ? Main.cursorScale * texScale : texScale;
					Main.spriteBatch.Draw(CursorTexture, new Vector2(Main.mouseX, Main.mouseY), null, Color.White, rotation, CursorOffset, s, SpriteEffects.None, 0f);
				});
				cursor.Emit(OpCodes.Ret);

				cursor.MarkLabel(label);
			}
		}
	}
}