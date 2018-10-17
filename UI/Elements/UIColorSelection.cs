using Microsoft.Xna.Framework;

namespace BaseLibrary.UI.Elements
{
	public enum Channel
	{
		R,
		G,
		B,
		A,
		All
	}

	public class UIColorSelection : BaseElement
	{
		public UIColorSelection(int steps, Channel channel = Channel.All)
		{
			UIColor uiColor = new UIColor(Color.Black);
			uiColor.Width = uiColor.Height = (0, 1);
			Append(uiColor);

			UIGradient gradient = new UIGradient(steps, channel);
			gradient.Width = gradient.Height = (-4, 1);
			gradient.Position = new Vector2(2);
			gradient.OnChangeColor += newColor => uiColor.color = newColor;
			uiColor.Append(gradient);
		}
	}
}