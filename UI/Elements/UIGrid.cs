﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.UI;

namespace BaseLibrary.UI.Elements
{
	public interface IGridElement<T> where T : BaseElement
	{
		UIGrid<T> Grid { get; set; }
	}

	public class UIGrid<T> : BaseElement where T : BaseElement
	{
		private class UIInnerList : BaseElement
		{
			public override bool ContainsPoint(Vector2 point) => true;

			protected override void DrawChildren(SpriteBatch spriteBatch)
			{
				for (int i = 0; i < Elements.Count; i++)
				{
					UIElement current = Elements[i];
					if (Collision.CheckAABBvAABBCollision(Parent.Position, Parent.Size, current.GetDimensions().Position(), current.GetDimensions().Size())) current.Draw(spriteBatch);
				}
			}
		}

		public List<T> Items = new List<T>();
		public UIScrollbar scrollbar;
		internal BaseElement innerList = new BaseElement();
		private float innerListHeight;
		public float ListPadding = 4f;

		public int Count => Items.Count;

		public int columns;

		public UIGrid(int columns = 1)
		{
			this.columns = columns;

			innerList.OverflowHidden = false;
			innerList.Width = (0f, 1f);
			innerList.Height = (0f, 1f);
			innerList.MaxHeight = new StyleDimension(10000, 0);
			OverflowHidden = true;
			Append(innerList);

			scrollbar = new UIScrollbar();
			scrollbar.OnScroll += () =>
			{
				innerList.Top = (-scrollbar.ViewPosition, 0f);
				innerList.Recalculate();
			};
		}

		public override void ScrollWheel(UIScrollWheelEvent evt) => scrollbar.ViewPosition -= evt.ScrollWheelValue;

		public override void Update(GameTime gameTime)
		{
			if (IsMouseHovering && innerListHeight > InnerDimensions.Height) Hooking.BlockScrolling = true;

			for (int i = 0; i < Items.Count; i++) Items[i].Update(gameTime);

			base.Update(gameTime);
		}

		public void Add(IEnumerable<T> items)
		{
			foreach (T item in items)
			{
				if (item is IGridElement<T> element) element.Grid = this;
				Items.Add(item);
				innerList.Append(item);
			}

			Items.Sort(SortMethod);
			RecalculateChildren();
		}

		public void Insert(int index, T item)
		{
			if (item is IGridElement<T> element) element.Grid = this;
			Items.Insert(index, item);
			innerList.Insert(index, item);

			Items.Sort(SortMethod);
			RecalculateChildren();
		}

		public void Add(T item)
		{
			if (item is IGridElement<T> element) element.Grid = this;
			Items.Add(item);
			innerList.Append(item);

			Items.Sort(SortMethod);
			RecalculateChildren();
		}

		public void Remove(T item)
		{
			if (item is IGridElement<T> element) element.Grid = null;
			Items.Remove(item);
			innerList.RemoveChild(item);

			Items.Sort(SortMethod);
			RecalculateChildren();
		}

		public void Clear()
		{
			innerList.RemoveAllChildren();
			Items.Clear();

			RecalculateChildren();
		}

		public override void Recalculate()
		{
			base.Recalculate();

			innerList.Recalculate();
		}

		public override void RecalculateChildren()
		{
			float top = 0f;
			float left = 0f;

			for (int i = 0; i < Items.Count; i++)
			{
				UIElement item = Items[i];

				item.Top.Set(top, 0f);
				item.Left.Set(left, 0f);
				item.Recalculate();
				CalculatedStyle dimensions = item.GetOuterDimensions();

				if (i % columns == columns - 1 || i == Items.Count - 1)
				{
					top += dimensions.Height + ListPadding;
					left = 0;
				}
				else left += dimensions.Width + ListPadding;
			}

			innerListHeight = top;
			innerList.Height = (innerListHeight, 0);
			innerList.Recalculate();

			scrollbar?.SetView(InnerDimensions.Height, innerListHeight);
		}

		public int SortMethod(UIElement item1, UIElement item2) => item1.CompareTo(item2);

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.End();
			Rectangle prevRect = spriteBatch.GraphicsDevice.ScissorRectangle;
			spriteBatch.GraphicsDevice.ScissorRectangle = Rectangle.Intersect(GetClippingRectangle(spriteBatch), spriteBatch.GraphicsDevice.ScissorRectangle);
			spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, Utility.DefaultSamplerState, null, Utility.OverflowHiddenState, null, Main.UIScaleMatrix);

			DrawSelf(spriteBatch);
			innerList.InvokeMethod<object>("DrawChildren", spriteBatch);

			spriteBatch.End();
			spriteBatch.GraphicsDevice.ScissorRectangle = prevRect;
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Utility.DefaultSamplerState, null, null, null, Main.UIScaleMatrix);
		}
	}
}