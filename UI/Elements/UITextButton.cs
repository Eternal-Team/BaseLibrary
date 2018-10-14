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
		public float padding;
		public bool RenderPanel = true;

		public UITextButton(string text, float padding = 8f)
		{
			this.text = text;
			this.padding = padding;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = GetDimensions();

			if (RenderPanel) spriteBatch.DrawPanel(dimensions, IsMouseHovering ? Main.mouseLeft ? Utility.Utility.ColorPanel_Selected : Utility.Utility.ColorPanel_Hovered : Utility.Utility.ColorPanel);

			float scale = Math.Min((dimensions.Width - padding * 2f) / text.Measure().X, (dimensions.Height - padding * 2f) / text.Measure().Y);

			Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, text, dimensions.X + dimensions.Width * 0.5f, dimensions.Y + dimensions.Height * 0.5f, Color.White, Color.Black, new Vector2(text.Measure().X * 0.5f, 10), scale);
		}
	}
}