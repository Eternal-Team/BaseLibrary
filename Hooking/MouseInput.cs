using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;

namespace BaseLibrary;

public static class MouseInput
{
	private struct LastClick
	{
		public TimeSpan Time;
		public Vector2 Position;

		public LastClick(TimeSpan timeSinceLast, Vector2 position)
		{
			Time = timeSinceLast;
			Position = position;
		}
	}

	public static int DoubleClickTime { get; set; }

	public static int DoubleClickMaxMove { get; set; }

	public static bool MoveRaisedOnDrag { get; set; } = true;

	private static GameTime time;

	private static MouseState previous;

	public static MouseState Mouse;

	public static KeyboardState Keyboard;

	private static Dictionary<MouseButton, LastClick> lastClicks;

	private static Dictionary<MouseButton, LastClick> lastDoubleClicks;

	public static event Action<MouseButtonEventArgs> ButtonReleased;

	public static event Action<MouseButtonEventArgs> ButtonPressed;

	public static event Action<MouseButtonEventArgs> ButtonClicked;

	public static event Action<MouseButtonEventArgs> ButtonDoubleClicked;

	public static event Action<MouseButtonEventArgs> ButtonTripleClicked;

	public static event Action<MouseMoveEventArgs> MouseMoved;

	public static event Action<MouseEventArgs> MouseDragged;

	public static event Action<MouseScrollEventArgs> MouseScroll;

	internal static void Load()
	{
		DoubleClickTime = 300;
		DoubleClickMaxMove = 2;
		MoveRaisedOnDrag = true;

		lastClicks = new Dictionary<MouseButton, LastClick>
		{
			{ MouseButton.Left, new LastClick(TimeSpan.Zero, Vector2.Zero) },
			{ MouseButton.Right, new LastClick(TimeSpan.Zero, Vector2.Zero) },
			{ MouseButton.Middle, new LastClick(TimeSpan.Zero, Vector2.Zero) },
			{ MouseButton.XButton1, new LastClick(TimeSpan.Zero, Vector2.Zero) },
			{ MouseButton.XButton2, new LastClick(TimeSpan.Zero, Vector2.Zero) }
		};

		lastDoubleClicks = new Dictionary<MouseButton, LastClick>
		{
			{ MouseButton.Left, new LastClick(TimeSpan.Zero, Vector2.Zero) },
			{ MouseButton.Right, new LastClick(TimeSpan.Zero, Vector2.Zero) },
			{ MouseButton.Middle, new LastClick(TimeSpan.Zero, Vector2.Zero) },
			{ MouseButton.XButton1, new LastClick(TimeSpan.Zero, Vector2.Zero) },
			{ MouseButton.XButton2, new LastClick(TimeSpan.Zero, Vector2.Zero) }
		};
	}

	public static IEnumerable<MouseButton> GetHeldButtons()
	{
		if (Mouse.LeftButton == ButtonState.Pressed) yield return MouseButton.Left;
		if (Mouse.RightButton == ButtonState.Pressed) yield return MouseButton.Right;
		if (Mouse.MiddleButton == ButtonState.Pressed) yield return MouseButton.Middle;
		if (Mouse.XButton1 == ButtonState.Pressed) yield return MouseButton.XButton1;
		if (Mouse.XButton2 == ButtonState.Pressed) yield return MouseButton.XButton2;
	}

	public static bool IsMouseDown(MouseButton button)
	{
		switch (button)
		{
			case MouseButton.Left:
				return Mouse.LeftButton == ButtonState.Pressed;
			case MouseButton.Middle:
				return Mouse.MiddleButton == ButtonState.Pressed;
			case MouseButton.Right:
				return Mouse.RightButton == ButtonState.Pressed;
			case MouseButton.XButton1:
				return Mouse.XButton1 == ButtonState.Pressed;
			case MouseButton.XButton2:
				return Mouse.XButton2 == ButtonState.Pressed;
		}

		return false;
	}

