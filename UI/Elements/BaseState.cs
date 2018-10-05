using System.Collections.Generic;
using Terraria.UI;

namespace BaseLibrary.UI.Elements
{
	public class BaseState : UIState
	{
		public List<UIElement> Elements
		{
			get => base.Elements;
			set => base.Elements = value;
		}
	}
}