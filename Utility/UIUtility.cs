using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using System;
using Terraria.UI;

namespace BaseLibrary
{
	public static partial class Utility
	{
		public static GUI<T> SetupGUI<T>() where T : BaseUI
		{
			UserInterface userInterface = new UserInterface();
			T state = (T)Activator.CreateInstance(typeof(T));
			state.Activate();
			userInterface.SetState(state);

			return new GUI<T>(state, userInterface, InterfaceScaleType.UI);
		}

		public static void Center(this UIElement element)
		{
			element.HAlign = 0.5f;
			element.VAlign = 0.5f;
		}

		public static Vector2 Size(this CalculatedStyle dimensions) => new Vector2(dimensions.Width, dimensions.Height);
	}
}