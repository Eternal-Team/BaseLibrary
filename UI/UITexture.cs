using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;

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
	public static readonly UITextureSettings Default = new UITextureSettings {
		ScaleMode = ScaleMode.None,
		SpriteEffects = SpriteEffects.None,
		Rotation = 0f,
		Scale = 1f,
		Color = Color.White,
		ImagePos = Dimension.FromPercent(50),
		Origin = Vector2.Zero,
		SourceRectangle = null,
		SamplerState = SamplerState.LinearClamp,
		ResizeToContent = false
	};

	public ScaleMode ScaleMode;
	public bool ResizeToContent;
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

	protected Texture2D Texture => texture is null ? BaseLibrary.MissingTexture.Value : texture.Value;

	public override void Recalculate()
	{
		if (Settings.ResizeToContent)
		{
			Size.PercentX = 0;
			Size.PercentY = 0;

			Size.PixelsX = Texture.Width;
			Size.PixelsY = Texture.Height;
		}

		base.Recalculate();
	}

	protected override void Draw(SpriteBatch spriteBatch)
	{
		RasterizerState rasterizer = new RasterizerState { CullMode = CullMode.None, ScissorTestEnable = true };

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Settings.SamplerState, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);

		Vector2 textureSize = Settings.SourceRectangle?.Size() ?? Texture.Size();

		Vector2 scale = Settings.ScaleMode switch {
			ScaleMode.Stretch => new Vector2(Dimensions.Width / textureSize.X, Dimensions.Height / textureSize.Y),
			ScaleMode.Zoom => new Vector2(Math.Min(Dimensions.Width / textureSize.X, Dimensions.Height / textureSize.Y)),
			_ => Vector2.One
		};

		scale *= Settings.Scale;

		Vector2 position = new Vector2 {
			X = Dimensions.X + Settings.ImagePos.PercentX * Dimensions.Width * 0.01f - Settings.ImagePos.PercentX * (textureSize.X * scale.X) * 0.01f + Settings.ImagePos.PixelsX,
			Y = Dimensions.Y + Settings.ImagePos.PercentY * Dimensions.Height * 0.01f - Settings.ImagePos.PercentY * (textureSize.Y * scale.Y) * 0.01f + Settings.ImagePos.PixelsY
		};

		spriteBatch.Draw(Texture, position, Settings.SourceRectangle, Settings.Color, Settings.Rotation, Settings.Origin, scale, Settings.SpriteEffects, 0f);

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);
	}
}