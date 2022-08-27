using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BaseLibrary;

public abstract class Layer
{
	public virtual bool Enabled => true;

	public virtual void OnUpdate(GameTime gameTime)
	{
	}

	public virtual void OnDraw(SpriteBatch spriteBatch, GameTime gameTime)
	{
	}

	public virtual void OnMouseMove(MouseMoveEventArgs args)
	{
	}

	public virtual void OnMouseDown(MouseButtonEventArgs args)
	{
	}

	public virtual void OnMouseUp(MouseButtonEventArgs args)
	{
	}

	public virtual void OnMouseEnter(MouseEventArgs args)
	{
	}

	public virtual void OnMouseLeave(MouseEventArgs args)
	{
	}

	public virtual void OnClick(MouseButtonEventArgs args)
	{
	}

	public virtual void OnDoubleClick(MouseButtonEventArgs args)
	{
	}

	public virtual void OnTripleClick(MouseButtonEventArgs args)
	{
	}

	public virtual void OnMouseScroll(MouseScrollEventArgs args)
	{
	}

	public virtual void OnKeyPressed(KeyboardEventArgs args)
	{
	}

	public virtual void OnKeyReleased(KeyboardEventArgs args)
	{
	}

	public virtual void OnKeyTyped(KeyboardEventArgs args)
	{
	}

	public virtual void OnWindowResize(WindowResizedEventArgs args)
	{
	}
	
	public virtual void OnScaleChanged()
	{
	}
}