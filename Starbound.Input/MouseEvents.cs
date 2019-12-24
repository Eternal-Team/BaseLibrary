using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameInput;

namespace BaseLibrary.Input
{
	/// <summary>
	///     An abstraction around mouse input that turns XNA's underlying polling model into an event-based
	///     model for mouse input.
	/// </summary>
	public static class MouseEvents /*: GameComponent*/
	{
		/// <summary>
		///     Stores the previous mouse state for comparison later.
		/// </summary>
		private static MouseState previous;

		/// <summary>
		///     Stores information about when the last click was for the purposes of handling double clicks.
		/// </summary>
		private static Dictionary<MouseButton, MouseButtonEventArgs> lastClicks;

		/// <summary>
		///     Stores information about when the last double click was for the purposes of handling triple
		///     clicks.
		/// </summary>
		private static Dictionary<MouseButton, MouseButtonEventArgs> lastDoubleClicks;

		/// <summary>
		///     The maximum amount of time allowed between clicks for it to count as a double-click. Measured in
		///     milliseconds. Defaults to 300 milliseconds.
		/// </summary>
		public static int DoubleClickTime { get; set; }

		/// <summary>
		///     The maximum amount that the cursor can move (in pixels) and still count as a double-click.
		///     Defaults to 2.
		/// </summary>
		public static int DoubleClickMaxMove { get; set; }

		/// <summary>
		///     Indicates whether a MouseMoved event is raised even when being dragged. If set to <code>false</code>,
		///     moves and drags will be treated as separate events, with moves occurring when no button is pressed,
		///     and drags occurring when a button is pressed. If set to <code>true</code>, moves will be relayed
		///     any time the mouse moves, regardless if whether a button is pressed or not, and drags will still
		///     only occur when a button is pressed. In this case, a MouseMoved and MouseDragged event will both
		///     be raised.
		/// </summary>
		public static bool MoveRaisedOnDrag { get; set; }

		/// <summary>
		///     Sets defaults for the various mouse input settings.
		/// </summary>
		internal static void Load()
		{
			DoubleClickTime = 300;
			DoubleClickMaxMove = 2;
			MoveRaisedOnDrag = true;

			lastClicks = new Dictionary<MouseButton, MouseButtonEventArgs>
			{
				{ MouseButton.Left, new MouseButtonEventArgs(new TimeSpan(-1, 0, 0), new MouseState(), new MouseState(), MouseButton.Left) },
				{ MouseButton.Right, new MouseButtonEventArgs(new TimeSpan(-1, 0, 0), new MouseState(), new MouseState(), MouseButton.Right) },
				{ MouseButton.Middle, new MouseButtonEventArgs(new TimeSpan(-1, 0, 0), new MouseState(), new MouseState(), MouseButton.Middle) },
				{ MouseButton.XButton1, new MouseButtonEventArgs(new TimeSpan(-1, 0, 0), new MouseState(), new MouseState(), MouseButton.XButton1) },
				{ MouseButton.XButton2, new MouseButtonEventArgs(new TimeSpan(-1, 0, 0), new MouseState(), new MouseState(), MouseButton.XButton2) }
			};

			lastDoubleClicks = new Dictionary<MouseButton, MouseButtonEventArgs>
			{
				{ MouseButton.Left, new MouseButtonEventArgs(new TimeSpan(-1, 0, 0), new MouseState(), new MouseState(), MouseButton.Left) },
				{ MouseButton.Right, new MouseButtonEventArgs(new TimeSpan(-1, 0, 0), new MouseState(), new MouseState(), MouseButton.Right) },
				{ MouseButton.Middle, new MouseButtonEventArgs(new TimeSpan(-1, 0, 0), new MouseState(), new MouseState(), MouseButton.Middle) },
				{ MouseButton.XButton1, new MouseButtonEventArgs(new TimeSpan(-1, 0, 0), new MouseState(), new MouseState(), MouseButton.XButton1) },
				{ MouseButton.XButton2, new MouseButtonEventArgs(new TimeSpan(-1, 0, 0), new MouseState(), new MouseState(), MouseButton.XButton2) }
			};
		}

		internal static bool ButtonPressCaptured;

