using BaseLibrary.UI.New;
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

		protected override void Update(GameTime gameTime)
		{
			if (IsMouseHovering && Main.mouseLeft)
			{
				Vector2 coords = (UserInterface.ActiveInstance.MousePosition - Dimensions.Position()) / Dimensions.Size();

				OnChangeColor(coords.Y < 0.5 ? new Color(Utility.HSV2RGB(new Vector3(coords.X * 360f, coords.Y * 2f, 1f))) : new Color(Utility.HSV2RGB(new Vector3(coords.X * 360f, 1f, 1f - (coords.Y * 2f - 1f)))));
			}
		}

		protected override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.DrawWithEffect(BaseLibrary.ColorSelectionShader, () => spriteBatch.Draw(Main.magicPixel, Dimensions));
		}
	}
}