namespace BaseLibrary
{
	public static partial class Hooking
	{
		//private static UIElement UIElement => Main.hasFocus ? UserInterface.ActiveInstance.CurrentState?.GetElementAt(UserInterface.ActiveInstance.MousePosition) : null;

		//private static UIElement LastElementHover
		//{
		//	get => UserInterface.ActiveInstance.GetValue<UIElement>("_lastElementHover");
		//	set => UserInterface.ActiveInstance.SetValue("_lastElementHover", value);
		//}

		//private static GameTime gameTime;

		public static void Initialize()
		{
			//Utility.Input.InterceptMouse += () => UIElement is BaseElement;

			//IL.Terraria.Main.DoUpdate += il =>
			//{
			//	ILCursor cursor = new ILCursor(il);

			//	if (cursor.TryGotoNext(i => i.MatchCall(typeof(Main).GetMethod("DoUpdate_HandleInput", Utility.defaultFlags))))
			//	{
			//		cursor.Emit(OpCodes.Ldarg, 1);
			//		cursor.EmitDelegate<Action<GameTime>>(time => gameTime = time);
			//	}
			//};

			//On.Terraria.UI.UserInterface.Update += (orig, self, time) =>
			//{
			//	//if (Utility.Input.InterceptMouse()) self.CurrentState?.Update(time);
			//	//else
			//		orig(self, time);
			//};

			//PlayerInput.UpdateInput += orig =>
			//{
			//	Utility.Input.Update(gameTime);

			//	orig();
			//};

			//PlayerInput.MouseInput += orig =>
			//{
			//	//if (!Utility.Input.InterceptMouse())
			//		orig();
			//};

			//PlayerInput.KeyboardInput += orig =>
			//{
			//	if (!Utility.Input.InterceptKeyboard()) orig();
			//};

			//MouseEvents.ButtonClicked += ButtonClicked;

			//MouseEvents.ButtonDoubleClicked += ButtonDoubleClicked;

			//MouseEvents.ButtonTripleClicked += ButtonTripleClicked;

			//MouseEvents.ButtonPressed += ButtonPressed;

			//MouseEvents.ButtonReleased += ButtonReleased;

			//MouseEvents.MouseWheelMoved += MouseWheelMoved;

			//MouseEvents.MouseMoved += MouseMoved;

			//MouseEvents.MouseDragged += MouseDragged;
		}

		public static void Uninitialize()
		{
			//MouseEvents.ButtonClicked -= ButtonClicked;

			//MouseEvents.ButtonDoubleClicked -= ButtonDoubleClicked;

			//MouseEvents.ButtonTripleClicked -= ButtonTripleClicked;

			//MouseEvents.ButtonPressed -= ButtonPressed;

			//MouseEvents.ButtonReleased -= ButtonReleased;

			//MouseEvents.MouseWheelMoved -= MouseWheelMoved;

			//MouseEvents.MouseMoved -= MouseMoved;

			//MouseEvents.MouseDragged -= MouseDragged;
		}

		//private static void MouseDragged(object sender, MouseEventArgs args)
		//{
		//	if (UIElement is BaseElement cast) cast.MouseDragged(args);
		//}

		//private static void MouseMoved(object sender, MouseEventArgs args)
		//{
		//	UserInterface.ActiveInstance.MousePosition = new Vector2(args.Current.X, args.Current.Y);

		//	Terraria.GameInput.PlayerInput.MouseX = args.Current.X;
		//	Terraria.GameInput.PlayerInput.MouseY = args.Current.Y;

		//	Terraria.GameInput.PlayerInput.CurrentInputMode = InputMode.Mouse;
		//	Terraria.GameInput.PlayerInput.Triggers.Current.UsedMovementKey = false;

		//	if (UIElement != LastElementHover)
		//	{
		//		LastElementHover?.MouseOut(new UIMouseEvent(LastElementHover, UserInterface.ActiveInstance.MousePosition));

		//		UIElement?.MouseOver(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));

		//		LastElementHover = UIElement;
		//	}
		//}

		//private static void MouseWheelMoved(object sender, MouseWheelEventArgs args)
		//{
		//	UIElement?.ScrollWheel(new UIScrollWheelEvent(UIElement, UserInterface.ActiveInstance.MousePosition, args.Delta));

