using BaseLibrary.Input.Mouse;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BaseLibrary.Input
{
	public class WindowResizedEventArgs
	{
		public Vector2 Size { get; internal set; }

		public float Width => Size.X;

		public float Height => Size.Y;
	}

	public class InputEventArgs
	{
		public bool Handled = false;
	}

	public class MouseEventArgs : InputEventArgs
	{
		public Vector2 Position { get; internal set; }

		public float X => Position.X;

		public float Y => Position.Y;
	}

	public class KeyboardEventArgs : InputEventArgs
	{
		public Modifiers Modifiers { get; internal set; }

		public Keys Key { get; internal set; }

		public char? Character { get; internal set; }
	}

	public class MouseScrollEventArgs : MouseEventArgs
	{
		public Vector2 Offset { get; internal set; }

		public float OffsetX => Offset.X;

		public float OffsetY => Offset.Y;
	}

	public class MouseButtonEventArgs : MouseEventArgs
	{
		public MouseButton Button { get; internal set; }

		public Modifiers Modifiers { get; internal set; }
	}

	public class MouseMoveEventArgs : MouseEventArgs
	{
		public Vector2 Delta { get; internal set; }

		public float DeltaX => Delta.X;

		public float DeltaY => Delta.Y;
	}
}