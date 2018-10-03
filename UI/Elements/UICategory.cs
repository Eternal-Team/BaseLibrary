using BaseLibrary.ModBook;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;

namespace BaseLibrary.UI.Elements
{
	internal class UICategory : BaseElement
	{
		[PathOverride("Terraria/UI/Achievement_Borders")]
		public static Texture2D TextureBorder { get; set; }

		private Category category;

		private UIPanel panel;

		public UICategory(Category category)
		{
			this.category = category;

			panel = new UIPanel();
			panel.Width.Precent = panel.Height.Precent = 1f;
			panel.SetPadding(8f);

			UITexture textureIcon = new UITexture(category.GetTexture(), TextureBorder, ScaleMode.Zoom);
			textureIcon.SetPadding(4f);
			textureIcon.Height.Set(0f, 1f);
			textureIcon.substituteWidth = true;
			panel.Append(textureIcon);

			UIText textName = new UIText(category.Name);
			textName.Left.Set(8f, 1f);
			textName.VAlign = 0.5f;
			textureIcon.Append(textName);

			Append(panel);
		}

		public override int CompareTo(object obj) => obj is UICategory ui ? ui.category.Name.CompareTo(category.Name) : base.CompareTo(obj);
	}
}