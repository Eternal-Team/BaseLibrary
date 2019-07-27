using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.Localization;

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

		public UIText(Ref<string> text, float textScale = 1f, bool large = false) => InternalSetText(text, textScale, large);

		public override void Recalculate()
		{
			InternalSetText(text, textScale, isLarge);
			base.Recalculate();
		}

		private void InternalSetText(object text, float textScale, bool large)
		{
			this.text = text;

			DynamicSpriteFont dynamicSpriteFont = large ? Main.fontDeathText : Main.fontMouseText;
			textSize = new Vector2(dynamicSpriteFont.MeasureString(Text).X, large ? 32f : 16f) * textScale;
			this.textScale = textScale;
			isLarge = large;
			MinWidth.Set(textSize.X + PaddingLeft + PaddingRight, 0f);
			MinHeight.Set(textSize.Y + PaddingTop + PaddingBottom, 0f);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);
			Vector2 pos = InnerDimensions.Position();
			if (isLarge) pos.Y -= 10f * textScale;
			else pos.Y -= 2f * textScale;

			pos.X += (InnerDimensions.Width - textSize.X) * 0.5f;

			if (isLarge) Utils.DrawBorderStringBig(spriteBatch, Text, pos, TextColor, textScale);
			else Utils.DrawBorderString(spriteBatch, Text, pos, TextColor, textScale);
		}
	}
}