using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace BaseLibrary.UI.Elements
{
	public class UIRoundedBar : BaseElement
	{
		public int BorderRadius = 4;
		public Color BackgroundColor = new Color(245, 244, 242);
		public Color BarColor = new Color(20, 144, 112);

		public int Progress;

		protected override void Draw(SpriteBatch spriteBatch)
		{
			Effect shader = BaseLibrary.RoundedRectShader;
			Vector2 size = Size;

			spriteBatch.DrawWithEffect(shader, () =>
			{
				shader.Parameters["u_Dimensions"].SetValue(size);
				shader.Parameters["u_Radius"].SetValue(BorderRadius);

				spriteBatch.Draw(Main.magicPixel, Dimensions, BackgroundColor);

				Vector2 completedSize = new Vector2(size.X * Progress * 0.01f, size.Y);
				completedSize.X = Math.Max(completedSize.X, BorderRadius * 2);

				shader.Parameters["u_Dimensions"].SetValue(completedSize);
				shader.Parameters["u_Radius"].SetValue(BorderRadius);

				spriteBatch.Draw(Main.magicPixel, new Rectangle((int)Position.X, (int)Position.Y, (int)completedSize.X, (int)completedSize.Y), BarColor);
			});
		}
	}
}