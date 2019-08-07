using Microsoft.Xna.Framework;

namespace BaseLibrary.UI.Elements
{
	public class UIColorSelection : BaseElement
	{
		public UIColorSelection(Ref<Color> color)
		{
			UIColor uiColor = new UIColor(color.Value);
			uiColor.Width = uiColor.Height = (0, 1);
			uiColor.SetPadding(2);
			Append(uiColor);

			UIGradient gradient = new UIGradient();
			gradient.Width = gradient.Height = (0, 1);
			gradient.OnChangeColor += newColor => color.Value = uiColor.color = newColor;
			uiColor.Append(gradient);
		}
	}
}