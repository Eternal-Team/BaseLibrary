using BaseLibrary.Input.Mouse;
using BaseLibrary.UI.New;
using Microsoft.Xna.Framework.Input;

namespace BaseLibrary.Input
{
	public class WindowResizedEventArgs
	{
		public Vector2Int Size { get; internal set; }

		public int Width => Size.X;

		public int Height => Size.Y;
	}

	public class InputEventArgs
	{
		public bool Handled = false;
	}

	public class MouseEventArgs : InputEventArgs
	{
		public Vector2Int Position { get; internal set; }

		public int X => Position.X;

		public int Y => Position.Y;
	}

	public class KeyboardEventArgs : InputEventArgs
	{
		public Modifiers Modifiers { get; internal set; }

		public Keys Key { get; internal set; }

		public char? Character { get; internal set; }
	}

	public class MouseScrollEventArgs : MouseEventArgs
	{
		public Vector2Int Offset { get; internal set; }

		public int OffsetX => Offset.X;

		public int OffsetY => Offset.Y;
	}

	public class MouseButtonEventArgs : MouseEventArgs
	{
		public MouseButton Button { get; internal set; }

		public Modifiers Modifiers { get; internal set; }
	}

	public class MouseMoveEventArgs : MouseEventArgs
	{
		public Vector2Int Delta { get; internal set; }

		public int DeltaX => Delta.X;

		public int DeltaY => Delta.Y;
	}
}