		//	//if (InUI)
		//	//{
		//	//	InUI = false;
		//	//	return;
		//	//}

		//	//Terraria.GameInput.PlayerInput.ScrollWheelDelta = 0;
		//	//Terraria.GameInput.PlayerInput.ScrollWheelValue -= args.Delta;

		//	//Terraria.GameInput.PlayerInput.CurrentInputMode = InputMode.Mouse;
		//	//Terraria.GameInput.PlayerInput.Triggers.Current.UsedMovementKey = false;
		//}

		//private static void ButtonReleased(object sender, MouseButtonEventArgs args)
		//{
		//	if (UIElement == null) return;

		//	if (BaseElement.PressedButtons.Contains(args.Button)) BaseElement.PressedButtons.Remove(args.Button);

		//	switch (args.Button)
		//	{
		//		case MouseButton.Left:
		//			UIElement.MouseUp(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
		//			break;
		//		case MouseButton.Right:
		//			UIElement.RightMouseUp(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
		//			break;
		//		case MouseButton.Middle:
		//			UIElement.MiddleMouseUp(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
		//			break;
		//		case MouseButton.XButton1:
		//			UIElement.XButton1MouseUp(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
		//			break;
		//		case MouseButton.XButton2:
		//			UIElement.XButton2MouseUp(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
		//			break;
		//	}
		//}

		//private static void ButtonPressed(object sender, MouseButtonEventArgs args)
		//{
		//	if (UIElement == null) return;

		//	if (!BaseElement.PressedButtons.Contains(args.Button)) BaseElement.PressedButtons.Add(args.Button);

		//	switch (args.Button)
		//	{
		//		case MouseButton.Left:
		//			UIElement.MouseDown(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
		//			break;
		//		case MouseButton.Right:
		//			UIElement.RightMouseDown(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
		//			break;
		//		case MouseButton.Middle:
		//			UIElement.MiddleMouseDown(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
		//			break;
		//		case MouseButton.XButton1:
		//			UIElement.XButton1MouseDown(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
		//			break;
		//		case MouseButton.XButton2:
		//			UIElement.XButton2MouseDown(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
		//			break;
		//	}
		//}

		//private static void ButtonTripleClicked(object sender, MouseButtonEventArgs args)
		//{
		//	if (UIElement is BaseElement cast)
		//	{
		//		switch (args.Button)
		//		{
		//			case MouseButton.Left:
		//				cast.TripleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
		//				break;
		//			case MouseButton.Right:
		//				cast.RightTripleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
		//				break;
		//			case MouseButton.Middle:
		//				cast.MiddleTripleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
		//				break;
		//			case MouseButton.XButton1:
		//				cast.XButton1TripleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
		//				break;
		//			case MouseButton.XButton2:
		//				cast.XButton2TripleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
		//				break;
		//		}
		//	}
		//}

		//private static void ButtonDoubleClicked(object sender, MouseButtonEventArgs args)
		//{
		//	if (UIElement == null) return;

		//	switch (args.Button)
		//	{
		//		case MouseButton.Left:
		//			UIElement.DoubleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
		//			break;
		//		case MouseButton.Right:
		//			UIElement.RightDoubleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
		//			break;
		//		case MouseButton.Middle:
		//			UIElement.MiddleDoubleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
		//			break;
		//		case MouseButton.XButton1:
		//			UIElement.XButton1DoubleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
		//			break;
		//		case MouseButton.XButton2:
		//			UIElement.XButton2DoubleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
		//			break;
		//	}
		//}

		//private static void ButtonClicked(object sender, MouseButtonEventArgs args)
		//{
		//	if (UIElement == null) return;

		//	switch (args.Button)
		//	{
		//		case MouseButton.Left:
		//			UIElement.Click(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
		//			break;
		//		case MouseButton.Right:
		//			UIElement.RightClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
		//			break;
		//		case MouseButton.Middle:
		//			UIElement.MiddleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
		//			break;
		//		case MouseButton.XButton1:
		//			UIElement.XButton1Click(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
		//			break;
		//		case MouseButton.XButton2:
		//			UIElement.XButton2Click(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
		//			break;
		//	}
		//}
	}
}