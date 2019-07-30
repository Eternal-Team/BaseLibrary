using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System.Linq;
using Terraria;
using Terraria.Localization;
using Terraria.UI;

namespace BaseLibrary.UI.Elements
{
	public class UIMultilineText : BaseElement
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

		private object text;
		private string actualText;

		private float textScale;
		private DynamicSpriteFont font;
		private Vector2 textSize;
		private Vector2 textPosition;

		public UIScrollbar scrollbar;
		private float innerListHeight;

		public Color TextColor = Color.White;
		public Color BorderColor = Color.Black;

		public HorizontalAlignment HorizontalAlignment = HorizontalAlignment.Left;
		public VerticalAlignment VerticalAlignment = VerticalAlignment.Top;

		public bool DrawBackground;

		public UIMultilineText(string text, float scale = 1f)
		{
			Padding = (0, 2, 2, 0);

			this.text = text;
			font = scale > 1f ? Main.fontDeathText : Main.fontMouseText;
			textScale = scale > 1f ? scale * 0.5f : scale;

			scrollbar = new UIScrollbar();
			scrollbar.SetView(100, 1000);
		}

		public UIMultilineText(LocalizedText text, float scale = 1f)
		{
			Padding = (0, 2, 2, 0);

			this.text = text;
			font = scale > 1f ? Main.fontDeathText : Main.fontMouseText;
			textScale = scale > 1f ? scale * 0.5f : scale;

			scrollbar = new UIScrollbar();
			scrollbar.SetView(100, 1000);
		}

		public UIMultilineText(Ref<string> text, float scale = 1f)
		{
			Padding = (0, 2, 2, 0);

			this.text = text;
			font = scale > 1f ? Main.fontDeathText : Main.fontMouseText;
			textScale = scale > 1f ? scale * 0.5f : scale;

			scrollbar = new UIScrollbar();
			scrollbar.SetView(100, 1000);
		}

		public override void Recalculate()
		{
			base.Recalculate();

			CalculateTextMetrics();
		}

		public override void ScrollWheel(UIScrollWheelEvent evt)
		{
			base.ScrollWheel(evt);

			if (scrollbar != null) scrollbar.ViewPosition -= evt.ScrollWheelValue * 0.5f;
		}

		private void CalculateTextMetrics()
		{
			textSize = Vector2.Zero;
			textPosition = Vector2.Zero;
			actualText = "";

			if (text == null || string.IsNullOrWhiteSpace(text.ToString())) return;

			string[] textArray = Utility.WrapText(text.ToString(), InnerDimensions.Width, font).ToArray();
			foreach (string line in textArray)
			{
				Vector2 size = font.MeasureString(line);
				if (size.X > textSize.X) textSize.X = size.X;
				textSize.Y += size.Y;
			}

			textSize *= textScale;

			if (HorizontalAlignment == HorizontalAlignment.Left) textPosition.X = InnerDimensions.X;
			else if (HorizontalAlignment == HorizontalAlignment.Center) textPosition.X = InnerDimensions.X + InnerDimensions.Width * 0.5f - textSize.X * 0.5f;
			else if (HorizontalAlignment == HorizontalAlignment.Right) textPosition.X = InnerDimensions.X + InnerDimensions.Width - textSize.X;

			if (VerticalAlignment == VerticalAlignment.Top) textPosition.Y = InnerDimensions.Y;
			else if (VerticalAlignment == VerticalAlignment.Center) textPosition.Y = InnerDimensions.Y + InnerDimensions.Height * 0.5f - textSize.Y * 0.5f;
			else if (VerticalAlignment == VerticalAlignment.Bottom) textPosition.Y = InnerDimensions.Y + InnerDimensions.Height - textSize.Y;

			actualText = textArray.Concat("\n");

			innerListHeight = textSize.Y;

			scrollbar?.SetView(InnerDimensions.Height, innerListHeight);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (DrawBackground) spriteBatch.DrawPanel(Dimensions.ToRectangle(), BaseLibrary.texturePanelBackground, Utility.ColorPanel_Selected * 0.75f);

			spriteBatch.Draw(Utility.ImmediateState, GetClippingRectangle(spriteBatch), () => Utils.DrawBorderStringFourWay(spriteBatch, font, actualText, textPosition.X, textPosition.Y - scrollbar.GetValue(), TextColor, BorderColor, Vector2.Zero, textScale));
		}
	}
}