using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace BaseLibrary.Utility
{
	public static class DrawingUtility
	{
		public static class Colors
		{
			public static readonly Color Panel = new Color(73, 94, 171);
			public static readonly Color PanelSelected = new Color(51, 65, 119);
			public static readonly Color PanelDisabled = new Color(71, 78, 102);
			public static readonly Color PanelHovered = new Color(94, 120, 221);
			public static readonly Color Slot = new Color(63, 65, 151);
			public static readonly Color Outline = new Color(18, 18, 38);
		}

		public static Texture2D GetTexturePremultiplied(string path)
		{
			Texture2D texture = ModContent.GetTexture(path).Value;
			Color[] buffer = new Color[texture.Width * texture.Height];
			texture.GetData(buffer);
			for (int i = 0; i < buffer.Length; i++) buffer[i] = Color.FromNonPremultiplied(buffer[i].R, buffer[i].G, buffer[i].B, buffer[i].A);
			texture.SetData(buffer);
			return texture;
		}

		public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Rectangle dimensions)
		{
			spriteBatch.Draw(texture, dimensions, Color.White);
		}

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

		public static void DrawPanel(SpriteBatch spriteBatch, Rectangle dimensions, Texture2D texture, Color color)
		{
			Point point = new Point(dimensions.X, dimensions.Y);
			Point point2 = new Point(point.X + dimensions.Width - 12, point.Y + dimensions.Height - 12);
			int width = point2.X - point.X - 12;
			int height = point2.Y - point.Y - 12;
			spriteBatch.Draw(texture, new Rectangle(point.X, point.Y, 12, 12), new Rectangle(0, 0, 12, 12), color);
			spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y, 12, 12), new Rectangle(16, 0, 12, 12), color);
			spriteBatch.Draw(texture, new Rectangle(point.X, point2.Y, 12, 12), new Rectangle(0, 16, 12, 12), color);
			spriteBatch.Draw(texture, new Rectangle(point2.X, point2.Y, 12, 12), new Rectangle(16, 16, 12, 12), color);
			spriteBatch.Draw(texture, new Rectangle(point.X + 12, point.Y, width, 12), new Rectangle(12, 0, 4, 12), color);
			spriteBatch.Draw(texture, new Rectangle(point.X + 12, point2.Y, width, 12), new Rectangle(12, 16, 4, 12), color);
			spriteBatch.Draw(texture, new Rectangle(point.X, point.Y + 12, 12, height), new Rectangle(0, 12, 12, 4), color);
			spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y + 12, 12, height), new Rectangle(16, 12, 12, 4), color);
			spriteBatch.Draw(texture, new Rectangle(point.X + 12, point.Y + 12, width, height), new Rectangle(12, 12, 4, 4), color);
		}

		public static void DrawPanel(SpriteBatch spriteBatch, Rectangle rectangle, Color? bgColor = null, Color? borderColor = null)
		{
			DrawPanel(spriteBatch, rectangle, Main.Assets.Request<Texture2D>("Images/UI/PanelBackground").Value, bgColor ?? Colors.Panel);
			DrawPanel(spriteBatch, rectangle, Main.Assets.Request<Texture2D>("Images/UI/PanelBorder").Value, borderColor ?? Color.Black);
		}
	}
}