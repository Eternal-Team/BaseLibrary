using BaseLibrary.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.Localization;

namespace BaseLibrary.UI.New
{
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
		private DynamicSpriteFont font;
		private Vector2 textSize;
		private Vector2 textPosition;

		public UIText(string text, float scale = 1f)
		{
			Height.Pixels = 20;

			this.text = text;
			font = scale > 1f ? Main.fontDeathText : Main.fontMouseText;
			textScale = scale > 1f ? scale * 0.5f : scale;
		}

		public UIText(LocalizedText text, float scale = 1f)
		{
			Height.Pixels = 20;

			this.text = text;
			font = scale > 1f ? Main.fontDeathText : Main.fontMouseText;
			textScale = scale > 1f ? scale * 0.5f : scale;
		}

		public UIText(Ref<string> text, float scale = 1f)
		{
			Height.Pixels = 20;

			this.text = text;
			font = scale > 1f ? Main.fontDeathText : Main.fontMouseText;
			textScale = scale > 1f ? scale * 0.5f : scale;
		}

		public override void Recalculate()
		{
			base.Recalculate();

			CalculateTextMetrics();
		}

		protected override void Draw(SpriteBatch spriteBatch)
		{
			Utils.DrawBorderStringFourWay(spriteBatch, font, text.ToString(), textPosition.X, textPosition.Y, TextColor, BorderColor, Vector2.Zero, textScale);
		}

		private void CalculateTextMetrics()
		{
			if (text == null || string.IsNullOrWhiteSpace(text.ToString()))
			{
				textSize = Vector2.Zero;
				textPosition = Vector2.Zero;
				return;
			}

			textSize = font.MeasureString(text.ToString());
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