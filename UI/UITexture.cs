using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;

namespace BaseLibrary.UI;

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
		ImagePos = Dimension.FromPercent(50),
		Origin = Vector2.Zero,
		SourceRectangle = null,
		SamplerState = SamplerState.LinearClamp
	};

	public ScaleMode ScaleMode;
	public SpriteEffects SpriteEffects;
	public float Rotation;
	public float Scale;
	public Color Color;
	public Vector2 Origin;
	public SamplerState SamplerState;

	public Rectangle? SourceRectangle;

	public Dimension ImagePos;
}

public class UITexture(Asset<Texture2D>? texture) : BaseElement
{
	public UITextureSettings Settings = UITextureSettings.Default;

	public readonly Asset<Texture2D>? Texture = texture;

	protected override void Draw(SpriteBatch spriteBatch)
	{
		if (Texture is null)
			return;

		RasterizerState rasterizer = new() { CullMode = CullMode.None, ScissorTestEnable = true };

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Settings.SamplerState, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);

		Texture2D texture = Texture.Value;
		Vector2 textureSize = Settings.SourceRectangle?.Size() ?? texture.Size();

		Vector2 scale = Vector2.One;
		if (Settings.ScaleMode == ScaleMode.Stretch) scale = new Vector2(Dimensions.Width / textureSize.X, Dimensions.Height / textureSize.Y);
		else if (Settings.ScaleMode == ScaleMode.Zoom) scale = new Vector2(Math.Min(Dimensions.Width / textureSize.X, Dimensions.Height / textureSize.Y));

		scale *= Settings.Scale;

		Vector2 position = new()
		{
			X = Dimensions.X + Settings.ImagePos.PercentX * Dimensions.Width * 0.01f - Settings.ImagePos.PercentX * (textureSize.X * scale.X) * 0.01f + Settings.ImagePos.PixelsX,
			Y = Dimensions.Y + Settings.ImagePos.PercentY * Dimensions.Height * 0.01f - Settings.ImagePos.PercentY * (textureSize.Y * scale.Y) * 0.01f + Settings.ImagePos.PixelsY
		};

		spriteBatch.Draw(texture, position, Settings.SourceRectangle, Settings.Color, Settings.Rotation, Settings.Origin, scale, Settings.SpriteEffects, 0f);

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);
	}
}