	internal static void Update(GameTime gameTime)
	{
		if (!Main.instance.IsActive || !Main.hasFocus) return;

		time = gameTime;

		Mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
		Keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();

		Modifiers modifiers = KeyboardUtil.GetModifiers(Keyboard);
		Vector2 position = new Vector2(Mouse.X, Mouse.Y);

		if (Mouse.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released)
			OnButtonPressed(new MouseButtonEventArgs(position, MouseButton.Left, modifiers));

		if (Mouse.MiddleButton == ButtonState.Pressed && previous.MiddleButton == ButtonState.Released)
			OnButtonPressed(new MouseButtonEventArgs(position, MouseButton.Middle, modifiers));

		if (Mouse.RightButton == ButtonState.Pressed && previous.RightButton == ButtonState.Released)
			OnButtonPressed(new MouseButtonEventArgs(position, MouseButton.Right, modifiers));

		if (Mouse.XButton1 == ButtonState.Pressed && previous.XButton1 == ButtonState.Released)
			OnButtonPressed(new MouseButtonEventArgs(position, MouseButton.XButton1, modifiers));

		if (Mouse.XButton2 == ButtonState.Pressed && previous.XButton2 == ButtonState.Released)
			OnButtonPressed(new MouseButtonEventArgs(position, MouseButton.XButton2, modifiers));

		if (Mouse.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released)
			OnButtonClicked(new MouseButtonEventArgs(position, MouseButton.Left, modifiers));

		if (Mouse.MiddleButton == ButtonState.Pressed && previous.MiddleButton == ButtonState.Released)
			OnButtonClicked(new MouseButtonEventArgs(position, MouseButton.Middle, modifiers));

		if (Mouse.RightButton == ButtonState.Pressed && previous.RightButton == ButtonState.Released)
			OnButtonClicked(new MouseButtonEventArgs(position, MouseButton.Right, modifiers));

		if (Mouse.XButton1 == ButtonState.Pressed && previous.XButton1 == ButtonState.Released)
			OnButtonClicked(new MouseButtonEventArgs(position, MouseButton.XButton1, modifiers));

		if (Mouse.XButton2 == ButtonState.Pressed && previous.XButton2 == ButtonState.Released)
			OnButtonClicked(new MouseButtonEventArgs(position, MouseButton.XButton2, modifiers));

		if (Mouse.LeftButton == ButtonState.Released && previous.LeftButton == ButtonState.Pressed)
			OnButtonReleased(new MouseButtonEventArgs(position, MouseButton.Left, modifiers));

		if (Mouse.MiddleButton == ButtonState.Released && previous.MiddleButton == ButtonState.Pressed)
			OnButtonReleased(new MouseButtonEventArgs(position, MouseButton.Middle, modifiers));

		if (Mouse.RightButton == ButtonState.Released && previous.RightButton == ButtonState.Pressed)
			OnButtonReleased(new MouseButtonEventArgs(position, MouseButton.Right, modifiers));

		if (Mouse.XButton1 == ButtonState.Released && previous.XButton1 == ButtonState.Pressed)
			OnButtonReleased(new MouseButtonEventArgs(position, MouseButton.XButton1, modifiers));

		if (Mouse.XButton2 == ButtonState.Released && previous.XButton2 == ButtonState.Pressed)
			OnButtonReleased(new MouseButtonEventArgs(position, MouseButton.XButton2, modifiers));

		bool buttonDown = Mouse.LeftButton == ButtonState.Pressed || Mouse.MiddleButton == ButtonState.Pressed || Mouse.RightButton == ButtonState.Pressed || Mouse.XButton1 == ButtonState.Pressed || Mouse.XButton2 == ButtonState.Pressed;

		if (previous.X != Mouse.X || previous.Y != Mouse.Y)
		{
			if (buttonDown)
				OnMouseDragged(new MouseEventArgs(position));

			if (MoveRaisedOnDrag || !buttonDown)
				OnMouseMoved(new MouseMoveEventArgs(position, new Vector2(Mouse.X - previous.X, Mouse.Y - previous.Y)));
		}

		if (previous.ScrollWheelValue != Mouse.ScrollWheelValue)
		{
			int value = Mouse.ScrollWheelValue;
			int delta = Mouse.ScrollWheelValue - previous.ScrollWheelValue;
			OnMouseScroll(new MouseScrollEventArgs(position, new Vector2(0, delta)));
		}

		previous = Mouse;
	}

	private static void OnButtonClicked(MouseButtonEventArgs args)
	{
		TimeSpan lastDoubleClick = lastDoubleClicks[args.Button].Time;

		if ((time.TotalGameTime - lastDoubleClick).TotalMilliseconds < DoubleClickTime && Vector2.Distance(args.Position, lastDoubleClicks[args.Button].Position) < DoubleClickMaxMove)
		{
			ButtonTripleClicked?.Invoke(args);
			return;
		}

		TimeSpan lastClick = lastClicks[args.Button].Time;
		if ((time.TotalGameTime - lastClick).TotalMilliseconds < DoubleClickTime && Vector2.Distance(args.Position, lastClicks[args.Button].Position) < DoubleClickMaxMove)
		{
			ButtonDoubleClicked?.Invoke(args);
			lastDoubleClicks[args.Button] = new LastClick(time.TotalGameTime, args.Position);
			return;
		}

		lastClicks[args.Button] = new LastClick(time.TotalGameTime, args.Position);
		ButtonClicked?.Invoke(args);
	}

	private static void OnButtonPressed(MouseButtonEventArgs args) => ButtonPressed?.Invoke(args);

	private static void OnButtonReleased(MouseButtonEventArgs args) => ButtonReleased?.Invoke(args);

	private static void OnMouseMoved(MouseMoveEventArgs args) => MouseMoved?.Invoke(args);

	private static void OnMouseDragged(MouseEventArgs args) => MouseDragged?.Invoke(args);

	private static void OnMouseScroll(MouseScrollEventArgs args) => MouseScroll?.Invoke(args);
}