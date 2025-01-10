using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;

namespace BaseLibrary.Input;

public enum MouseButton
{
	Left,
	Middle,
	Right,
	XButton1,
	XButton2
}

[Flags]
public enum Modifiers
{
	None = 0,
	LeftControl = 1,
	RightControl = 2,
	LeftShift = 4,
	RightShift = 8,
	LeftAlt = 16,
	RightAlt = 32
}

public static class MouseInput
{
	private struct LastClick(TimeSpan timeSinceLast, Vector2 position)
	{
		public readonly TimeSpan Time = timeSinceLast;
		public readonly Vector2 Position = position;
	}

	public static event Action<MouseButtonEventArgs>? ButtonReleased;
	public static event Action<MouseButtonEventArgs>? ButtonPressed;
	public static event Action<MouseButtonEventArgs>? ButtonClicked;
	public static event Action<MouseButtonEventArgs>? ButtonDoubleClicked;
	public static event Action<MouseButtonEventArgs>? ButtonTripleClicked;
	public static event Action<MouseMoveEventArgs>? MouseMoved;
	public static event Action<MouseEventArgs>? MouseDragged;
	public static event Action<MouseScrollEventArgs>? MouseScroll;

	private static GameTime time;
	private static Dictionary<MouseButton, LastClick> lastClicks;
	private static Dictionary<MouseButton, LastClick> lastDoubleClicks;
	private static int DoubleClickTime { get; set; }
	private static int DoubleClickMaxMove { get; set; }
	private static bool MoveRaisedOnDrag { get; set; } = true;

	private static MouseState previousMouseState;
	internal static MouseState currentMouseState;

	internal static KeyboardState Keyboard;

	internal static void Load()
	{
		DoubleClickTime = 300;
		DoubleClickMaxMove = 2;
		MoveRaisedOnDrag = true;

		lastClicks = new Dictionary<MouseButton, LastClick> {
			{ MouseButton.Left, new LastClick(TimeSpan.Zero, Vector2.Zero) },
			{ MouseButton.Right, new LastClick(TimeSpan.Zero, Vector2.Zero) },
			{ MouseButton.Middle, new LastClick(TimeSpan.Zero, Vector2.Zero) },
			{ MouseButton.XButton1, new LastClick(TimeSpan.Zero, Vector2.Zero) },
			{ MouseButton.XButton2, new LastClick(TimeSpan.Zero, Vector2.Zero) }
		};

		lastDoubleClicks = new Dictionary<MouseButton, LastClick> {
			{ MouseButton.Left, new LastClick(TimeSpan.Zero, Vector2.Zero) },
			{ MouseButton.Right, new LastClick(TimeSpan.Zero, Vector2.Zero) },
			{ MouseButton.Middle, new LastClick(TimeSpan.Zero, Vector2.Zero) },
			{ MouseButton.XButton1, new LastClick(TimeSpan.Zero, Vector2.Zero) },
			{ MouseButton.XButton2, new LastClick(TimeSpan.Zero, Vector2.Zero) }
		};
	}

	public static IEnumerable<MouseButton> GetHeldButtons()
	{
		if (currentMouseState.LeftButton == ButtonState.Pressed) yield return MouseButton.Left;
		if (currentMouseState.RightButton == ButtonState.Pressed) yield return MouseButton.Right;
		if (currentMouseState.MiddleButton == ButtonState.Pressed) yield return MouseButton.Middle;
		if (currentMouseState.XButton1 == ButtonState.Pressed) yield return MouseButton.XButton1;
		if (currentMouseState.XButton2 == ButtonState.Pressed) yield return MouseButton.XButton2;
	}

	public static bool IsMouseDown(MouseButton button) => button switch {
		MouseButton.Left => currentMouseState.LeftButton == ButtonState.Pressed,
		MouseButton.Middle => currentMouseState.MiddleButton == ButtonState.Pressed,
		MouseButton.Right => currentMouseState.RightButton == ButtonState.Pressed,
		MouseButton.XButton1 => currentMouseState.XButton1 == ButtonState.Pressed,
		MouseButton.XButton2 => currentMouseState.XButton2 == ButtonState.Pressed,
		_ => false
	};

