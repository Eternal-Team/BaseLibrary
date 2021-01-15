using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace BaseLibrary.UI
{
	public class UIDivider : BaseElement
	{
		private static Asset<Texture2D> textureDivider;

		public UIDivider()
		{
			if (textureDivider == null) textureDivider = ModContent.GetTexture("Terraria/Images/UI/Divider");
			Height.Pixels = 4;
		}

		protected override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(textureDivider.Value, Dimensions, Color.White);
		}
	}
}