using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.UI.Chat;

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
	private readonly List<TextLine> _snippets = [];
	private readonly float textScale;
	public UITextOptions Settings = UITextOptions.Default;
	private object? text;

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

	public UIText(Ref<string> text, float scale = 1f)
	{
		this.text = text;
		Settings.Font = scale > 1f ? FontAssets.DeathText : FontAssets.MouseText;
		textScale = scale > 1f ? scale * 0.5f : scale;
	}

	public object? Text
	{
		get => text;
		set
		{
			text = value;
			CalculateTextMetrics();
		}
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

		string? actualText = text.ToString();
		if (string.IsNullOrWhiteSpace(actualText))
			return;

		RasterizerState rasterizer = new() { CullMode = CullMode.None, ScissorTestEnable = true };

		spriteBatch.End();

		SamplerState samplerText = SamplerState.LinearClamp;
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, samplerText, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);

		foreach (TextLine snippets in _snippets)
		{
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Settings.Font.Value, snippets.Snippets.ToArray(), snippets.Position, 0f, Settings.TextColor, Settings.BorderColor, Vector2.Zero, new Vector2(textScale), out int _);
		}

		spriteBatch.End();

		SamplerState sampler = SamplerState.PointClamp;
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);
	}

	private void CalculateTextMetrics()
	{
		_snippets.Clear();
		string? actualText = text?.ToString();
		if (string.IsNullOrWhiteSpace(actualText)) return;

		HorizontalAlignment hAlign = Settings.HorizontalAlignment;
		VerticalAlignment vAlign = Settings.VerticalAlignment;

		if (InnerDimensions.Width > 0)
		{
			float totalHeight = 0;
			foreach (List<TextSnippet> snippets in Utils.WordwrapStringSmart(actualText, Settings.TextColor, Settings.Font.Value, InnerDimensions.Width, -1))
			{
				Vector2 size = ChatManager.GetStringSize(Settings.Font.Value, snippets.ToArray(), new Vector2(textScale));
				_snippets.Add(new TextLine(snippets, size, Vector2.Zero));
				totalHeight += size.Y;
			}

			totalHeight -= 8f;
			
			float top = 0;
			foreach (TextLine line in _snippets)
			{
				Vector2 size = line.Size;
				line.Position.X = hAlign switch
				{
					HorizontalAlignment.Left => InnerDimensions.X,
					HorizontalAlignment.Center => InnerDimensions.X + InnerDimensions.Width * 0.5f - size.X * 0.5f,
					HorizontalAlignment.Right => InnerDimensions.X + InnerDimensions.Width - size.X,
					_ => throw new ArgumentOutOfRangeException()
				};

				// NOTE: there is still some discrepancy with line height 
				line.Position.Y = vAlign switch
				{
					VerticalAlignment.Top => InnerDimensions.Y,
					VerticalAlignment.Center => InnerDimensions.Y + InnerDimensions.Height * 0.5f - totalHeight * 0.5f + top,
					VerticalAlignment.Bottom => InnerDimensions.Y + InnerDimensions.Height - totalHeight + top,
					_ => throw new ArgumentOutOfRangeException()
				};

				top += size.Y;
			}
		}
	}

	private class TextLine(List<TextSnippet> snippets, Vector2 size, Vector2 position)
	{
		public readonly Vector2 Size = size;
		public readonly List<TextSnippet> Snippets = snippets;
		public Vector2 Position = position;
	}
}