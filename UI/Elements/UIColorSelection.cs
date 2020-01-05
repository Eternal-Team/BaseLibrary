using BaseLibrary.UI.New;
using Microsoft.Xna.Framework;

namespace BaseLibrary.UI.Elements
{
	public class UIColorSelection : BaseElement
	{
		public UIColorSelection(Ref<Color> color)
		{
			UIColor uiColor = new UIColor(color.Value);
			uiColor.Width.Percent = uiColor.Height.Percent = 100;
			uiColor.Padding = new Padding(2);
			Add(uiColor);

			UIGradient gradient = new UIGradient();
			gradient.Width.Percent = gradient.Height.Percent = 100;
			gradient.OnChangeColor += newColor => color.Value = uiColor.color = newColor;
			uiColor.Add(gradient);
		}
	}
}