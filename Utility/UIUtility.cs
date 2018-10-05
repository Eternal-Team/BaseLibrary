using System;
using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Terraria.UI;
using TheOneLibrary.Base.UI;

namespace BaseLibrary.Utility
{
	public static partial class Utility
	{
		public static GUI<T> SetupGUI<T>(InterfaceScaleType? scaleType = null, params object[] args) where T : BaseUI
		{
			UserInterface userInterface = new UserInterface();
			T state = (T)Activator.CreateInstance(typeof(T), args);
			state.Activate();
			userInterface.SetState(state);

			return new GUI<T>(state, userInterface, scaleType ?? InterfaceScaleType.UI);
		}

		public static void Center(this UIElement element)
		{
			element.HAlign = 0.5f;
			element.VAlign = 0.5f;
		}

		public static Vector2 Size(this CalculatedStyle dimensions) => new Vector2(dimensions.Width, dimensions.Height);

		public static RectangleF ToRectangleF(this CalculatedStyle dimensions) => new RectangleF(dimensions.X, dimensions.Y, dimensions.Width, dimensions.Height);
	}
}