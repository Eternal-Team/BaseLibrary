using System;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace BaseLibrary.UI.Elements
{
	public class UIButton : BaseElement
	{
		public Texture2D texture;
		public ScaleMode scaleMode;

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
			CalculatedStyle dimensions = GetDimensions();
			if (texture != null)
			{
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

				switch (scaleMode)
				{
					case ScaleMode.Stretch:
						spriteBatch.Draw(texture, dimensions);
						break;
					case ScaleMode.Zoom:
						spriteBatch.Draw(texture, dimensions.Position(), null, Color.White, 0f, Vector2.Zero, Math.Min(dimensions.Width / texture.Width, dimensions.Height / texture.Height), SpriteEffects.None, 0f);
						break;
					case ScaleMode.None:
						spriteBatch.Draw(texture, dimensions.Position());
						break;
				}

				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			}
		}
	}
}