using System;
using System.Collections;
using System.Collections.Generic;
using BaseLibrary.Input;
using Microsoft.Xna.Framework;
using Terraria.UI;

namespace BaseLibrary.UI;

public partial class BaseElement : IComparable<BaseElement>, IEnumerable<BaseElement>
{
	internal BaseElement? Parent;
	public List<BaseElement> Children { get; } = [];

	public Dimension Size;
	public Dimension Position;

	public Margin Margin = Margin.Zero;
	public Padding Padding = Padding.Zero;

	public int? MinWidth = null, MaxWidth = null;
	public int? MinHeight = null, MaxHeight = null;

	public Rectangle Dimensions { get; set; }
	public Rectangle InnerDimensions { get; set; }
	public Rectangle OuterDimensions { get; set; }

	public Display Display = Display.Visible;
	public Overflow Overflow = Overflow.Visible;
	
	public event Action<MouseMoveEventArgs>? OnMouseMove;
	public event Action<MouseScrollEventArgs>? OnMouseScroll;
	public event Action<MouseButtonEventArgs>? OnClick;
	public event Action<MouseButtonEventArgs>? OnDoubleClick;
	public event Action<MouseButtonEventArgs>? OnTripleClick;
	public event Action<MouseButtonEventArgs>? OnMouseDown;
	public event Action<MouseButtonEventArgs>? OnMouseUp;
	public event Action<MouseButtonEventArgs>? OnMouseHeld;
	public event Action<MouseEventArgs>? OnMouseEnter;
	public event Action<MouseEventArgs>? OnMouseLeave;

	public event Action<KeyboardEventArgs>? OnKeyPressed;
	public event Action<KeyboardEventArgs>? OnKeyReleased;
	public event Action<KeyboardEventArgs>? OnKeyTyped;
	
	public bool IsMouseHovering { get; private set; }

	public object? HoverText;
    
	// NOTE: defer recalculation based on dirty flag?
	public virtual void Recalculate()
	{
		Rectangle parent = Parent?.InnerDimensions ?? UserInterface.ActiveInstance.GetDimensions().ToRectangle();

		Rectangle dimensions = Rectangle.Empty;
		dimensions.Width = (int)(Size.PercentX * parent.Width * 0.01f + Size.PixelsX);
		dimensions.Height = (int)(Size.PercentY * parent.Height * 0.01f + Size.PixelsY);

		int minWidth = MinWidth ?? 0;
		int minHeight = MinHeight ?? 0;
		int maxWidth = MaxWidth ?? int.MaxValue;
		int maxHeight = MaxHeight ?? int.MaxValue;
		
		MathUtility.Clamp(ref dimensions.Width, minWidth, maxWidth);
		MathUtility.Clamp(ref dimensions.Height, minHeight, maxHeight);

		// BUG: shouldn't position be based on outer dimensions?
		dimensions.X = (int)(parent.X + (Position.PercentX * parent.Width * 0.01f - dimensions.Width * Position.PercentX * 0.01f) + Position.PixelsX) + Margin.Left;
		dimensions.Y = (int)(parent.Y + (Position.PercentY * parent.Height * 0.01f - dimensions.Height * Position.PercentY * 0.01f) + Position.PixelsY) + Margin.Top;

		Dimensions = dimensions;
		InnerDimensions = new Rectangle(dimensions.X + Padding.Left, dimensions.Y + Padding.Top, dimensions.Width - Padding.Left - Padding.Right, dimensions.Height - Padding.Top - Padding.Bottom);
		OuterDimensions = new Rectangle(dimensions.X - Margin.Left, dimensions.Y - Margin.Top, dimensions.Width + Margin.Left + Margin.Right, dimensions.Height + Margin.Top + Margin.Bottom);

		RecalculateChildren();
	}

	protected virtual void RecalculateChildren()
	{
		foreach (BaseElement element in Children)
		{
			element.Recalculate();
		}
	}

	public virtual void Add(BaseElement element)
	{
		if (Children.Contains(element)) throw new Exception($"Element {element} is already added");

		element.Parent = this;
		Children.Add(element);
		element.Recalculate();
	}
	
	public virtual void Remove(BaseElement element)
	{
		if (!Children.Contains(element)) throw new Exception($"Element {element} is not contained");

		Children.Remove(element);
		element.Parent = null;
	}
	
	public virtual void Clear()
	{
		Children.Clear();
	}

	public BaseElement AddOnClick(Action<MouseButtonEventArgs> onClick)
	{
		OnClick += onClick;
		return this;
	}
	
	public IEnumerator<BaseElement> GetEnumerator() => Children.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}