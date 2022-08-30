using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
		public static readonly UITextureSettings Default = new()
		{
			ScaleMode = ScaleMode.None,
			SpriteEffects = SpriteEffects.None,
			Rotation = 0f,
			Scale = 1f,
			Color = Color.White,
			ImageX = new StyleDimension(0, 50),
			ImageY = new StyleDimension(0, 50),
			Origin = Vector2.Zero,
			SourceRectangle = null
		};

		public ScaleMode ScaleMode;
		public SpriteEffects SpriteEffects;
		public float Rotation;
		public float Scale;
		public Color Color;
		public Vector2 Origin;

		public Rectangle? SourceRectangle;

		public StyleDimension ImageX;
		public StyleDimension ImageY;
	}

	public class UITexture : BaseElement
	{
		public UITextureSettings Settings = UITextureSettings.Default;

		private Asset<Texture2D>? Texture;

		public UITexture(Asset<Texture2D>? texture)
		{
			Texture = texture;
		}

		protected override void Draw(SpriteBatch spriteBatch)
		{
			if (Texture is null)
				return;

			Texture2D texture = Texture.Value;
			Vector2 textureSize = Settings.SourceRectangle?.Size() ?? texture.Size();

			Vector2 scale = Vector2.One;
			if (Settings.ScaleMode == ScaleMode.Stretch) scale = new Vector2(Dimensions.Width / textureSize.X, Dimensions.Height / textureSize.Y);
			else if (Settings.ScaleMode == ScaleMode.Zoom) scale = new Vector2(Math.Min(Dimensions.Width / textureSize.X, Dimensions.Height / textureSize.Y));

			scale *= Settings.Scale;

			Vector2 position = new Vector2
			{
				X = Dimensions.X + Settings.ImageX.Percent * Dimensions.Width / 100f - Settings.ImageX.Percent * (textureSize.X * scale.X) / 100f + Settings.ImageX.Pixels,
				Y = Dimensions.Y + Settings.ImageY.Percent * Dimensions.Height / 100f - Settings.ImageY.Percent * (textureSize.Y * scale.Y) / 100f + Settings.ImageY.Pixels
			};

			spriteBatch.Draw(texture, position, Settings.SourceRectangle, Settings.Color, Settings.Rotation, Settings.Origin, scale, Settings.SpriteEffects, 0f);
		}
	}
}