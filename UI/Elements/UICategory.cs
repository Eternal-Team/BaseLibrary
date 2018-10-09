using BaseLibrary.ModBook;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework.Graphics;

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

			panel = new UIPanel
			{
				Width = (0, 1),
				Height = (0, 1),
				Padding = (8, 8, 8, 8)
			};

			UITexture textureIcon = new UITexture(category.GetTexture(), TextureBorder, ScaleMode.Zoom)
			{
				Height = (0, 1),
				Padding = (8, 8, 8, 8),
				substituteWidth = true
			};
			panel.Append(textureIcon);

			UIText textName = new UIText(category.Name)
			{
				Left = (8, 1),
				VAlign = 0.5f
			};
			textureIcon.Append(textName);

			Append(panel);
		}

		public override int CompareTo(object obj) => obj is UICategory ui ? ui.category.Name.CompareTo(category.Name) : base.CompareTo(obj);
	}
}