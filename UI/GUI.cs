//using Terraria;
//using Terraria.UI;

//namespace TheOneLibrary.Base.UI
//{
//	public interface IGUI
//	{
//		BaseUI GetUI();
//		UserInterface GetInterface();
//		LegacyGameInterfaceLayer GetInterfaceLayer();
//		bool Draw();
//	}

//	public class GUI<T> : IGUI where T : BaseUI
//	{
//		public T UI;
//		public UserInterface UserInterface;
//		public LegacyGameInterfaceLayer InterfaceLayer;

//		public GUI(T ui, UserInterface userInterface, InterfaceScaleType scaleType = InterfaceScaleType.UI)
//		{
//			UI = ui;
//			UserInterface = userInterface;
//			InterfaceLayer = new LegacyGameInterfaceLayer($"{ui.GetType().Assembly.GetName().Name}:{ui.GetType().Name}", Draw, scaleType);
//		}

//		public bool Draw()
//		{
//			//if (UI.Visible)
//			//{
//			UserInterface.Update(Main._drawInterfaceGameTime);
//			UI.Draw(Main.spriteBatch);
//			//}

//			return true;
//		}

//		public BaseUI GetUI() => UI;

//		public UserInterface GetInterface() => UserInterface;

//		public LegacyGameInterfaceLayer GetInterfaceLayer() => InterfaceLayer;
//	}
//}

