using BaseLibrary.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseLibrary.UI
{
	public interface IGridElement<T> where T : BaseElement
	{
		UIGrid<T> Grid { get; set; }
	}

	public class UIGrid<T> : BaseElement where T : BaseElement
	{
		public int columns;

		private float innerListHeight;

		public int ItemMargin = 4;
		public UIScrollbar scrollbar;

		public Func<T, bool> SearchSelector;

		private int yOffset;

		public UIGrid(int columns = 1)
		{
			this.columns = columns;

			Overflow = Overflow.Hidden;

			scrollbar = new UIScrollbar();
			scrollbar.OnScroll += () =>
			{
				yOffset = (int)-scrollbar.ViewPosition;
				RecalculateChildren();
			};
		}

		public void Add(IEnumerable<T> items)
		{
			foreach (T item in items)
			{
				if (item is IGridElement<T> element) element.Grid = this;
				item.Parent = this;
				Children.Add(item);
			}

			Children.Sort(SortMethod);
			RecalculateChildren();
		}

		public void Add(T item)
		{
			if (item is IGridElement<T> element) element.Grid = this;
			item.Parent = this;
			Children.Add(item);

			Children.Sort(SortMethod);
			RecalculateChildren();
		}

		public void Insert(int index, T item)
		{
			if (item is IGridElement<T> element) element.Grid = this;
			item.Parent = this;
			Children.Insert(index, item);

			Children.Sort(SortMethod);
			RecalculateChildren();
		}

		public void Remove(T item)
		{
			if (item is IGridElement<T> element) element.Grid = null;
			item.Parent = null;
			Children.Remove(item);

			Children.Sort(SortMethod);
			RecalculateChildren();
		}

		public override void RecalculateChildren()
		{
			List<BaseElement> visible = Children.Where(item => item.Display != Display.None).ToList();

			int left = 0;
			int top = yOffset;

			for (int i = 0; i < visible.Count; i++)
			{
				BaseElement item = visible[i];

				item.X.Pixels = left;
				item.Y.Pixels = top + item.Margin.Top;
				item.Recalculate();
				Rectangle dimensions = item.OuterDimensions;

				if (i % columns == columns - 1 || i == visible.Count - 1)
				{
					top += dimensions.Height + ItemMargin;
					left = 0;
				}
				else left += dimensions.Width + ItemMargin;
			}

			innerListHeight = top - yOffset;

			scrollbar?.SetView(InnerDimensions.Height, innerListHeight);
		}

		protected override void MouseScroll(MouseScrollEventArgs args)
		{
			scrollbar.ViewPosition -= args.OffsetY;

			args.Handled = true;
		}

		public void Search()
		{
			if (SearchSelector == null) return;

			yOffset = 0;

			foreach (T item in Children) item.Display = SearchSelector.Invoke(item) ? Display.Visible : Display.None;

			RecalculateChildren();
		}

		private int SortMethod(BaseElement item1, BaseElement item2) => item1.CompareTo(item2);
	}
}