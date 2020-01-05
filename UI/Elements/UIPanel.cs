//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Terraria;

//namespace BaseLibrary.UI.Elements
//{
//	public class UIPanel : BaseElement
//	{
//		public Color BorderColor = Color.Black;
//		public Color BackgroundColor = Utility.ColorPanel;

//		public Texture2D customTexture;

//		public UIPanel() => SetPadding(8);

//		protected override void Draw(SpriteBatch spriteBatch)
//		{
//			if (customTexture != null) spriteBatch.Draw(customTexture, Dimensions);
//			else spriteBatch.DrawPanel(Dimensions, BackgroundColor, BorderColor);

//			if (IsMouseHovering)
//			{
//				Main.LocalPlayer.showItemIcon = false;
//				Main.ItemIconCacheUpdate(0);
//			}
//		}
//	}
//}

