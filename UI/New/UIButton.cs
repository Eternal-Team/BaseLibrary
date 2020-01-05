using BaseLibrary.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;

namespace BaseLibrary.UI.New
{
	public class UIButton : BaseElement
	{
		public Texture2D texture;
		public bool RenderPanel;

		private ScaleMode scaleMode;
		public Rectangle? sourceRectangle;

		private bool mouseDown;

		public UIButton(Texture2D texture = null, Rectangle? sourceRectangle = null, ScaleMode scaleMode = ScaleMode.Stretch)
		{
			this.texture = texture;
			this.scaleMode = scaleMode;
			this.sourceRectangle = sourceRectangle;
		}

		protected override void MouseEnter(MouseEventArgs args)
		{
			base.MouseEnter(args);

			Main.PlaySound(SoundID.MenuTick);
		}

		protected override void MouseLeave(MouseEventArgs args)
		{
			base.MouseLeave(args);

			Main.PlaySound(SoundID.MenuTick);
		}

		protected override void MouseDown(MouseButtonEventArgs args)
		{
			base.MouseDown(args);

			args.Handled = true;
			mouseDown = true;
		}

		protected override void MouseUp(MouseButtonEventArgs args)
		{
			base.MouseUp(args);

			args.Handled = true;
			mouseDown = false;
		}

		protected override void Draw(SpriteBatch spriteBatch)
		{
			if (RenderPanel) spriteBatch.DrawPanel(Dimensions, IsMouseHovering ? mouseDown ? Utility.ColorPanel_Selected : Utility.ColorPanel_Hovered : Utility.ColorPanel);

			if (texture != null)
			{
				if (scaleMode == ScaleMode.Stretch) spriteBatch.Draw(texture, InnerDimensions, sourceRectangle, Color.White);
				else if (scaleMode == ScaleMode.Zoom) spriteBatch.Draw(texture, InnerDimensions.Center(), sourceRectangle, Color.White, 0f, texture.Size() * 0.5f, Math.Min(InnerDimensions.Width / texture.Width, InnerDimensions.Height / texture.Height), SpriteEffects.None, 0f);
				else if (scaleMode == ScaleMode.None) spriteBatch.Draw(texture, InnerDimensions.TopLeft(), sourceRectangle, Color.White);
			}
		}
	}
}