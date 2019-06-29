using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

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
			Vector2 position = InnerDimensions.Position() + InnerDimensions.Size() * 0.5f;
			Vector2 origin = textureBack.Size() * 0.5f;

			if (textureFront != null) spriteBatch.Draw(textureFront, Dimensions);

			switch (scaleMode)
			{
				case ScaleMode.Stretch:
					Vector2 scale = new Vector2(InnerDimensions.Width / textureBack.Width, InnerDimensions.Height / textureBack.Height);
					spriteBatch.Draw(textureBack, position, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
					break;
				case ScaleMode.Zoom:
					spriteBatch.Draw(textureBack, position, null, Color.White, 0f, origin, Math.Min(InnerDimensions.Width / textureBack.Width, InnerDimensions.Height / textureBack.Height), SpriteEffects.None, 0f);
					break;
				case ScaleMode.None:
					spriteBatch.Draw(textureBack, position, null, Color.White, 0f, origin, Vector2.One, SpriteEffects.None, 0f);
					break;
			}
		}
	}
}