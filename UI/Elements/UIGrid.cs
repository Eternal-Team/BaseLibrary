using Microsoft.Xna.Framework;
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
		public delegate bool ElementSearchMethod(UIElement element);

		private class UIInnerList : BaseElement
		{
			public override bool ContainsPoint(Vector2 point) => true;

			protected override void DrawChildren(SpriteBatch spriteBatch)
			{
				Vector2 position = Parent.Dimensions.Position();
				for (int i = 0; i < Elements.Count; i++)
				{
					UIElement current = Elements[i];
					Vector2 position2 = current.GetDimensions().Position();
					if (Collision.CheckAABBvAABBCollision(position, Parent.Dimensions.Size(), position2, current.GetDimensions().Size())) current.Draw(spriteBatch);
				}
			}
		}

		public List<T> items = new List<T>();
		protected UIScrollbar scrollbar;
		internal UIElement innerList = new UIInnerList();
		private float innerListHeight;
		public float ListPadding = 4f;

		public int Count => items.Count;

		public int columns;

		public UIGrid(int columns = 1)
		{
			this.columns = columns;
			innerList.OverflowHidden = false;
			innerList.Width.Set(0f, 1f);
			innerList.Height.Set(0f, 1f);
			OverflowHidden = true;
			Append(innerList);

			scrollbar = new UIScrollbar();
			scrollbar.SetView(100, 1000);
			SetScrollbar(scrollbar);
		}

		public float GetTotalHeight() => innerListHeight;

		public void Goto(ElementSearchMethod searchMethod, bool center = false, bool bottom = false)
		{
			for (int i = 0; i < items.Count; i++)
			{
				if (searchMethod(items[i]))
				{
					scrollbar.ViewPosition = items[i].Top.pixels;
					if (bottom) scrollbar.ViewPosition = items[i].Top.pixels + items[i].GetOuterDimensions().Height;
					if (center) scrollbar.ViewPosition = items[i].Top.pixels - InnerDimensions.Height / 2 + items[i].GetOuterDimensions().Height / 2;
					return;
				}
			}
		}

		public override void Update(GameTime gameTime)
		{
			for (int i = 0; i < items.Count; i++) items[i].Update(gameTime);

			base.Update(gameTime);
		}

		public void AddRange(IEnumerable<T> items)
		{
			foreach (T item in items) Add(item);
		}

		public void Add(T item)
		{
			if (item is IGridElement<T>) ((IGridElement<T>)item).Grid = this;
			items.Add(item);
			innerList.Append(item);
			UpdateOrder();
			innerList.Recalculate();
		}

		public bool Remove(T item)
		{
			if (item is IGridElement<T>) ((IGridElement<T>)item).Grid = null;
			innerList.RemoveChild(item);
			UpdateOrder();
			return items.Remove(item);
		}

		public void Clear()
		{
			innerList.RemoveAllChildren();
			items.Clear();
		}

		public override void Recalculate()
		{
			base.Recalculate();

			innerList.Recalculate();
			UpdateScrollbar();
		}

		public override void ScrollWheel(UIScrollWheelEvent evt)
		{
			base.ScrollWheel(evt);
			if (scrollbar != null) scrollbar.ViewPosition -= evt.ScrollWheelValue;
		}

		public override void RecalculateChildren()
		{
			float top = 0f;
			float left = 0f;

			for (int i = 0; i < items.Count; i++)
			{
				UIElement item = items[i];
				CalculatedStyle dimensions = item.GetDimensions();
				item.Top.Set(top, 0f);
				item.Left.Set(left, 0f);
				if (item.Width.Precent > 0f) item.Width.Set((Dimensions.Width - (columns - 1) * ListPadding) * item.Width.Precent, 0f);
				item.Recalculate();
				if (i % columns == columns - 1 || i == items.Count - 1)
				{
					top += dimensions.Height + ListPadding;
					left = 0;
				}
				else left += dimensions.Width + ListPadding;
			}

			innerListHeight = top - ListPadding;
		}

		private void UpdateScrollbar()
		{
			scrollbar?.SetView(InnerDimensions.Height, innerListHeight);
		}

		public void SetScrollbar(UIScrollbar scrollbar)
		{
			this.scrollbar = scrollbar;
			UpdateScrollbar();
		}

		public void RemoveScrollbar()
		{
			scrollbar = null;
		}

		public void UpdateOrder()
		{
			items.Sort(SortMethod);
			UpdateScrollbar();
		}

		public int SortMethod(UIElement item1, UIElement item2) => item1.CompareTo(item2);

		public override List<SnapPoint> GetSnapPoints()
		{
			List<SnapPoint> list = new List<SnapPoint>();
			if (GetSnapPoint(out SnapPoint item)) list.Add(item);
			foreach (T current in items) list.AddRange(current.GetSnapPoints());
			return list;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.End();
			Rectangle prevRect = spriteBatch.GraphicsDevice.ScissorRectangle;
			spriteBatch.GraphicsDevice.ScissorRectangle = Rectangle.Intersect(GetClippingRectangle(spriteBatch), spriteBatch.GraphicsDevice.ScissorRectangle);
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, Utility.OverflowHiddenState, null, Main.UIScaleMatrix);

			DrawSelf(spriteBatch);
			innerList.InvokeMethod<object>("DrawChildren", spriteBatch);

			spriteBatch.End();
			spriteBatch.GraphicsDevice.ScissorRectangle = prevRect;
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, null, null, Main.UIScaleMatrix);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (scrollbar != null) innerList.Top.Set(-scrollbar.GetValue(), 0f);
			Recalculate();
		}
	}
}