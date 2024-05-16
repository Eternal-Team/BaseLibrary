using System;
using System.Collections;
using System.Collections.Generic;
using BaseLibrary.Input;
using Microsoft.Xna.Framework;
using Terraria.UI;

namespace BaseLibrary.UI;

public partial class BaseElement : IComparable<BaseElement>, IEnumerable<BaseElement>
{
	private readonly List<BaseElement> _children = [];

	internal BaseElement? Parent;
	public BaseElement Children => this;

	public Dimension Size;
	public Dimension Position;

	public Margin Margin = Margin.Zero;
	public Padding Padding = Padding.Zero;

	public int? MinWidth = null, MaxWidth = null;
	public int? MinHeight = null, MaxHeight = null;

	public Rectangle Dimensions { get; set; }
	public Rectangle InnerDimensions { get; set; }
	internal Rectangle OuterDimensions { get; set; }

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
    
	public virtual void Recalculate()
	{
		// NOTE: defer recalculation based on dirty flag?

		Rectangle parent = Parent?.InnerDimensions ?? UserInterface.ActiveInstance.GetDimensions().ToRectangle();

		Rectangle dimensions = Rectangle.Empty;
		dimensions.Width = (int)(Size.PercentX * parent.Width * 0.01f + Size.PixelsX);
		dimensions.Height = (int)(Size.PercentY * parent.Height * 0.01f + Size.PixelsY);

		int minWidth = MinWidth ?? 0;
		int minHeight = MinHeight ?? 0;
		int maxWidth = MaxWidth ?? parent.Width;
		int maxHeight = MaxHeight ?? parent.Height;
		
		MathUtility.Clamp(ref dimensions.Width, minWidth, maxWidth);
		MathUtility.Clamp(ref dimensions.Height, minHeight, maxHeight);

		dimensions.X = (int)(parent.X + (Position.PercentX * parent.Width * 0.01f - dimensions.Width * Position.PercentX * 0.01f) + Position.PixelsX);
		dimensions.Y = (int)(parent.Y + (Position.PercentY * parent.Height * 0.01f - dimensions.Height * Position.PercentY * 0.01f) + Position.PixelsY);

		Dimensions = dimensions;
		InnerDimensions = new Rectangle(dimensions.X + Padding.Left, dimensions.Y + Padding.Top, dimensions.Width - Padding.Left - Padding.Right, dimensions.Height - Padding.Top - Padding.Bottom);
		OuterDimensions = new Rectangle(dimensions.X - Margin.Left, dimensions.Y - Margin.Top, dimensions.Width + Margin.Left + Margin.Right, dimensions.Height + Margin.Top + Margin.Bottom);

		RecalculateChildren();
	}

	protected virtual void RecalculateChildren()
	{
		foreach (BaseElement element in _children)
		{
			element.Recalculate();
		}
	}

	public void Add(BaseElement element)
	{
		if (_children.Contains(element)) throw new Exception($"Element {element} is already added");

		element.Parent = this;
		_children.Add(element);
		element.Recalculate();
	}
	
	public void Remove(BaseElement element)
	{
		if (!_children.Contains(element)) throw new Exception($"Element {element} is not contained");

		_children.Remove(element);
		element.Parent = null;
	}

	public BaseElement AddOnClick(Action<MouseButtonEventArgs> onClick)
	{
		OnClick += onClick;
		return this;
	}
	
	public IEnumerator<BaseElement> GetEnumerator() => _children.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}