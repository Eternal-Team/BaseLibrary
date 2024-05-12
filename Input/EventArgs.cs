using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BaseLibrary.Input;

public class WindowResizedEventArgs(Vector2 size)
{
	public Vector2 Size => size;
	public float Width => size.X;
	public float Height => size.Y;
}

public class InputEventArgs
{
	public bool Handled = false;
}

public class MouseEventArgs(Vector2 position) : InputEventArgs
{
	public Vector2 Position => position;
	public float X => position.X;
	public float Y => position.Y;
}

public class KeyboardEventArgs(Modifiers modifiers, Keys key, char? character) : InputEventArgs
{
	public Modifiers Modifiers => modifiers;
	public Keys Key => key;
	public char? Character => character;
}

public class MouseScrollEventArgs(Vector2 position, Vector2 offset) : MouseEventArgs(position)
{
	public Vector2 Offset => offset;
	public float OffsetX => offset.X;
	public float OffsetY => offset.Y;
}

public class MouseButtonEventArgs(Vector2 position, MouseButton button, Modifiers modifiers) : MouseEventArgs(position)
{
	public MouseButton Button => button;
	public Modifiers Modifiers => modifiers;
}

public class MouseMoveEventArgs(Vector2 position, Vector2 delta) : MouseEventArgs(position)
{
	public Vector2 Delta => delta;
	public float DeltaX => delta.X;
	public float DeltaY => delta.Y;
}