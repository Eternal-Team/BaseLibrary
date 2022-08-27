using System;
using System.Collections.Generic;
using System.Linq;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;

namespace BaseLibrary.UI;

public abstract class BaseState : BaseElement
{
	public BaseState()
	{
		Width = new StyleDimension(0, 100);
		Height = new StyleDimension(0, 100);
	}
}

public class BaseElement : IComparable<BaseElement>
{
	// public static int SlotSize => ModContent.GetInstance<BaseLibraryConfig>().SlotSize;
	// public static int SlotMargin => ModContent.GetInstance<BaseLibraryConfig>().SlotMargin;

	// public BaseElement With<T>(Action<T> action) where T : BaseElement
	// {
	// 	T element = ReflectionUtility.CreateInstance<T>();
	// 	action(element);
	// 	Add(element);
	//
	// 	return this;
	// }

	public BaseElement With(Action action)
	{
		action();
		return this;
	}

	public BaseElement Parent { get; protected internal set; }

	public List<BaseElement> Children = new();
	public int Count => Children.Count;

	public bool IsMouseHovering { get; private set; }
	public object? HoverText;

	public Vector2 Position
	{
		get => Dimensions.TopLeft();
		set
		{
			X = new StyleDimension((int)value.X, 0);
			Y = new StyleDimension((int)value.Y, 0);
			Recalculate();
		}
	}

	public Vector2 Size
	{
		get => Dimensions.Size();
		set
		{
			Width = new StyleDimension((int)value.X, 0);
			Height = new StyleDimension((int)value.Y, 0);
			Recalculate();
		}
	}

	public Rectangle Dimensions { get; private set; }
	public Rectangle InnerDimensions { get; private set; }
	public Rectangle OuterDimensions { get; private set; }

	public Display Display = Display.Visible;
	public Overflow Overflow = Overflow.Visible;

	public StyleDimension Width = new();
	public StyleDimension Height = new();
	public StyleDimension X = new();
	public StyleDimension Y = new();

	public Padding Padding;
	public Margin Margin;

	public int? MinWidth;
	public int? MinHeight;
	public int? MaxWidth;
	public int? MaxHeight;

	#region Events
	public event Action<MouseMoveEventArgs> OnMouseMove;
	public event Action<MouseScrollEventArgs> OnMouseScroll;
	public event Action<MouseButtonEventArgs> OnClick;
	public event Action<MouseButtonEventArgs> OnDoubleClick;
	public event Action<MouseButtonEventArgs> OnTripleClick;
	public event Action<MouseButtonEventArgs> OnMouseDown;
	public event Action<MouseButtonEventArgs> OnMouseUp;
	public event Action<MouseEventArgs> OnMouseOut;
	public event Action<MouseEventArgs> OnMouseOver;
	public event Action<MouseEventArgs> OnMouseEnter;
	public event Action<MouseEventArgs> OnMouseLeave;

	public event Action<KeyboardEventArgs> OnKeyPressed;
	public event Action<KeyboardEventArgs> OnKeyReleased;
	public event Action<KeyboardEventArgs> OnKeyTyped;

	public event Action<SpriteBatch> OnPreDraw;
	#endregion

	#region Virtual methods
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
			for (int i = 0; i < Children.Count; i++)
			{
				BaseElement element = this[i];
				if (element.Display != Display.None) element.InternalDraw(spriteBatch);
			}
		}
		else if (Overflow == Overflow.Hidden)
		{
			for (int i = 0; i < Children.Count; i++)
			{
				BaseElement element = this[i];
				if (element.Display != Display.None && MathUtility.CheckAABBvAABBCollision(Parent.Dimensions, element.Dimensions)) element.InternalDraw(spriteBatch);
			}
		}
	}

	protected virtual void MouseHeld(MouseButtonEventArgs args)
	{
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
	#endregion

	#region Internal Methods
	internal void InternalUpdate(GameTime gameTime)
	{
		Update(gameTime);

		for (int i = 0; i < Children.Count; i++)
		{
			BaseElement current = this[i];
			if (current.Display != Display.None) current.InternalUpdate(gameTime);
		}

		IsMouseHovering = ContainsPoint(Main.MouseScreen);
	}

	internal void InternalDraw(SpriteBatch spriteBatch)
	{
		GraphicsDevice device = spriteBatch.GraphicsDevice;
		SamplerState sampler = SamplerState.LinearClamp;
		RasterizerState rasterizer = new RasterizerState { CullMode = CullMode.None, ScissorTestEnable = true };

		Rectangle original = device.ScissorRectangle;

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);

		OnPreDraw?.Invoke(spriteBatch);
		Draw(spriteBatch);

		spriteBatch.End();

		if (Overflow == Overflow.Hidden)
		{
			Rectangle clippingRectangle = GetClippingRectangle(spriteBatch);
			Rectangle adjustedClippingRectangle = Rectangle.Intersect(clippingRectangle, device.ScissorRectangle);
			device.ScissorRectangle = adjustedClippingRectangle;
		}

		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);

		DrawChildren(spriteBatch);
		if (this is not BaseState && IsMouseHovering)
		{
			Main.LocalPlayer.cursorItemIconEnabled = false;
			Main.ItemIconCacheUpdate(0);

			if (HoverText is not null)
			{
				if (HoverText is Func<string> func) Main.instance.MouseText(func());
				else Main.instance.MouseText(HoverText.ToString());
			}
		}

		spriteBatch.End();

		device.ScissorRectangle = original;

		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);

