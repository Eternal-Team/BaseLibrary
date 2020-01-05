using BaseLibrary.UI.New;
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
		public Color Color = Color.White;

		public UITexture(Texture2D texture, ScaleMode scaleMode = ScaleMode.None)
		{
			this.texture = texture;
			this.scaleMode = scaleMode;
		}

		protected override void Draw(SpriteBatch spriteBatch)
		{
			if (texture == null) return;

			Vector2 position = Dimensions.Position() + Dimensions.Size() * 0.5f;
			Vector2 origin = texture.Size() * 0.5f;

			SpriteBatchState state = new SpriteBatchState
			{
				BlendState = BlendState.NonPremultiplied,
				SpriteSortMode = SpriteSortMode.Immediate,
				SamplerState = SamplerState.LinearClamp,
				TransformMatrix = Matrix.Identity
			};

			spriteBatch.Draw(state, () =>
			{
				switch (scaleMode)
				{
					case ScaleMode.Stretch:
						Vector2 scale = new Vector2(Dimensions.Width / (float)texture.Width, Dimensions.Height / (float)texture.Height);
						spriteBatch.Draw(texture, position, null, Color, Rotation.ToRadians(), origin, scale, SpriteEffects, 0f);
						break;
					case ScaleMode.Zoom:
						spriteBatch.Draw(texture, position, null, Color, Rotation.ToRadians(), origin, Math.Min(Dimensions.Width / texture.Width, Dimensions.Height / texture.Height), SpriteEffects, 0f);
						break;
					case ScaleMode.None:
						spriteBatch.Draw(texture, position, null, Color, Rotation.ToRadians(), origin, Vector2.One, SpriteEffects, 0f);
						break;
				}
			});
		}
	}
}