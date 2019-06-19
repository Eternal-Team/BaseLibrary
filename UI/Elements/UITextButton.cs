using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace BaseLibrary.UI.Elements
{
	public class UITextButton : BaseElement
	{
		public string text;
		public bool RenderPanel = true;

		public UITextButton(string text)
		{
			this.text = text;
			SetPadding(8);
		}

		public override void MouseOver(UIMouseEvent evt)
		{
			base.MouseOver(evt);

			Main.PlaySound(SoundID.MenuTick);
		}

		public override void MouseOut(UIMouseEvent evt)
		{
			base.MouseOut(evt);

			Main.PlaySound(SoundID.MenuTick);
		}

		public override void Click(UIMouseEvent evt)
		{
			base.Click(evt);

			Main.PlaySound(SoundID.MenuTick);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle innerDimensions = InnerDimensions;

			if (RenderPanel) spriteBatch.DrawPanel(Dimensions, IsMouseHovering ? Main.mouseLeft ? Utility.ColorPanel_Selected : Utility.ColorPanel_Hovered : Utility.ColorPanel);
			else innerDimensions = Dimensions;

			float scale = Math.Min(innerDimensions.Width / text.Measure().X, innerDimensions.Height / text.Measure().Y);
			Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, text, innerDimensions.X + innerDimensions.Width * 0.5f, innerDimensions.Y + innerDimensions.Height * 0.5f, Color.White, Color.Black, new Vector2(text.Measure().X * 0.5f, 10), scale);
		}
	}
}