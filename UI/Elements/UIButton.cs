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
		public ScaleMode scaleMode;
		public bool RenderPanel;

		public UIButton(Texture2D texture = null, ScaleMode scaleMode = ScaleMode.Stretch)
		{
			this.texture = texture;
			this.scaleMode = scaleMode;
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
				if (scaleMode == ScaleMode.Stretch) spriteBatch.Draw(texture, Dimensions);
				else if (scaleMode == ScaleMode.Zoom) spriteBatch.Draw(texture, Dimensions.Center(), null, Color.White, 0f, texture.Size() * 0.5f, Math.Min(InnerDimensions.Width / texture.Width, InnerDimensions.Height / texture.Height), SpriteEffects.None, 0f);
				else if (scaleMode == ScaleMode.None) spriteBatch.Draw(texture, Dimensions.Position());
			}
		}
	}
}