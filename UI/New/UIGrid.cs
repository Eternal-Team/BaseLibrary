using BaseLibrary.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseLibrary.UI.New
{
	public interface IGridElement<T> where T : BaseElement
	{
		UIGrid<T> Grid { get; set; }
	}

	public class UIGrid<T> : BaseElement where T : BaseElement
	{
		public int columns;

		private float innerListHeight;

		public int ListPadding = 4;
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
				Children.Add(item);
			}

			Children.Sort(SortMethod);
			RecalculateChildren();
		}

		public void Add(T item)
		{
			if (item is IGridElement<T> element) element.Grid = this;
			Children.Add(item);

			Children.Sort(SortMethod);
			RecalculateChildren();
		}

		public void Insert(int index, T item)
		{
			if (item is IGridElement<T> element) element.Grid = this;
			Children.Insert(index, item);

			Children.Sort(SortMethod);
			RecalculateChildren();
		}

		public void Remove(T item)
		{
			if (item is IGridElement<T> element) element.Grid = null;
			Children.Remove(item);

			Children.Sort(SortMethod);
			RecalculateChildren();
		}

		public override void RecalculateChildren()
		{
			int left = InnerDimensions.X;
			int top = InnerDimensions.Y + yOffset;

			List<BaseElement> visible = Children.Where(item => item.Display != Display.None).ToList();

			for (int i = 0; i < visible.Count; i++)
			{
				BaseElement item = visible[i];

				item.X.Pixels = left;
				item.Y.Pixels = top;
				item.Recalculate();
				Rectangle dimensions = item.OuterDimensions;

				if (i % columns == columns - 1 || i == visible.Count - 1)
				{
					top += dimensions.Height + ListPadding;
					left = InnerDimensions.X;
				}
				else left += dimensions.Width + ListPadding;
			}

			innerListHeight = top - InnerDimensions.Y - yOffset;

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