	internal static void Update(GameTime gameTime)
	{
		if (!Main.instance.IsActive || !Main.hasFocus) return;

		time = gameTime;

		currentMouseState = Mouse.GetState();
		Keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();

		Modifiers modifiers = KeyboardUtil.GetModifiers(Keyboard);
		Vector2 position = new(currentMouseState.X, currentMouseState.Y);

		if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
			OnButtonPressed(new MouseButtonEventArgs(position, MouseButton.Left, modifiers));

		if (currentMouseState.MiddleButton == ButtonState.Pressed && previousMouseState.MiddleButton == ButtonState.Released)
			OnButtonPressed(new MouseButtonEventArgs(position, MouseButton.Middle, modifiers));

		if (currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Released)
			OnButtonPressed(new MouseButtonEventArgs(position, MouseButton.Right, modifiers));

		if (currentMouseState.XButton1 == ButtonState.Pressed && previousMouseState.XButton1 == ButtonState.Released)
			OnButtonPressed(new MouseButtonEventArgs(position, MouseButton.XButton1, modifiers));

		if (currentMouseState.XButton2 == ButtonState.Pressed && previousMouseState.XButton2 == ButtonState.Released)
			OnButtonPressed(new MouseButtonEventArgs(position, MouseButton.XButton2, modifiers));

		if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
			OnButtonClicked(new MouseButtonEventArgs(position, MouseButton.Left, modifiers));

		if (currentMouseState.MiddleButton == ButtonState.Pressed && previousMouseState.MiddleButton == ButtonState.Released)
			OnButtonClicked(new MouseButtonEventArgs(position, MouseButton.Middle, modifiers));

		if (currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Released)
			OnButtonClicked(new MouseButtonEventArgs(position, MouseButton.Right, modifiers));

		if (currentMouseState.XButton1 == ButtonState.Pressed && previousMouseState.XButton1 == ButtonState.Released)
			OnButtonClicked(new MouseButtonEventArgs(position, MouseButton.XButton1, modifiers));

		if (currentMouseState.XButton2 == ButtonState.Pressed && previousMouseState.XButton2 == ButtonState.Released)
			OnButtonClicked(new MouseButtonEventArgs(position, MouseButton.XButton2, modifiers));

		if (currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed)
			OnButtonReleased(new MouseButtonEventArgs(position, MouseButton.Left, modifiers));

		if (currentMouseState.MiddleButton == ButtonState.Released && previousMouseState.MiddleButton == ButtonState.Pressed)
			OnButtonReleased(new MouseButtonEventArgs(position, MouseButton.Middle, modifiers));

		if (currentMouseState.RightButton == ButtonState.Released && previousMouseState.RightButton == ButtonState.Pressed)
			OnButtonReleased(new MouseButtonEventArgs(position, MouseButton.Right, modifiers));

		if (currentMouseState.XButton1 == ButtonState.Released && previousMouseState.XButton1 == ButtonState.Pressed)
			OnButtonReleased(new MouseButtonEventArgs(position, MouseButton.XButton1, modifiers));

		if (currentMouseState.XButton2 == ButtonState.Released && previousMouseState.XButton2 == ButtonState.Pressed)
			OnButtonReleased(new MouseButtonEventArgs(position, MouseButton.XButton2, modifiers));

		bool buttonDown = currentMouseState.LeftButton == ButtonState.Pressed || currentMouseState.MiddleButton == ButtonState.Pressed || currentMouseState.RightButton == ButtonState.Pressed || currentMouseState.XButton1 == ButtonState.Pressed || currentMouseState.XButton2 == ButtonState.Pressed;

		if (previousMouseState.X != currentMouseState.X || previousMouseState.Y != currentMouseState.Y)
		{
			if (buttonDown)
				OnMouseDragged(new MouseEventArgs(position));

			if (MoveRaisedOnDrag || !buttonDown)
				OnMouseMoved(new MouseMoveEventArgs(position, new Vector2(currentMouseState.X - previousMouseState.X, currentMouseState.Y - previousMouseState.Y)));
		}

		if (previousMouseState.ScrollWheelValue != currentMouseState.ScrollWheelValue)
		{
			int value = currentMouseState.ScrollWheelValue;
			int delta = currentMouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue;
			OnMouseScroll(Keyboard.IsKeyDown(Keys.LeftShift) || Keyboard.IsKeyDown(Keys.RightShift) ? new MouseScrollEventArgs(position, new Vector2(delta, 0)) : new MouseScrollEventArgs(position, new Vector2(0, delta)));
		}

		previousMouseState = currentMouseState;
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