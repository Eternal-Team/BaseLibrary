﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;

namespace BaseLibrary.UI
{
	public enum HorizontalAlignment
	{
		Left,
		Center,
		Right
	}

	public enum VerticalAlignment
	{
		Top,
		Center,
		Bottom
	}

	public class UIText : BaseElement
	{
		public object Text
		{
			get => text;
			set
			{
				text = value;
				CalculateTextMetrics();
			}
		}

		public Color TextColor = Color.White;
		public Color BorderColor = Color.Black;

		public HorizontalAlignment HorizontalAlignment = HorizontalAlignment.Left;
		public VerticalAlignment VerticalAlignment = VerticalAlignment.Top;

		public bool ScaleToFit;

		private object text;

		private float textScale;
		private Asset<DynamicSpriteFont> font;
		private Vector2 textSize;
		private Vector2 textPosition;

		public UIText(string text, float scale = 1f)
		{
			this.text = text;
			font = scale > 1f ? FontAssets.DeathText : FontAssets.MouseText;
			textScale = scale > 1f ? scale * 0.5f : scale;
		}

		public UIText(LocalizedText text, float scale = 1f)
		{
			this.text = text;
			font = scale > 1f ?FontAssets.DeathText : FontAssets.MouseText;
			textScale = scale > 1f ? scale * 0.5f : scale;
		}

		public UIText(Ref<string> text, float scale = 1f)
		{
			this.text = text;
			font = scale > 1f ?FontAssets.DeathText : FontAssets.MouseText;
			textScale = scale > 1f ? scale * 0.5f : scale;
		}

		public override void Recalculate()
		{
			base.Recalculate();

			CalculateTextMetrics();
		}

		protected override void Draw(SpriteBatch spriteBatch)
		{
			RasterizerState rasterizer = new RasterizerState { CullMode = CullMode.None, ScissorTestEnable = true };
			
			spriteBatch.End();

			SamplerState samplerText = SamplerState.LinearClamp;
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, samplerText, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);
			
			Utils.DrawBorderStringFourWay(spriteBatch, font.Value, text.ToString(), textPosition.X, textPosition.Y, TextColor, BorderColor, Vector2.Zero, textScale);
			
			spriteBatch.End();
			
			SamplerState sampler = SamplerState.PointClamp;
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);
		}

		private void CalculateTextMetrics()
		{
			if (text == null || string.IsNullOrWhiteSpace(text.ToString()))
			{
				textSize = Vector2.Zero;
				textPosition = Vector2.Zero;
				return;
			}

			textSize = font.Value.MeasureString(text.ToString());
			if (ScaleToFit) textScale = Math.Min(InnerDimensions.Width / textSize.X, InnerDimensions.Height / textSize.Y);
			textSize *= textScale;

			if (HorizontalAlignment == HorizontalAlignment.Left) textPosition.X = InnerDimensions.X;
			else if (HorizontalAlignment == HorizontalAlignment.Center) textPosition.X = InnerDimensions.X + InnerDimensions.Width * 0.5f - textSize.X * 0.5f;
			else if (HorizontalAlignment == HorizontalAlignment.Right) textPosition.X = InnerDimensions.X + InnerDimensions.Width - textSize.X;

			if (VerticalAlignment == VerticalAlignment.Top) textPosition.Y = InnerDimensions.Y;
			else if (VerticalAlignment == VerticalAlignment.Center) textPosition.Y = InnerDimensions.Y + InnerDimensions.Height * 0.5f - textSize.Y * 0.5f + 8f * textScale;
			else if (VerticalAlignment == VerticalAlignment.Bottom) textPosition.Y = InnerDimensions.Y + InnerDimensions.Height - textSize.Y + 8f * textScale;
		}
	}
}