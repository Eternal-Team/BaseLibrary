using BaseLibrary.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.UI;

namespace BaseLibrary.UI.New
{
	public readonly struct Padding
	{
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

	public static class Extensions
	{
		public static Vector2Int TopLeft(this Rectangle rectangle) => new Vector2Int(rectangle.X, rectangle.Y);
		public static Vector2Int TopRight(this Rectangle rectangle) => new Vector2Int(rectangle.X + rectangle.Width, rectangle.Y);
		public static Vector2Int BottomLeft(this Rectangle rectangle) => new Vector2Int(rectangle.X, rectangle.Y + rectangle.Height);
		public static Vector2Int BottomRight(this Rectangle rectangle) => new Vector2Int(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);

		public static Vector2Int Position(this Rectangle rectangle) => new Vector2Int(rectangle.X, rectangle.Y);
		public static Vector2Int Size(this Rectangle rectangle) => new Vector2Int(rectangle.Width, rectangle.Height);

		public static bool Contains(this Rectangle rectangle, Vector2Int point) => point.X >= rectangle.X && point.X <= rectangle.X + rectangle.Width && point.Y >= rectangle.Y && point.Y <= rectangle.Y + rectangle.Height;
	}

	public class StyleDimension
	{
		public int Percent = 0;
		public int Pixels = 0;
	}

	public class BaseElement : IComparable<BaseElement>
	{
		public BaseElement Parent { get; private set; }

		public bool IsMouseHovering { get; private set; }

		public Vector2Int Position => Dimensions.Position();
		public Vector2Int Size => Dimensions.Size();

		public Rectangle Dimensions { get; private set; }
		public Rectangle InnerDimensions { get; private set; }
		public Rectangle OuterDimensions { get; private set; }
		public List<BaseElement> Children = new List<BaseElement>();

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

			IsMouseHovering = ContainsPoint(new Vector2Int(Main.mouseX, Main.mouseY));
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

			//spriteBatch.End();
			//spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);

			//Draw(spriteBatch);
			//if (Overflow == Overflow.Visible) DrawChildren(spriteBatch);

			//spriteBatch.End();
			//spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);

			//if (Overflow == Overflow.Hidden)
			//{
			Rectangle original = device.ScissorRectangle;

			spriteBatch.End();

			if (Overflow == Overflow.Hidden)
			{
				Rectangle clippingRectangle = GetClippingRectangle(spriteBatch);
				Rectangle adjustedClippingRectangle = Rectangle.Intersect(clippingRectangle, device.ScissorRectangle);
				device.ScissorRectangle = adjustedClippingRectangle;
			}

			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);

			Draw(spriteBatch);
			DrawChildren(spriteBatch);

			spriteBatch.End();

			device.ScissorRectangle = original;

			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);
			//}
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

			MouseClick(args);
		}

		internal void InternalTripleClick(MouseButtonEventArgs args)
		{
			foreach (BaseElement element in ElementsAt(args.Position))
			{
				element.TripleClick(args);
				if (args.Handled) return;
			}

			MouseClick(args);
		}

		internal void InternalMouseMove(MouseMoveEventArgs args)
		{
			foreach (BaseElement element in ElementsAt(args.Position))
			{
				element.MouseMove(args);
				//if (args.Handled) return;
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
				element.KeyReleased(args);
				if (args.Handled) return;
			}

			KeyReleased(args);
		}

		internal void InternalKeyTyped(KeyboardEventArgs args)
		{
			foreach (BaseElement element in Children)
			{
				element.KeyTyped(args);
				if (args.Handled) return;
			}

			KeyTyped(args);
		}
		#endregion

		public virtual void Recalculate()
		{
			Rectangle parent = Parent?.InnerDimensions ?? UserInterface.ActiveInstance.GetDimensions().ToRectangle();

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

		internal bool ContainsPoint(Vector2Int point) => point.X >= Dimensions.X && point.X <= Dimensions.X + Dimensions.Width && point.Y >= Dimensions.Y && point.Y <= Dimensions.Y + Dimensions.Height;

		private Rectangle GetClippingRectangle(SpriteBatch spriteBatch)
		{
			Vector2Int topLeft = InnerDimensions.TopLeft();
			Vector2Int bottomRight = InnerDimensions.BottomRight();

			topLeft = Vector2Int.Transform(topLeft, Main.UIScaleMatrix);
			bottomRight = Vector2Int.Transform(bottomRight, Main.UIScaleMatrix);

			int width = spriteBatch.GraphicsDevice.Viewport.Width;
			int height = spriteBatch.GraphicsDevice.Viewport.Height;

			Rectangle result = new Rectangle
			{
				X = Utils.Clamp(topLeft.X, 0, width),
				Y = Utils.Clamp(topLeft.Y, 0, height),
				Width = Utils.Clamp(bottomRight.X - topLeft.X, 0, width - topLeft.X),
				Height = Utils.Clamp(bottomRight.Y - topLeft.Y, 0, height - topLeft.Y)
			};

			return result;
		}

		internal IEnumerable<BaseElement> ElementsAt(Vector2Int point)
		{
			List<BaseElement> elements = new List<BaseElement>();

			foreach (BaseElement element in Children.Where(element => element.ContainsPoint(point)))
			{
				elements.Add(element);
				elements.AddRange(element.ElementsAt(point));
			}

			elements.Reverse();
			return elements;
		}

		public virtual BaseElement GetElementAt(Vector2Int point)
		{
			BaseElement element = Children.FirstOrDefault(current => current.ContainsPoint(point) && current.Display != Display.None);

			if (element != null) return element.GetElementAt(point);

			return ContainsPoint(point) && Display != Display.None ? this : null;
		}

		public virtual int CompareTo(BaseElement other) => 0;

		public int Count => Children.Count;

		public void Add(BaseElement item)
		{
			if (item == null) throw new ArgumentNullException(nameof(item));

			Children.Add(item);
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