using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

namespace BaseLibrary.UI.Elements
{
	public class UIAnimatedTexture : BaseElement
	{
		private Texture2D textureBack;
		private Texture2D textureFront;
		private ScaleMode scaleMode;

		private DrawAnimation animation;
		public bool Animate;

		public UIAnimatedTexture(Texture2D textureBack, DrawAnimation animation, ScaleMode scaleMode = ScaleMode.None)
		{
			this.textureBack = textureBack;
			this.scaleMode = scaleMode;

			this.animation = animation;
		}

		public UIAnimatedTexture(Texture2D textureBack, Texture2D textureFront, ScaleMode scaleMode = ScaleMode.None)
		{
			this.textureBack = textureBack;
			this.textureFront = textureFront;
			this.scaleMode = scaleMode;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			Vector2 position = InnerDimensions.Position() + InnerDimensions.Size() * 0.5f;
			Rectangle frame = animation.GetFrame(textureBack);
			Vector2 origin = frame.Size() * 0.5f;

			if (Animate) animation.Update();

			if (textureFront != null) spriteBatch.Draw(textureFront, Dimensions);

			switch (scaleMode)
			{
				case ScaleMode.Stretch:
					spriteBatch.Draw(textureBack, position, frame, Color.White, 0f, origin, new Vector2(InnerDimensions.Width / frame.Width, InnerDimensions.Height / frame.Height), SpriteEffects.None, 0f);
					break;
				case ScaleMode.Zoom:
					spriteBatch.Draw(textureBack, position, frame, Color.White, 0f, origin, Math.Min(InnerDimensions.Width / frame.Width, InnerDimensions.Height / frame.Height), SpriteEffects.None, 0f);
					break;
				case ScaleMode.None:
					spriteBatch.Draw(textureBack, position, frame, Color.White, 0f, origin, Vector2.One, SpriteEffects.None, 0f);
					break;
			}
		}
	}
}