using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace BaseLibrary.UI.Elements
{
	public class UIPanel : BaseElement
	{
		public Color BorderColor = Color.Black;
		public Color BackgroundColor = Utility.ColorPanel;

		public Texture2D customTexture;

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = GetDimensions();
			if (customTexture != null) spriteBatch.Draw(customTexture, dimensions);
			else spriteBatch.DrawPanel(dimensions, BackgroundColor, BorderColor);
		}
	}
}