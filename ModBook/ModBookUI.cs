using BaseLibrary.UI.Elements;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics;
using Terraria.UI;
using TheOneLibrary.Base.UI;

namespace BaseLibrary.ModBook
{
	public class ModBookUI : BaseUI
	{
		private static readonly Texture2D dividerTexture;

		static ModBookUI()
		{
			dividerTexture = TextureManager.Load("Images/UI/Divider");
		}

		public override void Initialize()
		{
			panelMain.Width.Precent = 0.5f;
			panelMain.Height.Precent = 0.5f;
			panelMain.Center();
			panelMain.OnPostDraw += spriteBatch =>
			{
				CalculatedStyle dimensions = panelMain.GetDimensions();
				spriteBatch.Draw(dividerTexture, dimensions.Position() + new Vector2(dimensions.Width / 2f - 2f, 0), null, Color.White, 90f.ToRadians(), Vector2.Zero, new Vector2(dimensions.Height / 8f, 1f), SpriteEffects.None, 0f);
			};
		}

		public void Load(ModBook modBook)
		{
			int index = 0;
			foreach (Category category in modBook.Categories)
			{
				UIButton categoryButton = new UIButton(category.GetTexture());
				categoryButton.Width.Pixels = 40;
				categoryButton.Height.Pixels = 40;
				categoryButton.Left.Pixels = 8;
				categoryButton.Top.Pixels = 8 + 48 * index++;
				categoryButton.GetHoverText += () => category.Name;
				panelMain.Append(categoryButton);
			}
		}
	}
}