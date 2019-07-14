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
		private static string CurrrentShiftClickIcon;

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

		private static void Main_DrawInterface_36_Cursor(ILContext il)
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
					if (string.IsNullOrWhiteSpace(CurrrentShiftClickIcon)) return;

					Texture2D texture = ModContent.GetTexture(CurrrentShiftClickIcon);

					float texScale = Math.Min(20f / texture.Width, 20f / texture.Height);
					Main.spriteBatch.Draw(texture, new Vector2(Main.mouseX, Main.mouseY), null, Color.White, rotation, Vector2.Zero, Main.cursorScale * texScale, SpriteEffects.None, 0f);
				});
				cursor.Emit(OpCodes.Ret);

				cursor.MarkLabel(label);
			}
		}

		public static void SetCursor(string texture)
		{
			Main.cursorOverride = CustomCursorOverride;
			CurrrentShiftClickIcon = texture;
		}
	}
}