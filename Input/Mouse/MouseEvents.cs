using BaseLibrary.Input.Keyboard;
using BaseLibrary.UI.New;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Terraria;

namespace BaseLibrary.Input.Mouse
{
	public static class MouseEvents
	{
		private static MouseState previous;

		private static Dictionary<MouseButton, (TimeSpan Time, Vector2Int Position)> lastClicks;

		private static Dictionary<MouseButton, (TimeSpan Time, Vector2Int Position)> lastDoubleClicks;

		private static GameTime time;

		public static int DoubleClickTime { get; set; }

		public static int DoubleClickMaxMove { get; set; }

		public static bool MoveRaisedOnDrag { get; set; }

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

			lastClicks = new Dictionary<MouseButton, (TimeSpan, Vector2Int)>
			{
				{ MouseButton.Left, (TimeSpan.Zero, Vector2Int.Zero) },
				{ MouseButton.Right, (TimeSpan.Zero, Vector2Int.Zero) },
				{ MouseButton.Middle, (TimeSpan.Zero, Vector2Int.Zero) },
				{ MouseButton.XButton1, (TimeSpan.Zero, Vector2Int.Zero) },
				{ MouseButton.XButton2, (TimeSpan.Zero, Vector2Int.Zero) }
			};

			lastDoubleClicks = new Dictionary<MouseButton, (TimeSpan, Vector2Int)>
			{
				{ MouseButton.Left, (TimeSpan.Zero, Vector2Int.Zero) },
				{ MouseButton.Right, (TimeSpan.Zero, Vector2Int.Zero) },
				{ MouseButton.Middle, (TimeSpan.Zero, Vector2Int.Zero) },
				{ MouseButton.XButton1, (TimeSpan.Zero, Vector2Int.Zero) },
				{ MouseButton.XButton2, (TimeSpan.Zero, Vector2Int.Zero) }
			};
		}

		internal static void Update(GameTime gameTime)
		{
			if (!Main.instance.IsActive || !Main.hasFocus) return;

			time = gameTime;

			MouseState mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
			KeyboardState keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();

			Modifiers modifiers = KeyboardUtil.GetModifiers(keyboard);
			Vector2Int position = new Vector2Int(mouse.X, mouse.Y);

			if (mouse.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released)
				OnButtonPressed(new MouseButtonEventArgs
				{
					Button = MouseButton.Left,
					Modifiers = modifiers,
					Position = position
				});

			if (mouse.MiddleButton == ButtonState.Pressed && previous.MiddleButton == ButtonState.Released)
				OnButtonPressed(new MouseButtonEventArgs
				{
					Button = MouseButton.Middle,
					Modifiers = modifiers,
					Position = position
				});

			if (mouse.RightButton == ButtonState.Pressed && previous.RightButton == ButtonState.Released)
				OnButtonPressed(new MouseButtonEventArgs
				{
					Button = MouseButton.Right,
					Modifiers = modifiers,
					Position = position
				});

			if (mouse.XButton1 == ButtonState.Pressed && previous.XButton1 == ButtonState.Released)
				OnButtonPressed(new MouseButtonEventArgs
				{
					Button = MouseButton.XButton1,
					Modifiers = modifiers,
					Position = position
				});

			if (mouse.XButton2 == ButtonState.Pressed && previous.XButton2 == ButtonState.Released)
				OnButtonPressed(new MouseButtonEventArgs
				{
					Button = MouseButton.XButton2,
					Modifiers = modifiers,
					Position = position
				});

			if (mouse.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released)
				OnButtonClicked(new MouseButtonEventArgs
				{
					Button = MouseButton.Left,
					Modifiers = modifiers,
					Position = position
				});

			if (mouse.MiddleButton == ButtonState.Pressed && previous.MiddleButton == ButtonState.Released)
				OnButtonClicked(new MouseButtonEventArgs
				{
					Button = MouseButton.Middle,
					Modifiers = modifiers,
					Position = position
				});

			if (mouse.RightButton == ButtonState.Pressed && previous.RightButton == ButtonState.Released)
				OnButtonClicked(new MouseButtonEventArgs
				{
					Button = MouseButton.Right,
					Modifiers = modifiers,
					Position = position
				});

			if (mouse.XButton1 == ButtonState.Pressed && previous.XButton1 == ButtonState.Released)
				OnButtonClicked(new MouseButtonEventArgs
				{
					Button = MouseButton.XButton1,
					Modifiers = modifiers,
					Position = position
				});

			if (mouse.XButton2 == ButtonState.Pressed && previous.XButton2 == ButtonState.Released)
				OnButtonClicked(new MouseButtonEventArgs
				{
					Button = MouseButton.XButton2,
					Modifiers = modifiers,
					Position = position
				});

			if (mouse.LeftButton == ButtonState.Released && previous.LeftButton == ButtonState.Pressed)
				OnButtonReleased(new MouseButtonEventArgs
				{
					Button = MouseButton.Left,
					Modifiers = modifiers,
					Position = position
				});

			if (mouse.MiddleButton == ButtonState.Released && previous.MiddleButton == ButtonState.Pressed)
				OnButtonReleased(new MouseButtonEventArgs
				{
					Button = MouseButton.Middle,
					Modifiers = modifiers,
					Position = position
				});

			if (mouse.RightButton == ButtonState.Released && previous.RightButton == ButtonState.Pressed)
				OnButtonReleased(new MouseButtonEventArgs
				{
					Button = MouseButton.Right,
					Modifiers = modifiers,
					Position = position
				});

			if (mouse.XButton1 == ButtonState.Released && previous.XButton1 == ButtonState.Pressed)
				OnButtonReleased(new MouseButtonEventArgs
				{
					Button = MouseButton.XButton1,
					Modifiers = modifiers,
					Position = position
				});

			if (mouse.XButton2 == ButtonState.Released && previous.XButton2 == ButtonState.Pressed)
				OnButtonReleased(new MouseButtonEventArgs
				{
					Button = MouseButton.XButton2,
					Modifiers = modifiers,
					Position = position
				});

			bool buttonDown = mouse.LeftButton == ButtonState.Pressed || mouse.MiddleButton == ButtonState.Pressed || mouse.RightButton == ButtonState.Pressed || mouse.XButton1 == ButtonState.Pressed || mouse.XButton2 == ButtonState.Pressed;

			if (previous.X != mouse.X || previous.Y != mouse.Y)
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
						Delta = new Vector2Int(mouse.X - previous.X, mouse.Y - previous.Y)
					});
			}

			if (previous.ScrollWheelValue != mouse.ScrollWheelValue)
			{
				int value = mouse.ScrollWheelValue;
				int delta = mouse.ScrollWheelValue - previous.ScrollWheelValue;
				OnMouseScroll(new MouseScrollEventArgs
				{
					Position = position,
					Offset = new Vector2Int(0, delta)
				});
			}

			previous = mouse;
		}

		private static int DistanceBetween(Vector2Int a, Vector2Int b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

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