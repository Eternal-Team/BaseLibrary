using BaseLibrary.Input;
using BaseLibrary.Input.Mouse;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;

namespace BaseLibrary.UI.Elements
{
	public class UIToggleButton : BaseElement
	{
		public Texture2D texture;
		public ScaleMode scaleMode;
		public bool Toggled;

		public UIToggleButton(Texture2D texture = null, ScaleMode scaleMode = ScaleMode.Stretch)
		{
			this.texture = texture;
			this.scaleMode = scaleMode;
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

		protected override void MouseClick(MouseButtonEventArgs args)
		{
			if (args.Button != MouseButton.Left) return;

			args.Handled = true;

			Toggled = !Toggled;
		}

		protected override void Draw(SpriteBatch spriteBatch)
		{
			if (texture == null) return;

			spriteBatch.DrawPanel(Dimensions, IsMouseHovering ? Utility.ColorPanel_Hovered : Toggled ? Utility.ColorPanel_Selected : Utility.ColorPanel);

			switch (scaleMode)
			{
				case ScaleMode.Stretch:
					spriteBatch.Draw(texture, InnerDimensions);
					break;
				case ScaleMode.Zoom:
					spriteBatch.Draw(texture, Dimensions.Center(), null, Color.White, 0f, texture.Size() * 0.5f, Math.Min(InnerDimensions.Width / texture.Width, InnerDimensions.Height / texture.Height), SpriteEffects.None, 0f);
					break;
				case ScaleMode.None:
					spriteBatch.Draw(texture, InnerDimensions.Position());
					break;
			}
		}
	}
}