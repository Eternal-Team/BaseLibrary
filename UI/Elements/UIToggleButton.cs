using System;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.UI;

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

		public override void Click(UIMouseEvent evt)
		{
			base.Click(evt);

			Toggled = !Toggled;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (texture == null) return;

			CalculatedStyle dimensions = GetDimensions();
			CalculatedStyle innerDimensions = GetInnerDimensions();

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

			spriteBatch.DrawPanel(dimensions, IsMouseHovering ? Utility.Utility.ColorPanel_Hovered : Toggled ? Utility.Utility.ColorPanel_Selected : Utility.Utility.ColorPanel);

			if (scaleMode == ScaleMode.Stretch) spriteBatch.Draw(texture, innerDimensions);
			else if (scaleMode == ScaleMode.Zoom) spriteBatch.Draw(texture, dimensions.Center(), null, Color.White, 0f, texture.Size() * 0.5f, Math.Min(innerDimensions.Width / texture.Width, innerDimensions.Height / texture.Height), SpriteEffects.None, 0f);
			else if (scaleMode == ScaleMode.None) spriteBatch.Draw(texture, innerDimensions.Position());

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
		}
	}
}