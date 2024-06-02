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
		Font = FontAssets.MouseText,
		// ScaleToFit = false,
		// Ellipsis = false
	};

	public Color TextColor;
	public Color BorderColor;
	public HorizontalAlignment HorizontalAlignment;
	public VerticalAlignment VerticalAlignment;
	public Asset<DynamicSpriteFont> Font;

	// public bool ScaleToFit;
	// public bool Ellipsis;
}

public class UIText : BaseElement
{
	public UITextOptions Settings = UITextOptions.Default;

	public float TotalHeight { get; private set; }

	public object? Text
	{
		get => text;
		set
		{
			text = value;
			dirty = true;
			CalculateTextMetrics();
		}
	}

	private bool dirty = true;
	private readonly List<TextLine> _snippets = [];
	private readonly float textScale;
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

	public override void Recalculate()
	{
		int previousWidth = InnerDimensions.Width;

		base.Recalculate();

		if (previousWidth != InnerDimensions.Width) dirty = true;

		CalculateTextMetrics();
	}

	private string? previousText;
	
	private void CalculateTextMetrics()
	{
		string? actualText = text?.ToString();
		if (string.IsNullOrWhiteSpace(actualText)) return;

		if (previousText != actualText)
		{
			previousText = actualText;
			dirty = true;
		}
		
		if (!dirty) return;
		if (InnerDimensions.Width <= 0) return;

		dirty = false;

		_snippets.Clear();

		HorizontalAlignment hAlign = Settings.HorizontalAlignment;
		VerticalAlignment vAlign = Settings.VerticalAlignment;

		TotalHeight = 0;
		foreach (List<TextSnippet> snippets in Utils.WordwrapStringSmart(actualText, Settings.TextColor, Settings.Font.Value, InnerDimensions.Width, -1))
		{
			Vector2 size = ChatManager.GetStringSize(Settings.Font.Value, snippets.ToArray(), new Vector2(textScale));
			_snippets.Add(new TextLine(snippets, size, Vector2.Zero));
			TotalHeight += size.Y;
		}

		TotalHeight -= 8f;

		float top = 0;
		foreach (TextLine line in _snippets)
		{
			Vector2 size = line.Size;
			line.Position.X = hAlign switch
			{
				HorizontalAlignment.Left => 0,
				HorizontalAlignment.Center => InnerDimensions.Width * 0.5f - size.X * 0.5f,
				HorizontalAlignment.Right => InnerDimensions.Width - size.X,
				_ => throw new ArgumentOutOfRangeException()
			};

			line.Position.Y = vAlign switch
			{
				VerticalAlignment.Top => top,
				VerticalAlignment.Center => InnerDimensions.Height * 0.5f - TotalHeight * 0.5f + top,
				VerticalAlignment.Bottom => InnerDimensions.Height - TotalHeight + top,
				_ => throw new ArgumentOutOfRangeException()
			};

			top += size.Y;
		}
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
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Settings.Font.Value, snippets.Snippets.ToArray(), InnerDimensions.TopLeft() + snippets.Position, 0f, Settings.TextColor, Settings.BorderColor, Vector2.Zero, new Vector2(textScale), out int _);
		}

		spriteBatch.End();

		SamplerState sampler = SamplerState.PointClamp;
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);
	}

	private class TextLine(List<TextSnippet> snippets, Vector2 size, Vector2 position)
	{
		public readonly Vector2 Size = size;
		public readonly List<TextSnippet> Snippets = snippets;
		public Vector2 Position = position;
	}
}