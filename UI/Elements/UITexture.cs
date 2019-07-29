using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace BaseLibrary.UI.Elements
{
	public class UITexture : BaseElement
	{
		public Texture2D texture;
		private ScaleMode scaleMode;

		public SpriteEffects SpriteEffects = SpriteEffects.None;
		public float Rotation;

		public UITexture(Texture2D texture, ScaleMode scaleMode = ScaleMode.None)
		{
			this.texture = texture;
			this.scaleMode = scaleMode;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (texture == null) return;

			Vector2 position = InnerDimensions.Position() + InnerDimensions.Size() * 0.5f;
			Vector2 origin = texture.Size() * 0.5f;

			switch (scaleMode)
			{
				case ScaleMode.Stretch:
					Vector2 scale = new Vector2(InnerDimensions.Width / texture.Width, InnerDimensions.Height / texture.Height);
					spriteBatch.Draw(texture, position, null, Color.White, Rotation.ToRadians(), origin, scale, SpriteEffects, 0f);
					break;
				case ScaleMode.Zoom:
					spriteBatch.Draw(texture, position, null, Color.White, Rotation.ToRadians(), origin, Math.Min(InnerDimensions.Width / texture.Width, InnerDimensions.Height / texture.Height), SpriteEffects, 0f);
					break;
				case ScaleMode.None:
					spriteBatch.Draw(texture, position, null, Color.White, Rotation.ToRadians(), origin, Vector2.One, SpriteEffects, 0f);
					break;
			}
		}
	}
}