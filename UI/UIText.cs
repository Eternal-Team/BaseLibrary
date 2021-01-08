using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

	public struct UITextOptions
	{
		public static readonly UITextOptions Default = new UITextOptions
		{
			TextColor = Color.White,
			BorderColor = Color.Black,
			HorizontalAlignment = HorizontalAlignment.Left,
			VerticalAlignment = VerticalAlignment.Top,
			ScaleToFit = false,
			Font = FontAssets.MouseText.Value
		};

		public Color TextColor;
		public Color BorderColor;
		public HorizontalAlignment HorizontalAlignment;
		public VerticalAlignment VerticalAlignment;
		public bool ScaleToFit;
		public DynamicSpriteFont Font;
	}

	public class UIText : BaseElement
	{
		public UITextOptions Settings = UITextOptions.Default;

		public object Text
		{
			get => text;
			set
			{
				text = value;
				CalculateTextMetrics();
			}
		}

		private object text;

		private float textScale;
		private Vector2 textSize;
		private Vector2 textPosition;

		public UIText(string text, float scale = 1f)
		{
			this.text = text;
			Settings.Font = scale > 1f ? FontAssets.DeathText.Value : FontAssets.MouseText.Value;
			textScale = scale > 1f ? scale * 0.5f : scale;
		}

		public UIText(LocalizedText text, float scale = 1f)
		{
			this.text = text;
			Settings.Font = scale > 1f ? FontAssets.DeathText.Value : FontAssets.MouseText.Value;
			textScale = scale > 1f ? scale * 0.5f : scale;
		}

		public UIText(Ref<string> text, float scale = 1f)
		{
			this.text = text;
			Settings.Font = scale > 1f ? FontAssets.DeathText.Value : FontAssets.MouseText.Value;
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

			Utils.DrawBorderStringFourWay(spriteBatch, Settings.Font, text.ToString(), textPosition.X, textPosition.Y, Settings.TextColor, Settings.BorderColor, Vector2.Zero, textScale);

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

			textSize = Settings.Font.MeasureString(text.ToString());
			if (Settings.ScaleToFit) textScale = Math.Min(InnerDimensions.Width / textSize.X, InnerDimensions.Height / textSize.Y);
			textSize *= textScale;

			var hAlign = Settings.HorizontalAlignment;
			var vAlign = Settings.VerticalAlignment;

			if (hAlign == HorizontalAlignment.Left) textPosition.X = InnerDimensions.X;
			else if (hAlign == HorizontalAlignment.Center) textPosition.X = InnerDimensions.X + InnerDimensions.Width * 0.5f - textSize.X * 0.5f;
			else if (hAlign == HorizontalAlignment.Right) textPosition.X = InnerDimensions.X + InnerDimensions.Width - textSize.X;

			if (vAlign == VerticalAlignment.Top) textPosition.Y = InnerDimensions.Y;
			else if (vAlign == VerticalAlignment.Center) textPosition.Y = InnerDimensions.Y + InnerDimensions.Height * 0.5f - textSize.Y * 0.5f + 8f * textScale;
			else if (vAlign == VerticalAlignment.Bottom) textPosition.Y = InnerDimensions.Y + InnerDimensions.Height - textSize.Y + 8f * textScale;
		}
	}
}