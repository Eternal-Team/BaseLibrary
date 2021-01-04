using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace BaseLibrary.Utility
{
	public static class DrawingUtility
	{
		public static void DrawSlot(SpriteBatch spriteBatch, Rectangle dimensions, Texture2D texture, Color? color = null)
		{
			Point point = new Point(dimensions.X, dimensions.Y);
			Point point2 = new Point(point.X + dimensions.Width - 8, point.Y + dimensions.Height - 8);
			int width = point2.X - point.X - 8;
			int height = point2.Y - point.Y - 8;

			Color value = color ?? UICommon.DefaultUIBlue;
			spriteBatch.Draw(texture, new Rectangle(point.X, point.Y, 8, 8), new Rectangle(0, 0, 8, 8), value);
			spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y, 8, 8), new Rectangle(44, 0, 8, 8), value);
			spriteBatch.Draw(texture, new Rectangle(point.X, point2.Y, 8, 8), new Rectangle(0, 44, 8, 8), value);
			spriteBatch.Draw(texture, new Rectangle(point2.X, point2.Y, 8, 8), new Rectangle(44, 44, 8, 8), value);
			spriteBatch.Draw(texture, new Rectangle(point.X + 8, point.Y, width, 8), new Rectangle(8, 0, 36, 8), value);
			spriteBatch.Draw(texture, new Rectangle(point.X + 8, point2.Y, width, 8), new Rectangle(8, 44, 36, 8), value);
			spriteBatch.Draw(texture, new Rectangle(point.X, point.Y + 8, 8, height), new Rectangle(0, 8, 8, 36), value);
			spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y + 8, 8, height), new Rectangle(44, 8, 8, 36), value);
			spriteBatch.Draw(texture, new Rectangle(point.X + 8, point.Y + 8, width, height), new Rectangle(8, 8, 36, 36), value);
		}

		public static void DrawSlot(SpriteBatch spriteBatch, CalculatedStyle dimensions, Color? color = null, Texture2D texture = null)
		{
			DrawSlot(spriteBatch, dimensions.ToRectangle(), texture, color);
		}
	}
}