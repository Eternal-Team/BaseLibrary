using System;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using static BaseLibrary.Utility.Utility;

namespace BaseLibrary.UI.Elements
{
	public class UITexture : BaseElement
	{
		private Texture2D textureBack;
		private Texture2D textureFront;
		private ScaleMode scaleMode;

		public UITexture(Texture2D textureBack, ScaleMode scaleMode = ScaleMode.None)
		{
			this.textureBack = textureBack;
			this.scaleMode = scaleMode;
		}

		public UITexture(Texture2D textureBack, Texture2D textureFront, ScaleMode scaleMode = ScaleMode.None)
		{
			this.textureBack = textureBack;
			this.textureFront = textureFront;
			this.scaleMode = scaleMode;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = GetDimensions();

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, null, OverflowHiddenState, null, Main.UIScaleMatrix);

			if (textureFront != null) spriteBatch.Draw(textureFront, dimensions);

			switch (scaleMode)
			{
				case ScaleMode.Stretch:
					spriteBatch.Draw(textureBack, dimensions.ToRectangleF() + new RectangleF(PaddingTop, PaddingTop, PaddingTop * -2f, PaddingTop * -2f));
					break;
				case ScaleMode.Zoom:
					spriteBatch.Draw(textureBack, dimensions.Position() + dimensions.Size() * 0.5f, null, Color.White, 0f, new Vector2(textureBack.Width, textureBack.Height) * 0.5f, Math.Min((dimensions.Width - PaddingTop * 2f) / textureBack.Width, (dimensions.Height - PaddingTop * 2f) / textureBack.Height), SpriteEffects.None, 0f);
					break;
				case ScaleMode.None:
					spriteBatch.Draw(textureBack, dimensions.Position());
					break;
			}

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, OverflowHiddenState, null, Main.UIScaleMatrix);
		}
	}
}