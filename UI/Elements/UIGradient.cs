using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.UI;

namespace BaseLibrary.UI.Elements
{
	public class UIGradient : BaseElement
	{
		public Action<Color> OnChangeColor;

		public override void Click(UIMouseEvent evt)
		{
			Vector2 coords = (evt.MousePosition - Dimensions.Position()) / Dimensions.Size();

			OnChangeColor(coords.Y < 0.5 ? new Color(Utility.HSV2RGB(new Vector3(coords.X * 360f, coords.Y * 2f, 1f))) : new Color(Utility.HSV2RGB(new Vector3(coords.X * 360f, 1f, 1f - (coords.Y * 2f - 1f)))));
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			spriteBatch.End();

			spriteBatch.Begin(
				SpriteSortMode.Immediate,
				BlendState.AlphaBlend,
				SamplerState.AnisotropicClamp,
				DepthStencilState.None,
				Utility.OverflowHiddenState,
				BaseLibrary.ColorSelectionShader,
				Main.UIScaleMatrix);

			spriteBatch.Draw(Main.magicPixel, Dimensions);

			spriteBatch.End();
			spriteBatch.Begin();
		}
	}
}