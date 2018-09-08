using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace BaseLibrary.UI.Elements
{
	public class UIPanel : BaseElement
	{
		public Color BorderColor = Color.Black;
		public Color BackgroundColor = Utility.Utility.ColorPanel;

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = GetDimensions();
			spriteBatch.DrawPanel(dimensions, BackgroundColor, BorderColor);
		}
	}
}