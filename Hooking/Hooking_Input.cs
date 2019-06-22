using BaseLibrary.UI.Elements;
using Microsoft.Xna.Framework;
using Starbound.Input;
using Terraria;
using Terraria.GameInput;
using Terraria.UI;

namespace BaseLibrary
{
	public static partial class Hooking
	{
		public static void Initialize()
		{
			Utility.Input.InterceptMouseButton += () => UserInterface.ActiveInstance.CurrentState != null;

			MouseEvents.ButtonClicked += (sender, args) =>
			{
				if (UIElement == null) return;

				switch (args.Button)
				{
					case MouseButton.Left:
						UIElement.Click(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.Right:
						UIElement.RightClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.Middle:
						UIElement.MiddleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.XButton1:
						UIElement.XButton1Click(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.XButton2:
						UIElement.XButton2Click(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
				}
			};

			MouseEvents.ButtonDoubleClicked += (sender, args) =>
			{
				if (UIElement == null) return;

				switch (args.Button)
				{
					case MouseButton.Left:
						UIElement.DoubleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.Right:
						UIElement.RightDoubleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.Middle:
						UIElement.MiddleDoubleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.XButton1:
						UIElement.XButton1DoubleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.XButton2:
						UIElement.XButton2DoubleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
				}
			};

			MouseEvents.ButtonTripleClicked += (sender, args) =>
			{
				if (UIElement is BaseElement cast)
				{
					switch (args.Button)
					{
						case MouseButton.Left:
							cast.TripleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
							break;
						case MouseButton.Right:
							cast.RightTripleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
							break;
						case MouseButton.Middle:
							cast.MiddleTripleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
							break;
						case MouseButton.XButton1:
							cast.XButton1TripleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
							break;
						case MouseButton.XButton2:
							cast.XButton2TripleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
							break;
					}
				}
			};

			MouseEvents.ButtonPressed += (sender, args) =>
			{
				if (UIElement == null) return;

				switch (args.Button)
				{
					case MouseButton.Left:
						UIElement.MouseDown(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.Right:
						UIElement.RightMouseDown(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.Middle:
						UIElement.MiddleMouseDown(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.XButton1:
						UIElement.XButton1MouseDown(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.XButton2:
						UIElement.XButton2MouseDown(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
				}
			};

			MouseEvents.ButtonReleased += (sender, args) =>
			{
				if (UIElement == null) return;

				switch (args.Button)
				{
					case MouseButton.Left:
						UIElement?.MouseUp(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.Right:
						UIElement.RightMouseUp(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.Middle:
						UIElement.MiddleMouseUp(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.XButton1:
						UIElement.XButton1MouseUp(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.XButton2:
						UIElement.XButton2MouseUp(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
				}
			};

			MouseEvents.MouseWheelMoved += (sender, args) =>
			{
				UIElement?.ScrollWheel(new UIScrollWheelEvent(UIElement, UserInterface.ActiveInstance.MousePosition, args.Delta));

				PlayerInput.ScrollWheelDelta = 0;
				PlayerInput.ScrollWheelValue += args.Delta;

				PlayerInput.CurrentInputMode = InputMode.Mouse;
				PlayerInput.Triggers.Current.UsedMovementKey = false;
			};

			MouseEvents.MouseMoved += (sender, args) =>
			{
				UserInterface.ActiveInstance.MousePosition = new Vector2(args.Current.X, args.Current.Y);

				if (UIElement != LastElementHover)
				{
					LastElementHover?.MouseOut(new UIMouseEvent(LastElementHover, UserInterface.ActiveInstance.MousePosition));

					UIElement?.MouseOver(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));

					LastElementHover = UIElement;
				}
			};
		}

		private static UIElement UIElement => Main.hasFocus ? UserInterface.ActiveInstance.CurrentState?.GetElementAt(UserInterface.ActiveInstance.MousePosition) : null;

		private static UIElement LastElementHover
		{
			get => UserInterface.ActiveInstance.GetValue<UIElement>("_lastElementHover");
			set => UserInterface.ActiveInstance.SetValue("_lastElementHover", value);
		}
	}
}