using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace BaseLibrary.UI.Elements
{
	public class UIColorSelection : BaseElement
	{
		private Texture2D texture;

		public Color GetColor()
		{
			CalculatedStyle dimensions = GetDimensions();

			int x = (int)(Main.mouseX - dimensions.X);
			int y = (int)(Main.mouseY - dimensions.Y);

			Color[] data = new Color[texture.Width * texture.Height];
			texture.GetData(data);

			Vector2 scale = new Vector2(dimensions.Width / texture.Width, dimensions.Height / texture.Height);

			return data[(int)(y / scale.Y) * texture.Width + (int)(x / scale.X)];
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = GetDimensions();
			if (texture == null) texture = Utility.Utility.CreateGrad((int)dimensions.Width - 4, (int)dimensions.Height - 4);

			spriteBatch.Draw(Main.magicPixel, dimensions.Position(), null, Color.Black, 0f, Vector2.Zero, new Vector2(dimensions.Width, dimensions.Height / 1000f), SpriteEffects.None, 0f);
			spriteBatch.Draw(texture, dimensions.Position() + new Vector2(2, 2), null, Color.White, 0f, Vector2.Zero, new Vector2((dimensions.Width - 4f) / texture.Width, (dimensions.Height - 4f) / texture.Height), SpriteEffects.None, 0f);
		}
	}
}