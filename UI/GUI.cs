//using Terraria;
//using Terraria.UI;

//namespace TheOneLibrary.Base.UI
//{

using System;
using Terraria;
using Terraria.UI;
using TheOneLibrary.Base.UI;

namespace BaseLibrary.UI
{
	public interface IGUI
	{
		BaseUI UI { get; set; }
		UserInterface Interface { get; set; }
		LegacyGameInterfaceLayer InterfaceLayer { get; set; }
		bool Draw();
	}

	public class GUI<T> : IGUI where T : BaseUI
	{
		public BaseUI UI { get; set; }
		public UserInterface Interface { get; set; }
		public LegacyGameInterfaceLayer InterfaceLayer { get; set; }

		public GUI(T ui, UserInterface userInterface, InterfaceScaleType scaleType)
		{
			UI = ui;
			Interface = userInterface;
			Type type = ui.GetType();
			InterfaceLayer = new LegacyGameInterfaceLayer($"{type.Assembly.GetName().Name}:{type.Name}", Draw, scaleType);
		}

		public bool Draw()
		{
			Interface.Update(Main._drawInterfaceGameTime);
			UI.Draw(Main.spriteBatch);
			return true;
		}
	}
}