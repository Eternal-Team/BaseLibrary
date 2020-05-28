using BaseLibrary.Input.Keyboard;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Terraria;

namespace BaseLibrary.Input.Mouse
{
	public static class MouseInput
	{
		private static GameTime time;

		private static MouseState previous;
		public static MouseState Mouse;
		public static KeyboardState Keyboard;

		private static Dictionary<MouseButton, (TimeSpan Time, Vector2 Position)> lastClicks;
		private static Dictionary<MouseButton, (TimeSpan Time, Vector2 Position)> lastDoubleClicks;

		public static int DoubleClickTime { get; set; }

		public static int DoubleClickMaxMove { get; set; }

		public static bool MoveRaisedOnDrag { get; set; } = true;

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

			lastClicks = new Dictionary<MouseButton, (TimeSpan, Vector2)>
			{
				{ MouseButton.Left, (TimeSpan.Zero, Vector2.Zero) },
				{ MouseButton.Right, (TimeSpan.Zero, Vector2.Zero) },
				{ MouseButton.Middle, (TimeSpan.Zero, Vector2.Zero) },
				{ MouseButton.XButton1, (TimeSpan.Zero, Vector2.Zero) },
				{ MouseButton.XButton2, (TimeSpan.Zero, Vector2.Zero) }
			};

			lastDoubleClicks = new Dictionary<MouseButton, (TimeSpan, Vector2)>
			{
				{ MouseButton.Left, (TimeSpan.Zero, Vector2.Zero) },
				{ MouseButton.Right, (TimeSpan.Zero, Vector2.Zero) },
				{ MouseButton.Middle, (TimeSpan.Zero, Vector2.Zero) },
				{ MouseButton.XButton1, (TimeSpan.Zero, Vector2.Zero) },
				{ MouseButton.XButton2, (TimeSpan.Zero, Vector2.Zero) }
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
				OnButtonPressed(new MouseButtonEventArgs
				{
					Button = MouseButton.Left,
					Modifiers = modifiers,
					Position = position
				});

			if (Mouse.MiddleButton == ButtonState.Pressed && previous.MiddleButton == ButtonState.Released)
				OnButtonPressed(new MouseButtonEventArgs
				{
					Button = MouseButton.Middle,
					Modifiers = modifiers,
					Position = position
				});

			if (Mouse.RightButton == ButtonState.Pressed && previous.RightButton == ButtonState.Released)
				OnButtonPressed(new MouseButtonEventArgs
				{
					Button = MouseButton.Right,
					Modifiers = modifiers,
					Position = position
				});

			if (Mouse.XButton1 == ButtonState.Pressed && previous.XButton1 == ButtonState.Released)
				OnButtonPressed(new MouseButtonEventArgs
				{
					Button = MouseButton.XButton1,
					Modifiers = modifiers,
					Position = position
				});

			if (Mouse.XButton2 == ButtonState.Pressed && previous.XButton2 == ButtonState.Released)
				OnButtonPressed(new MouseButtonEventArgs
				{
					Button = MouseButton.XButton2,
					Modifiers = modifiers,
					Position = position
				});

			if (Mouse.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released)
				OnButtonClicked(new MouseButtonEventArgs
				{
					Button = MouseButton.Left,
					Modifiers = modifiers,
					Position = position
				});

			if (Mouse.MiddleButton == ButtonState.Pressed && previous.MiddleButton == ButtonState.Released)
				OnButtonClicked(new MouseButtonEventArgs
				{
					Button = MouseButton.Middle,
					Modifiers = modifiers,
					Position = position
				});

			if (Mouse.RightButton == ButtonState.Pressed && previous.RightButton == ButtonState.Released)
				OnButtonClicked(new MouseButtonEventArgs
				{
					Button = MouseButton.Right,
					Modifiers = modifiers,
					Position = position
				});

			if (Mouse.XButton1 == ButtonState.Pressed && previous.XButton1 == ButtonState.Released)
				OnButtonClicked(new MouseButtonEventArgs
				{
					Button = MouseButton.XButton1,
					Modifiers = modifiers,
					Position = position
				});

			if (Mouse.XButton2 == ButtonState.Pressed && previous.XButton2 == ButtonState.Released)
				OnButtonClicked(new MouseButtonEventArgs
				{
					Button = MouseButton.XButton2,
					Modifiers = modifiers,
					Position = position
				});

			if (Mouse.LeftButton == ButtonState.Released && previous.LeftButton == ButtonState.Pressed)
				OnButtonReleased(new MouseButtonEventArgs
				{
					Button = MouseButton.Left,
					Modifiers = modifiers,
					Position = position
				});

			if (Mouse.MiddleButton == ButtonState.Released && previous.MiddleButton == ButtonState.Pressed)
				OnButtonReleased(new MouseButtonEventArgs
				{
					Button = MouseButton.Middle,
					Modifiers = modifiers,
					Position = position
				});

			if (Mouse.RightButton == ButtonState.Released && previous.RightButton == ButtonState.Pressed)
				OnButtonReleased(new MouseButtonEventArgs
				{
					Button = MouseButton.Right,
					Modifiers = modifiers,
					Position = position
				});

			if (Mouse.XButton1 == ButtonState.Released && previous.XButton1 == ButtonState.Pressed)
				OnButtonReleased(new MouseButtonEventArgs
				{
					Button = MouseButton.XButton1,
					Modifiers = modifiers,
					Position = position
				});

			if (Mouse.XButton2 == ButtonState.Released && previous.XButton2 == ButtonState.Pressed)
				OnButtonReleased(new MouseButtonEventArgs
				{
					Button = MouseButton.XButton2,
					Modifiers = modifiers,
					Position = position
				});

			bool buttonDown = Mouse.LeftButton == ButtonState.Pressed || Mouse.MiddleButton == ButtonState.Pressed || Mouse.RightButton == ButtonState.Pressed || Mouse.XButton1 == ButtonState.Pressed || Mouse.XButton2 == ButtonState.Pressed;

			if (previous.X != Mouse.X || previous.Y != Mouse.Y)
			{
				if (buttonDown)
					OnMouseDragged(new MouseEventArgs
					{
						Position = position
					});

				if (MoveRaisedOnDrag || !buttonDown)
					OnMouseMoved(new MouseMoveEventArgs
					{
						Position = position,
						Delta = new Vector2(Mouse.X - previous.X, Mouse.Y - previous.Y)
					});
			}

			if (previous.ScrollWheelValue != Mouse.ScrollWheelValue)
			{
				int value = Mouse.ScrollWheelValue;
				int delta = Mouse.ScrollWheelValue - previous.ScrollWheelValue;
				OnMouseScroll(new MouseScrollEventArgs
				{
					Position = position,
					Offset = new Vector2(0, delta)
				});
			}

			previous = Mouse;
		}

