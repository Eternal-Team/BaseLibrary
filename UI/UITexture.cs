using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace BaseLibrary.UI
{
	public enum ScaleMode
	{
		Stretch,
		Zoom,
		None
	}

	public struct UITextureSettings
	{
		public static readonly UITextureSettings Default = new UITextureSettings
		{
			ScaleMode = ScaleMode.None,
			SpriteEffects = SpriteEffects.None,
			Rotation = 0f,
			Scale = 1f,
			Color = Color.White,
			ImageX = new StyleDimension(0, 50),
			ImageY = new StyleDimension(0, 50),
			Origin = Vector2.Zero
		};

		public ScaleMode ScaleMode;
		public SpriteEffects SpriteEffects;
		public float Rotation;
		public float Scale;
		public Color Color;
		public Vector2 Origin;
		
		public StyleDimension ImageX;
		public StyleDimension ImageY;
	}

	public class UITexture : BaseElement
	{
		public UITextureSettings Settings = UITextureSettings.Default;

		public Texture2D Texture;

		public UITexture(Texture2D texture)
		{
			Texture = texture;
		}

		protected override void Draw(SpriteBatch spriteBatch)
		{
			if (Texture == null) return;

			Vector2 scale = Vector2.Zero;

			if (Settings.ScaleMode == ScaleMode.Stretch) scale = new Vector2(Dimensions.Width / (float)Texture.Width, Dimensions.Height / (float)Texture.Height);
			else if (Settings.ScaleMode == ScaleMode.Zoom)
			{
				float s = Math.Min(Dimensions.Width / (float)Texture.Width, Dimensions.Height / (float)Texture.Height);
				scale = new Vector2(s);
			}
			else if (Settings.ScaleMode == ScaleMode.None) scale = Vector2.One;

			scale *= Settings.Scale;
			Vector2 texSize = Texture.Size() * scale;
			Vector2 position = new Vector2
			{
				X = Dimensions.X + Settings.ImageX.Percent * Dimensions.Width / 100f - Settings.ImageX.Percent * texSize.X / 100f + Settings.ImageX.Pixels,
				Y = Dimensions.Y + Settings.ImageY.Percent * Dimensions.Height / 100f - Settings.ImageY.Percent * texSize.Y / 100f + Settings.ImageY.Pixels
			};

			spriteBatch.Draw(Texture, position, null, Settings.Color, Settings.Rotation, Settings.Origin, scale, Settings.SpriteEffects, 0f);
		}
	}
}