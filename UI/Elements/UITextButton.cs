using System;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
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
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = GetDimensions();
			CalculatedStyle innerDimensions = GetInnerDimensions();

			if (RenderPanel) spriteBatch.DrawPanel(dimensions, IsMouseHovering ? Main.mouseLeft ? Utility.Utility.ColorPanel_Selected : Utility.Utility.ColorPanel_Hovered : Utility.Utility.ColorPanel);

			float scale = Math.Min(innerDimensions.Width / text.Measure().X, innerDimensions.Height / text.Measure().Y);
			Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, text, innerDimensions.X + innerDimensions.Width * 0.5f, innerDimensions.Y + innerDimensions.Height * 0.5f, Color.White, Color.Black, new Vector2(text.Measure().X * 0.5f, 10), scale);
		}
	}
}