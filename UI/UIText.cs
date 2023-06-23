using System;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BaseLibrary.UI;

public struct UITextOptions
{
	public static readonly UITextOptions Default = new()
	{
		TextColor = Color.White,
		BorderColor = Color.Black,
		HorizontalAlignment = HorizontalAlignment.Left,
		VerticalAlignment = VerticalAlignment.Top,
		ScaleToFit = false,
		Font = FontAssets.MouseText,
		Ellipsis = false
	};

	public Color TextColor;
	public Color BorderColor;
	public HorizontalAlignment HorizontalAlignment;
	public VerticalAlignment VerticalAlignment;
	public bool ScaleToFit;
	public Asset<DynamicSpriteFont> Font;
	public bool Ellipsis;
}

public class UIText : BaseElement
{
	// todo: add an option to "loop" text in case it doesnt fit
	public UITextOptions Settings = UITextOptions.Default;

	public object? Text
	{
		get => text;
		set
		{
			text = value;
			CalculateTextMetrics();
		}
	}

	private object? text;

	private float textScale;
	private Vector2 textSize;
	private Vector2 textPosition;

	public UIText(string text, float scale = 1f)
	{
		this.text = text;
		Settings.Font = scale > 1f ? FontAssets.DeathText : FontAssets.MouseText;
		textScale = scale > 1f ? scale * 0.5f : scale;
	}

	public UIText(LocalizedText text, float scale = 1f)
	{
		this.text = text;
		Settings.Font = scale > 1f ? FontAssets.DeathText : FontAssets.MouseText;
		textScale = scale > 1f ? scale * 0.5f : scale;
	}

	public UIText(ModTranslation text, float scale = 1f)
	{
		this.text = text;
		Settings.Font = scale > 1f ? FontAssets.DeathText : FontAssets.MouseText;
		textScale = scale > 1f ? scale * 0.5f : scale;
	}

	public UIText(Ref<string> text, float scale = 1f)
	{
		this.text = text;
		Settings.Font = scale > 1f ? FontAssets.DeathText : FontAssets.MouseText;
		textScale = scale > 1f ? scale * 0.5f : scale;
	}

	private string? GetString()
	{
		if (text is ModTranslation translation) return translation.Get();
		return text?.ToString();
	}

	public override void Recalculate()
	{
		base.Recalculate();

		CalculateTextMetrics();
	}

	protected override void Draw(SpriteBatch spriteBatch)
	{
		if (text is null)
			return;

		string? actualText = GetString();
		if (string.IsNullOrWhiteSpace(actualText))
			return;

		RasterizerState rasterizer = new RasterizerState { CullMode = CullMode.None, ScissorTestEnable = true };

		spriteBatch.End();

		SamplerState samplerText = SamplerState.LinearClamp;
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, samplerText, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);

		string s = actualText.Replace("\t", "    ");
		Utils.DrawBorderStringFourWay(spriteBatch, Settings.Font.Value, s, textPosition.X, textPosition.Y, Settings.TextColor, Settings.BorderColor, Vector2.Zero, textScale);

		spriteBatch.End();

		SamplerState sampler = SamplerState.PointClamp;
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);
	}

	private void CalculateTextMetrics()
	{
		if (text is null) goto fail;

		string? actualText = GetString();
		if (string.IsNullOrWhiteSpace(actualText)) goto fail;

		textSize = Settings.Font.Value.MeasureString(actualText);
		if (Settings.ScaleToFit) textScale = Math.Min(InnerDimensions.Width / textSize.X, InnerDimensions.Height / textSize.Y);
		textSize *= textScale;

		var hAlign = Settings.HorizontalAlignment;
		var vAlign = Settings.VerticalAlignment;

		textPosition.X = hAlign switch
		{
			HorizontalAlignment.Left => InnerDimensions.X,
			HorizontalAlignment.Center => InnerDimensions.X + InnerDimensions.Width * 0.5f - textSize.X * 0.5f,
			HorizontalAlignment.Right => InnerDimensions.X + InnerDimensions.Width - textSize.X,
			_ => textPosition.X
		};

		// note: text size Y is a bit off
		textPosition.Y = vAlign switch
		{
			VerticalAlignment.Top => InnerDimensions.Y,
			VerticalAlignment.Center => InnerDimensions.Y + InnerDimensions.Height * 0.5f - (textSize.Y - 8f) * 0.5f,
			VerticalAlignment.Bottom => InnerDimensions.Y + InnerDimensions.Height - textSize.Y + 8f * textScale,
			_ => textPosition.Y
		};

		return;

		fail:
		textSize = Vector2.Zero;
		textPosition = Vector2.Zero;
	}
}