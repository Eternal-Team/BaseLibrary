using Microsoft.Xna.Framework;
using Terraria;

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

	// todo: create Texture2D on GPU and send it to CPU

	public class UIColorSelection : BaseElement
	{
		public UIColorSelection(Ref<Color> color, int steps, Channel channel = Channel.All)
		{
			UIColor uiColor = new UIColor(color.Value);
			uiColor.Width = uiColor.Height = (0, 1);
			Append(uiColor);

			UIGradient gradient = new UIGradient(steps, channel);
			gradient.Width = gradient.Height = (-4, 1);
			gradient.Position = new Vector2(2);
			gradient.OnChangeColor += newColor => color.Value = uiColor.color = newColor;
			uiColor.Append(gradient);
		}
	}
}