using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace BaseLibrary.UI;

public enum ScaleMode
{
	/// <summary>
	/// Contents will be stretched out to the entire element
	/// </summary>
	Stretch,
	/// <summary>
	/// Contents will be stretched out to the smallest dimension (width/height)
	/// </summary>
	Zoom,
	/// <summary>
	/// Content will be displayed as is
	/// </summary>
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

public class UITexture : BaseElement
{
	protected static Asset<Texture2D> MissingTexture;
	
	public UITextureSettings Settings = UITextureSettings.Default;

	public readonly Asset<Texture2D>? Texture;

	public UITexture(Asset<Texture2D>? texture)
	{
		MissingTexture ??= ModContent.Request<Texture2D>(BaseLibrary.PlaceholderTexture);

		Texture = texture;
	}

	protected override void Draw(SpriteBatch spriteBatch)
	{
		Texture2D texture = Texture is null ? MissingTexture.Value : Texture.Value;

		RasterizerState rasterizer = new() { CullMode = CullMode.None, ScissorTestEnable = true };

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Settings.SamplerState, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);

		Vector2 textureSize = Settings.SourceRectangle?.Size() ?? texture.Size();

		Vector2 scale = Settings.ScaleMode switch {
			ScaleMode.Stretch => new Vector2(Dimensions.Width / textureSize.X, Dimensions.Height / textureSize.Y),
			ScaleMode.Zoom => new Vector2(Math.Min(Dimensions.Width / textureSize.X, Dimensions.Height / textureSize.Y)),
			_ => Vector2.One
		};

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