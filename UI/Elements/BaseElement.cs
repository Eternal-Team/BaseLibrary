using BaseLibrary.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace BaseLibrary.UI
{
	public readonly struct Padding
	{
		public static readonly Padding Zero = new Padding(0);

		public readonly int Left, Top, Right, Bottom;

		public Padding(int left, int top, int right, int bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		public Padding(int padding)
		{
			Left = Top = Right = Bottom = padding;
		}
	}

	public readonly struct Margin
	{
		public static readonly Margin Zero = new Margin(0);

		public readonly int Left, Top, Right, Bottom;

		public Margin(int left, int top, int right, int bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		public Margin(int margin)
		{
			Left = Top = Right = Bottom = margin;
		}
	}

	public class StyleDimension
	{
		public int Pixels;
		public int Percent;

		public StyleDimension()
		{
		}

		public StyleDimension(int pixels, int percent)
		{
			Pixels = pixels;
			Percent = percent;
		}
	}

	public abstract class BaseState : BaseElement
	{
		public abstract bool Enabled { get; }
	}

	public class BaseElement : IComparable<BaseElement>
	{
		public BaseElement Parent { get; protected internal set; }

		public List<BaseElement> Children = new List<BaseElement>();
		public int Count => Children.Count;

		public bool IsMouseHovering { get; private set; }
		public object HoverText = null;

		public Vector2 Position => Dimensions.Position();

		public Vector2 Size
		{
			get => Dimensions.Size();
			set
			{
				Width = new StyleDimension((int)value.X, 0);
				Height = new StyleDimension((int)value.Y, 0);
			}
		}

		public Rectangle Dimensions { get; private set; }
		public Rectangle InnerDimensions { get; private set; }
		public Rectangle OuterDimensions { get; private set; }

		public Display Display = Display.Visible;
		public Overflow Overflow = Overflow.Visible;

		public StyleDimension Width = new StyleDimension();
		public StyleDimension Height = new StyleDimension();
		public StyleDimension X = new StyleDimension();
		public StyleDimension Y = new StyleDimension();

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
					if (element.Display != Display.None && Utility.CheckAABBvAABBCollision(Parent.Dimensions, element.Dimensions)) element.InternalDraw(spriteBatch);
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
			RasterizerState rasterizer = new RasterizerState
			{
				CullMode = CullMode.None,
				ScissorTestEnable = true
			};

			Rectangle original = device.ScissorRectangle;

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);


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
			if (IsMouseHovering && HoverText != null) Utility.DrawMouseText(HoverText, Color.White);

			spriteBatch.End();

			device.ScissorRectangle = original;

			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);

			if (debugDraw && false)
			{
				if (OuterDimensions != Dimensions) spriteBatch.Draw(Main.magicPixel, OuterDimensions, Color.Goldenrod * 0.5f);
				spriteBatch.Draw(Main.magicPixel, Dimensions, Color.LimeGreen * 0.5f);
				if (InnerDimensions != Dimensions) spriteBatch.Draw(Main.magicPixel, InnerDimensions, Color.LightBlue * 0.5f);
			}
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

		internal void InternalMouseEnter(MouseMoveEventArgs args)
		{
			debugDraw = true;
			args.Handled = true;

			MouseEnter(args);
		}

		internal void InternalMouseLeave(MouseMoveEventArgs args)
		{
			debugDraw = false;
			args.Handled = true;

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
			Rectangle parent = Parent?.InnerDimensions ?? new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

			Rectangle dimensions = Rectangle.Empty;

			int minWidth = Math.Max(0, MinWidth ?? 0);
			int minHeight = Math.Max(0, MinHeight ?? 0);
			int maxWidth = Math.Min(Main.screenWidth, MaxWidth ?? Main.screenWidth);
			int maxHeight = Math.Min(Main.screenHeight, MaxHeight ?? Main.screenHeight);

			dimensions.Width = Width.Percent * parent.Width / 100 + Width.Pixels;
			if (dimensions.Width < minWidth) dimensions.Width = minWidth;
			else if (dimensions.Width > maxWidth) dimensions.Width = maxWidth;

			dimensions.Height = Height.Percent * parent.Height / 100 + Height.Pixels;
			if (dimensions.Height < minHeight) dimensions.Height = minHeight;
			else if (dimensions.Height > maxHeight) dimensions.Height = maxHeight;

			dimensions.X = parent.X + (X.Percent * parent.Width / 100 - dimensions.Width * X.Percent / 100) + X.Pixels;
			dimensions.Y = parent.Y + (Y.Percent * parent.Height / 100 - dimensions.Height * Y.Percent / 100) + Y.Pixels;

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
			Vector2 topLeft = InnerDimensions.Position();
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

		public BaseElement this[int index]
		{
			get => Children[index];
			set => Children[index] = value;
		}
	}
}