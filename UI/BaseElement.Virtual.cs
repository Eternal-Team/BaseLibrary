using System.Linq;
using BaseLibrary.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BaseLibrary.UI;

public partial class BaseElement
{
	public virtual int CompareTo(BaseElement? other) => 0;

	protected virtual void Update(GameTime gameTime)
	{
	}

	protected virtual void Draw(SpriteBatch spriteBatch)
	{
	}

	protected virtual void DrawChildren(SpriteBatch spriteBatch)
	{
		if (Overflow == Overflow.Visible)
		{
			foreach (BaseElement element in _children.Where(element => element.Display != Display.None))
			{
				element.InternalDraw(spriteBatch);
			}
		}
		else if (Overflow == Overflow.Hidden)
		{
			foreach (BaseElement element in _children.Where(element => element.Display != Display.None && Dimensions.Intersects(element.Dimensions))) // bug: this seems broken
			{
				element.InternalDraw(spriteBatch);
			}
		}
	}

	protected virtual void MouseHeld(MouseButtonEventArgs args)
	{
		OnMouseHeld?.Invoke(args);
	}

	protected virtual void MouseDown(MouseButtonEventArgs args)
	{
		OnMouseDown?.Invoke(args);
	}

	protected virtual void MouseUp(MouseButtonEventArgs args)
	{
		OnMouseUp?.Invoke(args);
	}

	protected virtual void MouseClick(MouseButtonEventArgs args)
	{
		OnClick?.Invoke(args);
	}

	protected virtual void DoubleClick(MouseButtonEventArgs args)
	{
		OnDoubleClick?.Invoke(args);
	}

	protected virtual void TripleClick(MouseButtonEventArgs args)
	{
		OnTripleClick?.Invoke(args);
	}

	protected virtual void MouseMove(MouseMoveEventArgs args)
	{
		OnMouseMove?.Invoke(args);
	}

	protected virtual void MouseScroll(MouseScrollEventArgs args)
	{
		OnMouseScroll?.Invoke(args);
	}

	protected virtual void MouseEnter(MouseEventArgs args)
	{
		OnMouseEnter?.Invoke(args);
	}

	protected virtual void MouseLeave(MouseEventArgs args)
	{
		OnMouseLeave?.Invoke(args);
	}

	protected virtual void KeyTyped(KeyboardEventArgs args)
	{
		OnKeyTyped?.Invoke(args);
	}

	protected virtual void KeyReleased(KeyboardEventArgs args)
	{
		OnKeyReleased?.Invoke(args);
	}

	protected virtual void KeyPressed(KeyboardEventArgs args)
	{
		OnKeyPressed?.Invoke(args);
	}

	protected virtual void Activate()
	{
	}

	protected virtual void Deactivate()
	{
	}
}