using Microsoft.Xna.Framework;
using Terraria.UI;

namespace BaseLibrary.Utility
{
	public static partial class Utility
	{
		//public static GUI<T> SetupGUI<T>(/*object obj = null*/) where T : BaseUI
		//{
		//	UserInterface userInterface = new UserInterface();
		//	T state = Activator.CreateInstance<T>();
		//	//(state as ITileEntityUI)?.SetTileEntity(obj as ModTileEntity);
		//	//(state as IContainerUI)?.SetContainer(obj as IContainer);
		//	state.Activate();
		//	userInterface.SetState(state);

		//	return new GUI<T>(state, userInterface);
		//}

		public static void Center(this UIElement element)
		{
			element.HAlign = 0.5f;
			element.VAlign = 0.5f;
		}

		public static Vector2 Size(this CalculatedStyle dimensions) => new Vector2(dimensions.Width, dimensions.Height);
	}
}