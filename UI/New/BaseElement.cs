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
	public enum Overflow
	{
		Visible,
		Hidden
	}

	public enum Display
	{
		Visible,
		None
	}

	public static class Extensions
	{
		public static Vector2Int TopLeft(this Rectangle rectangle) => new Vector2Int(rectangle.X, rectangle.Y);
		public static Vector2Int BottomRight(this Rectangle rectangle) => new Vector2Int(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);

		public static Vector2Int Position(this Rectangle rectangle) => new Vector2Int(rectangle.X, rectangle.Y);
		public static Vector2Int Size(this Rectangle rectangle) => new Vector2Int(rectangle.Width, rectangle.Height);
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

		public (int Left, int Top, int Right, int Bottom) Padding = (0, 0, 0, 0);
		public (int Left, int Top, int Right, int Bottom) Margin = (0, 0, 0, 0);

		public virtual void Update(GameTime gameTime)
		{
		}

		public virtual void RecalculateChildren()
		{
			foreach (BaseElement element in Children) element.Recalculate();
		}

		public virtual void Recalculate()
		{
			Rectangle parent = Parent?.InnerDimensions ?? UserInterface.ActiveInstance.GetDimensions().ToRectangle();

			Rectangle dimensions = Rectangle.Empty;

			dimensions.Width = Width.Percent * parent.Width / 100 + Width.Pixels;
			dimensions.Height = Height.Percent * parent.Height / 100 + Height.Pixels;

			dimensions.X = parent.X + X.Percent * parent.Width / 100 + X.Pixels;
			dimensions.Y = parent.Y + Y.Percent * parent.Height / 100 + Y.Pixels;

			Dimensions = dimensions;
			InnerDimensions = new Rectangle(dimensions.X + Padding.Left, dimensions.Y + Padding.Top, dimensions.Width - Padding.Left - Padding.Right, dimensions.Height - Padding.Top - Padding.Bottom);
			OuterDimensions = new Rectangle(dimensions.X - Margin.Left, dimensions.Y - Margin.Top, dimensions.Width + Margin.Left + Margin.Right, dimensions.Height + Margin.Top + Margin.Bottom);

			RecalculateChildren();
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

		protected virtual void Draw(SpriteBatch spriteBatch)
		{
		}

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
				CullMode = CullMode.None
			};

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);

			Draw(spriteBatch);
			if (Overflow == Overflow.Visible) DrawChildren(spriteBatch);

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);

			if (Overflow == Overflow.Hidden)
			{
				Rectangle original = device.ScissorRectangle;

				spriteBatch.End();

				Rectangle clippingRectangle = GetClippingRectangle(spriteBatch);
				Rectangle adjustedClippingRectangle = Rectangle.Intersect(clippingRectangle, device.ScissorRectangle);
				device.ScissorRectangle = adjustedClippingRectangle;

				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, Utility.OverflowHiddenState, null, Main.UIScaleMatrix);

				DrawChildren(spriteBatch);

				spriteBatch.End();

				device.ScissorRectangle = original;

				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);
			}
		}

		internal bool ContainsPoint(Vector2Int point) => point.X >= Dimensions.X && point.X <= Dimensions.X + Dimensions.Width && point.Y >= Dimensions.Y && point.Y <= Dimensions.Y + Dimensions.Height;

		// todo: cache, recalculate on mouse move/window resize
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

		#region Events
		public event Action<MouseMoveEventArgs> OnMouseMove;
		public event Func<MouseScrollEventArgs, bool> OnMouseScroll;
		public event Func<MouseButtonEventArgs, bool> OnClick;
		public event Func<MouseButtonEventArgs, bool> OnDoubleClick;
		public event Func<MouseButtonEventArgs, bool> OnTripleClick;
		public event Func<MouseButtonEventArgs, bool> OnMouseDown;
		public event Func<MouseButtonEventArgs, bool> OnMouseUp;
		public event Func<MouseEventArgs, bool> OnMouseOut;
		public event Func<MouseEventArgs, bool> OnMouseOver;
		public event Action<MouseEventArgs> OnMouseEnter;
		public event Action<MouseEventArgs> OnMouseLeave;

		public event Func<KeyboardEventArgs, bool> OnKeyPressed;
		public event Func<KeyboardEventArgs, bool> OnKeyReleased;
		public event Func<KeyboardEventArgs, bool> OnKeyTyped;

		public virtual void MouseMove(MouseMoveEventArgs args)
		{
			OnMouseMove?.Invoke(args);
		}

		public virtual void MouseClick(MouseButtonEventArgs args)
		{
			OnClick?.Invoke(args);
		}

		public virtual void MouseUp(MouseButtonEventArgs args)
		{
			OnMouseUp?.Invoke(args);
		}
		
		private IEnumerable<BaseElement> ElementsAt(Vector2Int point)
		{
			foreach (BaseElement element in Children.Where(element => element.ContainsPoint(point)))
			{
				yield return element;
				foreach (BaseElement child in element.ElementsAt(point)) yield return child;
			}
		}

		internal void InternalMouseUp(MouseButtonEventArgs args)
		{
			foreach (BaseElement element in ElementsAt(args.Position).Reverse())
			{
				element.MouseUp(args);
				if (args.Handled) return;
			}

			MouseUp(args);
		}

		internal void InternalMouseDown(MouseButtonEventArgs args)
		{
			foreach (BaseElement element in ElementsAt(args.Position).Reverse())
			{
				element.MouseDown(args);
				if (args.Handled) return;
			}

			MouseDown(args);
		}

		public virtual void MouseDown(MouseButtonEventArgs args)
		{
			OnMouseDown?.Invoke(args);
		}

		public virtual void MouseScroll(MouseScrollEventArgs args)
		{
			OnMouseScroll?.Invoke(args);
		}

		public void DoubleClick(MouseButtonEventArgs args)
		{
			OnDoubleClick?.Invoke(args);
		}

		public void TripleClick(MouseButtonEventArgs args)
		{
			OnTripleClick?.Invoke(args);
		}

		public virtual void KeyTyped(KeyboardEventArgs args)
		{
			OnKeyTyped?.Invoke(args);
		}

		public virtual void KeyReleased(KeyboardEventArgs args)
		{
			OnKeyReleased?.Invoke(args);
		}

		public virtual void KeyPressed(KeyboardEventArgs args)
		{
			OnKeyPressed?.Invoke(args);
		}
		#endregion

		#region Interface implementations
		public virtual int CompareTo(BaseElement other) => 0;

		public int Count => Children.Count;

		public void Add(BaseElement item)
		{
			if (item == null) throw new ArgumentNullException(nameof(item));

			Children.Add(item);
			item.Parent = this;
			item.Recalculate();
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
		#endregion
	}
}