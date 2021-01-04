using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader.UI;

namespace BaseLibrary.UI
{
	public class UIPanel : BaseElement
	{
		public Color BackgroundColor = UICommon.DefaultUIBlue;
		public Color BorderColor = Color.Black;

		// public Texture2D customTexture;

		public UIPanel()
		{
			Padding = new Padding(8);
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

		public static readonly Color ColorPanel = new Color(73, 94, 171);
		public static readonly Color ColorPanel_Selected = new Color(51, 65, 119);
		public static readonly Color ColorPanel_Hovered = new Color(94, 120, 221);
		public static readonly Color ColorSlot = new Color(63, 65, 151);
		public static readonly Color ColorOutline = new Color(18, 18, 38);

		public static void DrawPanel(SpriteBatch spriteBatch, Rectangle rectangle, Color? bgColor = null, Color? borderColor = null)
		{
			DrawPanel(spriteBatch, rectangle, Main.Assets.Request<Texture2D>("Images/UI/PanelBackground").Value, bgColor ?? ColorPanel);
			DrawPanel(spriteBatch, rectangle, Main.Assets.Request<Texture2D>("Images/UI/PanelBorder").Value, borderColor ?? Color.Black);
		}

		protected override void Draw(SpriteBatch spriteBatch)
		{
			// if (customTexture != null) spriteBatch.Draw(customTexture, Dimensions);
			// else
			DrawPanel(spriteBatch, Dimensions, BackgroundColor, BorderColor);

			if (IsMouseHovering)
			{
				Main.LocalPlayer.cursorItemIconEnabled = false;
				Main.ItemIconCacheUpdate(0);
			}
		}
	}
}