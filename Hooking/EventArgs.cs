using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BaseLibrary;

public class WindowResizedEventArgs
{
	public Vector2 Size { get; internal set; }

	public float Width => Size.X;

	public float Height => Size.Y;

	public WindowResizedEventArgs(Vector2 size)
	{
		Size = size;
	}
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

	public MouseEventArgs(Vector2 position)
	{
		Position = position;
	}
}

public class KeyboardEventArgs : InputEventArgs
{
	public Modifiers Modifiers { get; internal set; }

	public Keys Key { get; internal set; }

	public char? Character { get; internal set; }

	public KeyboardEventArgs(Modifiers modifiers, Keys key, char? character)
	{
		Modifiers = modifiers;
		Key = key;
		Character = character;
	}
}

public class MouseScrollEventArgs : MouseEventArgs
{
	public Vector2 Offset { get; internal set; }

	public float OffsetX => Offset.X;

	public float OffsetY => Offset.Y;

	public MouseScrollEventArgs(Vector2 position, Vector2 offset) : base(position)
	{
		Offset = offset;
	}
}

public class MouseButtonEventArgs : MouseEventArgs
{
	public MouseButton Button { get; internal set; }

	public Modifiers Modifiers { get; internal set; }

	public MouseButtonEventArgs(Vector2 position, MouseButton button, Modifiers modifiers) : base(position)
	{
		Button = button;
		Modifiers = modifiers;
	}
}

public class MouseMoveEventArgs : MouseEventArgs
{
	public Vector2 Delta { get; internal set; }

	public float DeltaX => Delta.X;

	public float DeltaY => Delta.Y;

	public MouseMoveEventArgs(Vector2 position, Vector2 delta) : base(position)
	{
		Delta = delta;
	}
}