// #if DEBUG
// 			if (debugDraw && debug)
// 			{
// 				if (OuterDimensions != Dimensions) spriteBatch.Draw(TextureAssets.MagicPixel.Value, OuterDimensions, Color.Goldenrod * 0.5f);
// 				spriteBatch.Draw(TextureAssets.MagicPixel.Value, Dimensions, Color.LimeGreen * 0.5f);
// 				if (InnerDimensions != Dimensions) spriteBatch.Draw(TextureAssets.MagicPixel.Value, InnerDimensions, Color.LightBlue * 0.5f);
// 			}
// #endif
	}

	internal void InternalMouseHeld(MouseButtonEventArgs args)
	{
		foreach (BaseElement element in ElementsAt(args.Position))
		{
			element.MouseHeld(args);
			if (args.Handled) return;
		}

		MouseHeld(args);
	}

	internal BaseElement InternalMouseDown(MouseButtonEventArgs args)
	{
		foreach (BaseElement element in ElementsAt(args.Position))
		{
			element.MouseDown(args);
			if (args.Handled) return element;
		}

		MouseDown(args);
		return this;
	}

	internal void InternalMouseUp(MouseButtonEventArgs args)
	{
		foreach (BaseElement element in ElementsAt(args.Position))
		{
			element.MouseUp(args);
			if (args.Handled) return;
		}

		MouseUp(args);
	}

	internal void InternalMouseClick(MouseButtonEventArgs args)
	{
		foreach (BaseElement element in ElementsAt(args.Position))
		{
			element.MouseClick(args);
			if (args.Handled) return;
		}

		MouseClick(args);
	}

	internal void InternalDoubleClick(MouseButtonEventArgs args)
	{
		foreach (BaseElement element in ElementsAt(args.Position))
		{
			element.DoubleClick(args);
			if (args.Handled) return;
		}

		DoubleClick(args);
	}

	internal void InternalTripleClick(MouseButtonEventArgs args)
	{
		foreach (BaseElement element in ElementsAt(args.Position))
		{
			element.TripleClick(args);
			if (args.Handled) return;
		}

		TripleClick(args);
	}

	internal void InternalMouseMove(MouseMoveEventArgs args)
	{
		foreach (BaseElement element in ElementsAt(args.Position))
		{
			element.MouseMove(args);
		}

		MouseMove(args);
	}

	internal void InternalMouseScroll(MouseScrollEventArgs args)
	{
		foreach (BaseElement element in ElementsAt(args.Position))
		{
			element.MouseScroll(args);
			if (args.Handled) return;
		}

		MouseScroll(args);
	}

	private bool debugDraw;
	internal static bool debug;

	internal void InternalMouseEnter(MouseMoveEventArgs args)
	{
		MouseEnter(args);
	}

	internal void InternalMouseLeave(MouseMoveEventArgs args)
	{
		MouseLeave(args);
	}

	internal void InternalKeyPressed(KeyboardEventArgs args)
	{
		foreach (BaseElement element in Children)
		{
			element.InternalKeyPressed(args);
			if (args.Handled) return;
		}

		KeyPressed(args);
	}

	internal void InternalKeyReleased(KeyboardEventArgs args)
	{
		foreach (BaseElement element in Children)
		{
			element.InternalKeyReleased(args);
			if (args.Handled) return;
		}

		KeyReleased(args);
	}

	internal void InternalKeyTyped(KeyboardEventArgs args)
	{
		foreach (BaseElement element in Children)
		{
			element.InternalKeyTyped(args);
			if (args.Handled) return;
		}

		KeyTyped(args);
	}

	internal void InternalActivate()
	{
		foreach (BaseElement element in Children)
		{
			element.InternalActivate();
		}

		Activate();
	}

	internal void InternalDeactivate()
	{
		foreach (BaseElement element in Children)
		{
			element.InternalDeactivate();
		}

		Deactivate();
	}
	#endregion

	public virtual void Recalculate()
	{
		Vector2 originalScreenSize = PlayerInput.OriginalScreenSize;
		Rectangle parent = Parent?.InnerDimensions ?? new Rectangle(0, 0, (int)(originalScreenSize.X / Main.UIScale), (int)(originalScreenSize.Y / Main.UIScale));

		Rectangle dimensions = Rectangle.Empty;

		int minWidth = Math.Max(0, MinWidth ?? 0);
		int minHeight = Math.Max(0, MinHeight ?? 0);
		int maxWidth = (int)Math.Min(Main.screenWidth * (1f / Main.UIScale), MaxWidth ?? Main.screenWidth * (1f / Main.UIScale));
		int maxHeight = (int)Math.Min(Main.screenHeight * (1f / Main.UIScale), MaxHeight ?? Main.screenHeight * (1f / Main.UIScale));

		dimensions.Width = (int)(Width.Percent * parent.Width / 100f + Width.Pixels);
		if (dimensions.Width < minWidth) dimensions.Width = minWidth;
		else if (dimensions.Width > maxWidth) dimensions.Width = maxWidth;

		dimensions.Height = (int)(Height.Percent * parent.Height / 100f + Height.Pixels);
		if (dimensions.Height < minHeight) dimensions.Height = minHeight;
		else if (dimensions.Height > maxHeight) dimensions.Height = maxHeight;

		dimensions.X = (int)(parent.X + (X.Percent * parent.Width / 100f - dimensions.Width * X.Percent / 100f) + X.Pixels);
		dimensions.Y = (int)(parent.Y + (Y.Percent * parent.Height / 100f - dimensions.Height * Y.Percent / 100f) + Y.Pixels);

		Dimensions = dimensions;
		InnerDimensions = new Rectangle(dimensions.X + Padding.Left, dimensions.Y + Padding.Top, dimensions.Width - Padding.Left - Padding.Right, dimensions.Height - Padding.Top - Padding.Bottom);
		OuterDimensions = new Rectangle(dimensions.X - Margin.Left, dimensions.Y - Margin.Top, dimensions.Width + Margin.Left + Margin.Right, dimensions.Height + Margin.Top + Margin.Bottom);

		RecalculateChildren();
	}

	public virtual void RecalculateChildren()
	{
		foreach (BaseElement element in Children) element.Recalculate();
	}

	internal bool ContainsPoint(Vector2 point) => point.X >= Dimensions.X && point.X <= Dimensions.X + Dimensions.Width && point.Y >= Dimensions.Y && point.Y <= Dimensions.Y + Dimensions.Height;

	protected virtual Rectangle GetClippingRectangle(SpriteBatch spriteBatch)
	{
		Vector2 topLeft = InnerDimensions.TopLeft();
		Vector2 bottomRight = InnerDimensions.BottomRight();

		topLeft = Vector2.Transform(topLeft, Main.UIScaleMatrix);
		bottomRight = Vector2.Transform(bottomRight, Main.UIScaleMatrix);

		int width = spriteBatch.GraphicsDevice.Viewport.Width;
		int height = spriteBatch.GraphicsDevice.Viewport.Height;

		Rectangle result = new Rectangle
		{
			X = (int)Utils.Clamp(topLeft.X, 0, width),
			Y = (int)Utils.Clamp(topLeft.Y, 0, height),
			Width = (int)Utils.Clamp(bottomRight.X - topLeft.X, 0, width - topLeft.X),
			Height = (int)Utils.Clamp(bottomRight.Y - topLeft.Y, 0, height - topLeft.Y)
		};

		return result;
	}

	internal IEnumerable<BaseElement> ElementsAt(Vector2 point)
	{
		List<BaseElement> elements = new List<BaseElement>();

		foreach (BaseElement element in Children.Where(element => element.ContainsPoint(point) && element.Display != Display.None))
		{
			elements.Add(element);
			elements.AddRange(element.ElementsAt(point));
		}

		elements.Reverse();
		return elements;
	}

	public virtual BaseElement GetElementAt(Vector2 point)
	{
		BaseElement element = Children.FirstOrDefault(current => current.ContainsPoint(point) && current.Display != Display.None);

		if (element != null) return element.GetElementAt(point);

		return ContainsPoint(point) && Display != Display.None ? this : null;
	}

	public virtual int CompareTo(BaseElement other) => 0;

	public void Add(BaseElement item)
	{
		if (item == null) throw new ArgumentNullException(nameof(item));
		if (Children.Contains(item)) throw new Exception($"Element {item} is already added");

		Children.Add(item);
		item.Parent = this;
		item.Recalculate();
	}

	public void AddRange(IEnumerable<BaseElement> elements)
	{
		if (elements == null) throw new ArgumentNullException(nameof(elements));

		foreach (BaseElement item in elements)
		{
			Children.Add(item);
			item.Parent = this;
			item.Recalculate();
		}
	}

	public void Insert(int index, BaseElement item)
	{
		if (item == null) throw new ArgumentNullException(nameof(item));

		Children.Insert(index, item);
		item.Parent = this;
		item.Recalculate();
	}

	public void Remove(BaseElement item)
	{
		if (item == null) throw new ArgumentNullException(nameof(item));

		Children.Remove(item);
		item.Parent = null;
	}

	public void Clear()
	{
		Children.Clear();
	}

	public BaseElement this[int index] => Children[index];
}