		/// <summary>
		///     Allows this component to handle polling and raise any and all mouse events that have occurred
		///     since the last update.
		/// </summary>
		/// <param name="gameTime"></param>
		internal static void Update(GameTime gameTime)
		{
			ButtonPressCaptured = false;

			MouseState current = Mouse.GetState();

			if (current.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released) ButtonPressCaptured |= OnButtonPressed(new MouseButtonEventArgs(gameTime.TotalGameTime, previous, current, MouseButton.Left));

			if (current.MiddleButton == ButtonState.Pressed && previous.MiddleButton == ButtonState.Released) ButtonPressCaptured |= OnButtonPressed(new MouseButtonEventArgs(gameTime.TotalGameTime, previous, current, MouseButton.Middle));

			if (current.RightButton == ButtonState.Pressed && previous.RightButton == ButtonState.Released) ButtonPressCaptured |= OnButtonPressed(new MouseButtonEventArgs(gameTime.TotalGameTime, previous, current, MouseButton.Right));

			if (current.XButton1 == ButtonState.Pressed && previous.XButton1 == ButtonState.Released) ButtonPressCaptured |= OnButtonPressed(new MouseButtonEventArgs(gameTime.TotalGameTime, previous, current, MouseButton.XButton1));

			if (current.XButton2 == ButtonState.Pressed && previous.XButton2 == ButtonState.Released) ButtonPressCaptured |= OnButtonPressed(new MouseButtonEventArgs(gameTime.TotalGameTime, previous, current, MouseButton.XButton2));

			if (current.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released) OnButtonClicked(new MouseButtonEventArgs(gameTime.TotalGameTime, previous, current, MouseButton.Left));

			if (current.MiddleButton == ButtonState.Pressed && previous.MiddleButton == ButtonState.Released) OnButtonClicked(new MouseButtonEventArgs(gameTime.TotalGameTime, previous, current, MouseButton.Middle));

			if (current.RightButton == ButtonState.Pressed && previous.RightButton == ButtonState.Released) OnButtonClicked(new MouseButtonEventArgs(gameTime.TotalGameTime, previous, current, MouseButton.Right));

			if (current.XButton1 == ButtonState.Pressed && previous.XButton1 == ButtonState.Released) OnButtonClicked(new MouseButtonEventArgs(gameTime.TotalGameTime, previous, current, MouseButton.XButton1));

			if (current.XButton2 == ButtonState.Pressed && previous.XButton2 == ButtonState.Released) OnButtonClicked(new MouseButtonEventArgs(gameTime.TotalGameTime, previous, current, MouseButton.XButton2));

			if (current.LeftButton == ButtonState.Released && previous.LeftButton == ButtonState.Pressed) OnButtonReleased(new MouseButtonEventArgs(gameTime.TotalGameTime, previous, current, MouseButton.Left));

			if (current.MiddleButton == ButtonState.Released && previous.MiddleButton == ButtonState.Pressed) OnButtonReleased(new MouseButtonEventArgs(gameTime.TotalGameTime, previous, current, MouseButton.Middle));

			if (current.RightButton == ButtonState.Released && previous.RightButton == ButtonState.Pressed) OnButtonReleased(new MouseButtonEventArgs(gameTime.TotalGameTime, previous, current, MouseButton.Right));

			if (current.XButton1 == ButtonState.Released && previous.XButton1 == ButtonState.Pressed) OnButtonReleased(new MouseButtonEventArgs(gameTime.TotalGameTime, previous, current, MouseButton.XButton1));

			if (current.XButton2 == ButtonState.Released && previous.XButton2 == ButtonState.Pressed) OnButtonReleased(new MouseButtonEventArgs(gameTime.TotalGameTime, previous, current, MouseButton.XButton2));

			bool buttonDown = current.LeftButton == ButtonState.Pressed || current.MiddleButton == ButtonState.Pressed || current.RightButton == ButtonState.Pressed || current.XButton1 == ButtonState.Pressed || current.XButton2 == ButtonState.Pressed;

			if (previous.X != current.X || previous.Y != current.Y)
			{
				if (buttonDown) OnMouseDragged(new MouseEventArgs(gameTime.TotalGameTime, previous, current));

				if (MoveRaisedOnDrag || !buttonDown) OnMouseMoved(new MouseEventArgs(gameTime.TotalGameTime, previous, current));
			}

			if (previous.ScrollWheelValue != current.ScrollWheelValue)
			{
				int value = current.ScrollWheelValue;
				int delta = current.ScrollWheelValue - previous.ScrollWheelValue;
				OnMouseWheelMoved(new MouseWheelEventArgs(gameTime.TotalGameTime, previous, current, delta, value));
			}

			previous = current;
		}

		/// <summary>
		///     Raises the ButtonReleased event. This is done automatically by a correctly configured component,
		///     but this is exposed publicly to allow programmatic button release events to occur.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private static bool OnButtonReleased(MouseButtonEventArgs args)
		{
			PlayerInput.LockTileUseButton = false;

			return ButtonReleased != null && ButtonReleased.GetInvocationList().Any(del => (bool)del.DynamicInvoke(args));
		}