		private static float DistanceBetween(Vector2 a, Vector2 b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

		private static void OnButtonClicked(MouseButtonEventArgs args)
		{
			TimeSpan lastDoubleClick = lastDoubleClicks[args.Button].Time;
			if ((time.TotalGameTime - lastDoubleClick).TotalMilliseconds < DoubleClickTime && DistanceBetween(args.Position, lastDoubleClicks[args.Button].Position) < DoubleClickMaxMove)
			{
				ButtonTripleClicked?.Invoke(args);
				return;
			}

			TimeSpan lastClick = lastClicks[args.Button].Time;
			if ((time.TotalGameTime - lastClick).TotalMilliseconds < DoubleClickTime && DistanceBetween(args.Position, lastClicks[args.Button].Position) < DoubleClickMaxMove)
			{
				ButtonDoubleClicked?.Invoke(args);
				lastDoubleClicks[args.Button] = (time.TotalGameTime, args.Position);
				return;
			}

			lastClicks[args.Button] = (time.TotalGameTime, args.Position);
			ButtonClicked?.Invoke(args);
		}

		private static void OnButtonPressed(MouseButtonEventArgs args) => ButtonPressed?.Invoke(args);

		private static void OnButtonReleased(MouseButtonEventArgs args) => ButtonReleased?.Invoke(args);

		private static void OnMouseMoved(MouseMoveEventArgs args) => MouseMoved?.Invoke(args);

		private static void OnMouseDragged(MouseEventArgs args) => MouseDragged?.Invoke(args);

		private static void OnMouseScroll(MouseScrollEventArgs args) => MouseScroll?.Invoke(args);
	}
}