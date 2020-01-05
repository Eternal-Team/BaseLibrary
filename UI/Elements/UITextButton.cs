//using BaseLibrary.UI.New;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using ReLogic.Graphics;
//using System;
//using Terraria;
//using Terraria.ID;
//using Terraria.Localization;
//using Terraria.UI;

//namespace BaseLibrary.UI.Elements
//{
//	public class UITextButton : BaseElement
//	{
//		public string text;
//		public bool RenderPanel = true;
//		public bool Toggleable;
//		public bool Selected;

//		public HorizontalAlignment HorizontalAlignment = HorizontalAlignment.Center;
//		public VerticalAlignment VerticalAlignment = VerticalAlignment.Center;

//		private Vector2 textSize;
//		private Vector2 textPosition;
//		private DynamicSpriteFont font;
//		private float textScale;

//		public UITextButton(string text, float scale = 1f)
//		{
//			this.text = text;
//			SetPadding(8);

//			this.text = text;
//			font = scale > 1f ? Main.fontDeathText : Main.fontMouseText;
//			textScale = scale > 1f ? scale * 0.5f : scale;
//		}

//		public UITextButton(LocalizedText text, float scale = 1f)
//		{
//			this.text = text.ToString();
//			SetPadding(8);

//			this.text = text.ToString();
//			font = scale > 1f ? Main.fontDeathText : Main.fontMouseText;
//			textScale = scale > 1f ? scale * 0.5f : scale;
//		}

//		public override void MouseOver(UIMouseEvent evt)
//		{
//			base.MouseOver(evt);

//			Main.PlaySound(SoundID.MenuTick);
//		}

//		public override void MouseOut(UIMouseEvent evt)
//		{
//			base.MouseOut(evt);

//			Main.PlaySound(SoundID.MenuTick);
//		}

//		public override void Click(UIMouseEvent evt)
//		{
//			base.Click(evt);

//			if (Toggleable) Selected = true;
//			Main.PlaySound(SoundID.MenuTick);
//		}

//		public override void Recalculate()
//		{
//			base.Recalculate();

//			CalculateTextMetrics();
//		}

//		private void CalculateTextMetrics()
//		{
//			textSize = Vector2.Zero;
//			textPosition = Vector2.Zero;
//			textScale = 1f;

//			if (string.IsNullOrWhiteSpace(text)) return;

//			textSize = font.MeasureString(text);
//			textScale = Math.Min(InnerDimensions.Width / textSize.X, InnerDimensions.Height / textSize.Y);

//			textSize *= textScale;

//			if (HorizontalAlignment == HorizontalAlignment.Left) textPosition.X = InnerDimensions.X;
//			else if (HorizontalAlignment == HorizontalAlignment.Center) textPosition.X = InnerDimensions.X + InnerDimensions.Width * 0.5f - textSize.X * 0.5f;
//			else if (HorizontalAlignment == HorizontalAlignment.Right) textPosition.X = InnerDimensions.X + InnerDimensions.Width - textSize.X;

//			if (VerticalAlignment == VerticalAlignment.Top) textPosition.Y = InnerDimensions.Y;
//			else if (VerticalAlignment == VerticalAlignment.Center) textPosition.Y = InnerDimensions.Y + InnerDimensions.Height * 0.5f - textSize.Y * 0.5f + 4f * textScale;
//			else if (VerticalAlignment == VerticalAlignment.Bottom) textPosition.Y = InnerDimensions.Y + InnerDimensions.Height - textSize.Y + 4f * textScale;
//		}

//		protected override void Draw(SpriteBatch spriteBatch)
//		{
//			if (RenderPanel)
//			{
//				if (Toggleable) spriteBatch.DrawPanel(Dimensions, IsMouseHovering ? Utility.ColorPanel_Hovered : Selected ? Utility.ColorPanel : Utility.ColorPanel_Selected);
//				else spriteBatch.DrawPanel(Dimensions, IsMouseHovering ? Main.mouseLeft ? Utility.ColorPanel_Selected : Utility.ColorPanel_Hovered : Utility.ColorPanel);
//			}

//			Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, text, textPosition.X, textPosition.Y, Color.White, Color.Black, Vector2.Zero, textScale);
//		}
//	}
//}