		/// <summary>
		///     Raises the ButtonClicked event. This is done automatically by a correctly configured component,
		///     but this is exposed publicly to allow programmatic button click events to occur.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private static bool OnButtonClicked(MouseButtonEventArgs args)
		{
			// If this click is within the right time and position of the last double click, raise
			// a triple-click event as well.
			TimeSpan lastDoubleClick = lastDoubleClicks[args.Button].Time;
			if ((args.Time - lastDoubleClick).TotalMilliseconds < DoubleClickTime && DistanceBetween(args.Current, lastDoubleClicks[args.Button].Current) < DoubleClickMaxMove)
			{
				ButtonTripleClicked?.Invoke(args);
				return true;
			}

			// If this click is within the right time and position of the last click, raise a
			// double-click event as well.
			TimeSpan lastClick = lastClicks[args.Button].Time;
			if ((args.Time - lastClick).TotalMilliseconds < DoubleClickTime && DistanceBetween(args.Current, lastClicks[args.Button].Current) < DoubleClickMaxMove)
			{
				ButtonDoubleClicked?.Invoke(args);
				lastDoubleClicks[args.Button] = args;
				return true;
			}

			lastClicks[args.Button] = args;
			ButtonClicked?.Invoke(args);
			return true;
		}

		/// <summary>
		///     Calculates the Manhattan distance between two mouse positions.
		/// </summary>
		private static int DistanceBetween(MouseState a, MouseState b)
		{
			return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
		}

		/// <summary>
		///     Raises the ButtonPressed event. This is done automatically by a correctly configured component,
		///     but this is exposed publicly to allow programmatic button press events to occur.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private static bool OnButtonPressed(MouseButtonEventArgs args)
		{
			if (ButtonPressed != null && ButtonPressed.GetInvocationList().Any(del => (bool)del.DynamicInvoke(args)))
			{
				if (args.Button == MouseButton.Right) PlayerInput.LockTileUseButton = true;
				return true;
			}

			return false;
		}

		/// <summary>
		///     Raises the MouseMoved event. This is done automatically by a correctly configured component,
		///     but this is exposed publicly to allow programmatic mouse move events to occur.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private static bool OnMouseMoved(MouseEventArgs args)
		{
			MouseMoved?.Invoke(args);
			return true;
		}

		/// <summary>
		///     Raises the MouseDragged event. This is done automatically by a correctly configured component,
		///     but this is exposed publicly to allow programmatic mouse drag events to occur.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private static bool OnMouseDragged(MouseEventArgs args)
		{
			MouseDragged?.Invoke(args);
			return true;
		}

		/// <summary>
		///     Raises the MouseWheelMoved event. This is done automatically by a correctly configured component,
		///     but this is exposed publicly to allow programmatic mouse wheel events to occur.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private static bool OnMouseWheelMoved(MouseWheelEventArgs args)
		{
			MouseWheelMoved?.Invoke(args);
			return true;
		}

		/// <summary>
		///     An event that is raised whenever any mouse button is released. The specific button, as well as
		///     other relevant information, can be looked up through the MouseButtonEventArgs parameter.
		/// </summary>
		public static event Func<MouseButtonEventArgs, bool> ButtonReleased;

		/// <summary>
		///     An event that is raised whenever any mouse button is pressed. The specific button, as well as
		///     other relevant information, can be looked up through the MouseButtonEventArgs parameter.
		/// </summary>
		public static event Func<MouseButtonEventArgs, bool> ButtonPressed;

		/// <summary>
		///     An event that is raised whenever any mouse button is clicked. The specific button, as well as
		///     other relevant information, can be looked up through the MouseButtonEventArgs parameter.
		///     In the default implementation, clicks occur at the same time as presses (when the button
		///     actually gets pressed in) but it represents a different conceptual model, and other
		///     implementations may not define clicks in the same way.
		/// </summary>
		public static event Func<MouseButtonEventArgs, bool> ButtonClicked;

		/// <summary>
		///     An event that is raised whenever two button clicks occur in the same spot in a short period
		///     of time. The tolerance allowed for defining "same spot" can be configured through
		///     MouseEvents.DoubleClickMaxMove, while the tolerance for the time period can be configured through
		///     MouseEvents.DoubleClickTime.
		/// </summary>
		public static event Func<MouseButtonEventArgs, bool> ButtonDoubleClicked;

		/// <summary>
		///     An event that is raised whenever three button clicks occur in the same spot in a short period
		///     of time. The tolerance allowed for defining "same spot" can be configured through
		///     MouseEvents.DoubleClickMaxMove, while the tolerance for the time period can be configured through
		///     MouseEvents.DoubleClickTime.
		/// </summary>
		public static event Func<MouseButtonEventArgs, bool> ButtonTripleClicked;

		/// <summary>
		///     An event that is raised whenever the mouse moves.
		/// </summary>
		public static event Func<MouseEventArgs, bool> MouseMoved;

		/// <summary>
		///     An event that is raised whenever the mouse is dragged (a mouse move with any button pressed).
		/// </summary>
		public static event Func<MouseEventArgs, bool> MouseDragged;

		/// <summary>
		///     An event that is raised whenever the mouse wheel is rotated.
		/// </summary>
		public static event Func<MouseEventArgs, bool> MouseWheelMoved;
	}
}