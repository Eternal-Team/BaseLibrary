using System;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace BaseLibrary.UI.Elements
{
	public class UITexture : BaseElement
	{
		public Texture2D texture;
		public ScaleMode scaleMode;

		public UITexture(Texture2D texture, ScaleMode scaleMode = ScaleMode.None)
		{
			this.texture = texture;
			this.scaleMode = scaleMode;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = GetDimensions();

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, null, Utility.Utility.OverflowHiddenState, null, Main.UIScaleMatrix);

			switch (scaleMode)
			{
				case ScaleMode.Stretch:
					spriteBatch.Draw(texture, dimensions);
					break;
				case ScaleMode.Zoom:
					spriteBatch.Draw(texture, dimensions.Position(), null, Color.White, 0f, Vector2.Zero, Math.Min(dimensions.Width / texture.Width, dimensions.Height / texture.Height), SpriteEffects.None, 0f);
					break;
				case ScaleMode.None:
					spriteBatch.Draw(texture, dimensions.Position());
					break;
			}

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, Utility.Utility.OverflowHiddenState, null, Main.UIScaleMatrix);
		}
	}
}