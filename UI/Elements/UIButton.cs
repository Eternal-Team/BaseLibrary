using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace BaseLibrary.UI.Elements
{
	public class UIButton : BaseElement
	{
		public Texture2D texture;
		public bool RenderPanel;

		private ScaleMode scaleMode;
		public Rectangle? sourceRectangle;

		public UIButton(Texture2D texture = null, Rectangle? sourceRectangle = null, ScaleMode scaleMode = ScaleMode.Stretch)
		{
			this.texture = texture;
			this.scaleMode = scaleMode;
			this.sourceRectangle = sourceRectangle;
		}

		public override void MouseOver(UIMouseEvent evt)
		{
			base.MouseOver(evt);

			Main.PlaySound(SoundID.MenuTick);
		}

		public override void MouseOut(UIMouseEvent evt)
		{
			base.MouseOut(evt);

			Main.PlaySound(SoundID.MenuTick);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (RenderPanel) spriteBatch.DrawPanel(Dimensions, IsMouseHovering ? Main.mouseLeft ? Utility.ColorPanel_Selected : Utility.ColorPanel_Hovered : Utility.ColorPanel);

			if (texture != null)
			{
				if (scaleMode == ScaleMode.Stretch) spriteBatch.Draw(texture, InnerDimensions.ToRectangle(), sourceRectangle, Color.White);
				else if (scaleMode == ScaleMode.Zoom) spriteBatch.Draw(texture, InnerDimensions.Center(), sourceRectangle, Color.White, 0f, texture.Size() * 0.5f, Math.Min(InnerDimensions.Width / texture.Width, InnerDimensions.Height / texture.Height), SpriteEffects.None, 0f);
				else if (scaleMode == ScaleMode.None) spriteBatch.Draw(texture, InnerDimensions.Position(), sourceRectangle, Color.White);
			}
		}
	}
}