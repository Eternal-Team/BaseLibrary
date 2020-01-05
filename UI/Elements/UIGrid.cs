//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Terraria;
//using Terraria.UI;

//namespace BaseLibrary.UI.Elements
//{
//	public interface IGridElement<T> where T : BaseElement
//	{
//		UIGrid<T> Grid { get; set; }
//	}

//	public class UIGrid<T> : BaseElement where T : BaseElement
//	{
//		public int Count => Items.Count;

//		public int columns;
//		internal BaseElement innerList = new BaseElement();
//		private float innerListHeight;

//		public List<T> Items = new List<T>();
//		public float ListPadding = 4f;
//		public UIScrollbar scrollbar;

//		public Func<T, bool> SearchSelector;

//		public UIGrid(int columns = 1)
//		{
//			this.columns = columns;

//			innerList.OverflowHidden = false;
//			innerList.Width = { Pixels = 0f, Percent = 100f };
//			innerList.Height = { Pixels = 0f, Percent = 100f };
//			innerList.MaxHeight = new StyleDimension(10000, 0);
//			OverflowHidden = true;
//			Add(innerList);

//			scrollbar = new UIScrollbar();
//			scrollbar.OnScroll += () =>
//			{
//				innerList.Y = { Pixels = -scrollbar.ViewPositionf);
//				innerList.Recalculate();
//			};
//		}

//		public void Add(IEnumerable<T> items)
//		{
//			foreach (T item in items)
//			{
//				if (item is IGridElement<T> element) element.Grid = this;
//				Items.Add(item);
//				innerList.Add(item);
//			}

//			Items.Sort(SortMethod);
//			RecalculateChildren();
//		}

//		public void Add(T item)
//		{
//			if (item is IGridElement<T> element) element.Grid = this;
//			Items.Add(item);
//			innerList.Add(item);

//			Items.Sort(SortMethod);
//			RecalculateChildren();
//		}

//		public void Clear()
//		{
//			innerList.RemoveAllChildren();
//			Items.Clear();

//			RecalculateChildren();
//		}

//		public override void Draw(SpriteBatch spriteBatch)
//		{
//			spriteBatch.End();
//			Rectangle prevRect = spriteBatch.GraphicsDevice.ScissorRectangle;
//			spriteBatch.GraphicsDevice.ScissorRectangle = Rectangle.Intersect(GetClippingRectangle(spriteBatch) spriteBatch.GraphicsDevice.ScissorRectangle);
//			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Utility.DefaultSamplerState, null, Utility.OverflowHiddenState, null, Main.UIScaleMatrix);

//			DrawSelf(spriteBatch);
//			innerList.InvokeMethod<object>("DrawChildren", spriteBatch);

//			spriteBatch.End();
//			spriteBatch.GraphicsDevice.ScissorRectangle = prevRect;
//			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Utility.DefaultSamplerState, null, null, null, Main.UIScaleMatrix);
//		}

//		public void Insert(int index, T item)
//		{
//			if (item is IGridElement<T> element) element.Grid = this;
//			Items.Insert(index, item);
//			innerList.Insert(index, item);

//			Items.Sort(SortMethod);
//			RecalculateChildren();
//		}

//		public override void Recalculate()
//		{
//			base.Recalculate();

//			innerList.Recalculate();
//		}

//		public override void RecalculateChildren()
//		{
//			float top = 0f;
//			float left = 0f;

//			List<T> visible = Items.Where(item => item.Visible).ToList();

//			for (int i = 0; i < visible.Count; i++)
//			{
//				BaseElement item = visible[i];
//				if (!item.Visible) continue;

//				item.Y = { Pixels = topf };
//				item.X = { Pixels = leftf);
//				item.Recalculate();
//				CalculatedStyle dimensions = item.GetOuterDimensions();

//				if (i % columns == columns - 1 || i == visible.Count - 1 }
//				{
//					top += dimensions.Height + ListPadding;
//					left = 0;
//				}
//				else left += dimensions.Width + ListPadding;
//			}

//			innerListHeight = top;
//			innerList.Height.Pixels = innerListHeight;
//			innerList.Recalculate( };

//			scrollbar?.SetView(InnerDimensions.Height, innerListHeight);
//		}

//		public void Remove(T item)
//		{
//			if (item is IGridElement<T> element) element.Grid = null;
//			Items.Remove(item);
//			innerList.Remove(item);

//			Items.Sort(SortMethod);
//			RecalculateChildren();
//		}

//		public override void ScrollWheel(UIScrollWheelEvent evt) => scrollbar.ViewPosition -= evt.OffsetY;

//		public void Search()
//		{
//			if (SearchSelector == null) return;

//			foreach (T item in Items) item.Visible = SearchSelector.Invoke(item);
//		}

//		public int SortMethod(BaseElement item1, BaseElement item2) => item1.CompareTo(item2);

//		protected override void Update(GameTime gameTime)
//		{
//			if (IsMouseHovering && innerListHeight > InnerDimensions.Height) Hooking.BlockScrolling = true;

//			for (int i = 0; i < Items.Count; i++) Items[i].Update(gameTime);

//			base.Update(gameTime);
//		}

//		private class UIInnerList : BaseElement
//		{
//			public override bool ContainsPoint(Vector2 point) => true;

//			protected override void DrawChildren(SpriteBatch spriteBatch)
//			{
//				for (int i = 0; i < Elements.Count; i++)
//				{
//					BaseElement current = Elements[i];
//					if (Collision.CheckAABBvAABBCollision(Parent.Position, Parent.Size, current.GetDimensions().Position(), current.GetDimensions().Size())) current.Draw(spriteBatch);
//				}
//			}
//		}
//	}
//}

