using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.UI;

namespace BaseLibrary.UI.Elements
{
	public class UIText : BaseElement
	{
		private object text = "";
		private float textScale = 1f;
		private Vector2 textSize = Vector2.Zero;
		private bool isLarge;

		public string Text => text.ToString();

		public Color TextColor = Color.White;

		public UIText(string text, float textScale = 1f, bool large = false) => InternalSetText(text, textScale, large);

		public UIText(LocalizedText text, float textScale = 1f, bool large = false) => InternalSetText(text, textScale, large);

		public override void Recalculate()
		{
			InternalSetText(text, textScale, isLarge);
			base.Recalculate();
		}

		public void SetText(string text) => InternalSetText(text, textScale, isLarge);

		public void SetText(LocalizedText text) => InternalSetText(text, textScale, isLarge);

		public void SetText(string text, float textScale, bool large) => InternalSetText(text, textScale, large);

		public void SetText(LocalizedText text, float textScale, bool large) => InternalSetText(text, textScale, large);

		private void InternalSetText(object text, float textScale, bool large)
		{
			DynamicSpriteFont dynamicSpriteFont = large ? Main.fontDeathText : Main.fontMouseText;
			Vector2 textSize = new Vector2(dynamicSpriteFont.MeasureString(text.ToString()).X, large ? 32f : 16f) * textScale;
			this.text = text;
			this.textScale = textScale;
			this.textSize = textSize;
			isLarge = large;
			MinWidth.Set(textSize.X + PaddingLeft + PaddingRight, 0f);
			MinHeight.Set(textSize.Y + PaddingTop + PaddingBottom, 0f);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);
			CalculatedStyle innerDimensions = GetInnerDimensions();
			Vector2 pos = innerDimensions.Position();
			if (isLarge) pos.Y -= 10f * textScale;
			else pos.Y -= 2f * textScale;

			pos.X += (innerDimensions.Width - textSize.X) * 0.5f;

			if (isLarge) Utils.DrawBorderStringBig(spriteBatch, Text, pos, TextColor, textScale);
			else Utils.DrawBorderString(spriteBatch, Text, pos, TextColor, textScale);
		}
	}
}