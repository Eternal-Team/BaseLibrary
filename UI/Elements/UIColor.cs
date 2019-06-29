using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace BaseLibrary.UI.Elements
{
	public class UIColor : BaseElement
	{
		public Color color;

		public UIColor(Color color) => this.color = color;

		protected override void DrawSelf(SpriteBatch spriteBatch) => spriteBatch.Draw(Main.magicPixel, Dimensions.ToRectangle(), color);
	}
}