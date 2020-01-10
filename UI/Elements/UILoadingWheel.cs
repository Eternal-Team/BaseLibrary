using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader.UI;

namespace BaseLibrary.UI.Elements
{
	public class UILoadingWheel : BaseElement
	{
		private const int MAX_FRAMES = 16;
		private const int MAX_DELAY = 5;

		private int FrameTick;
		private int Frame;
		public bool Enabled = true;

		private readonly float scale;
		private Texture2D texture;

		public UILoadingWheel(float scale = 1f)
		{
			this.scale = scale;
			Width.Pixels = Height.Pixels = (int)(200 * scale);

			texture = typeof(UICommon).GetValue<Texture2D>("LoaderTexture");
		}

		protected override void Draw(SpriteBatch spriteBatch)
		{
			if (!Enabled) return;

			if (++FrameTick >= MAX_DELAY)
			{
				FrameTick = 0;
				if (++Frame >= MAX_FRAMES) Frame = 0;
			}

			spriteBatch.Draw(texture, Dimensions.Position(), new Rectangle(200 * (Frame / 8), 200 * (Frame % 8), 200, 200), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		}
	}
}