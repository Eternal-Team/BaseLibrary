// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Microsoft.Xna.Framework;
// using Terraria.ModLoader.Input;
// using Terraria.ModLoader.Input.Mouse;
//
// namespace BaseLibrary.UI
// {
// 	public interface IGridElement
// 	{
// 		// UIGrid<T> Grid { get; set; }
//
// 		bool Selected { get; set; }
// 	}
//
// 	public struct UIGridSettings
// 	{
// 		public static readonly UIGridSettings Default = new UIGridSettings
// 		{
// 			ItemMargin = 4,
// 			MaxSelectedItems = 0
// 		};
//
// 		public int ItemMargin;
// 		public int MaxSelectedItems;
// 	}
//
// 	public class UIGrid<T> : BaseElement where T : BaseElement
// 	{
// 		public UIGridSettings Settings = UIGridSettings.Default;
//
// 		public UIScrollbar Scrollbar { get; }
//
// 		public Func<T, bool> SearchSelector;
// 		public Comparison<T> SortFunction = (item1, item2) => item1.CompareTo(item2);
//
// 		private float innerListHeight;
// 		private int yOffset;
// 		private int columns;
//
// 		public UIGrid(int columns = 1)
// 		{
// 			this.columns = columns;
//
// 			Overflow = Overflow.Hidden;
//
// 			Scrollbar = new UIScrollbar();
// 			Scrollbar.OnScroll += () =>
// 			{
// 				yOffset = (int)-Scrollbar.ViewPosition;
// 				RecalculateChildren();
// 			};
// 		}
//
// 		public void Add(IEnumerable<T> items)
// 		{
// 			foreach (T item in items)
// 			{
// 				// if (item is IGridElement<T> element) element.Grid = this;
// 				item.Parent = this;
// 				Children.Add(item);
// 				
// 				item.OnClick += args => SelectItem(args, item);
// 			}
//
// 			Children.Sort(SortMethod);
// 			RecalculateChildren();
// 		}
//
// 		public void Add(T item)
// 		{
// 			// if (item is IGridElement<T> element) element.Grid = this;
// 			item.Parent = this;
// 			Children.Add(item);
//
// 			item.OnClick += args => SelectItem(args, item);
//
// 			Children.Sort(SortMethod);
// 			RecalculateChildren();
// 		}
//
// 		public void Insert(int index, T item)
// 		{
// 			// if (item is IGridElement<T> element) element.Grid = this;
// 			item.Parent = this;
// 			Children.Insert(index, item);
//
// 			Children.Sort(SortMethod);
// 			RecalculateChildren();
// 		}
//
// 		public void Remove(T item)
// 		{
// 			// if (item is IGridElement<T> element) element.Grid = null;
// 			item.Parent = null;
// 			Children.Remove(item);
// 			
// 			item.OnClick -= args => SelectItem(args, item);
//
// 			Children.Sort(SortMethod);
// 			RecalculateChildren();
// 		}
//
// 		public override void RecalculateChildren()
// 		{
// 			List<BaseElement> visible = Children.Where(item => item.Display != Display.None).ToList();
//
// 			int left = 0;
// 			int top = yOffset;
//
// 			for (int i = 0; i < visible.Count; i++)
// 			{
// 				BaseElement item = visible[i];
//
// 				item.X.Pixels = left;
// 				item.Y.Pixels = top + item.Margin.Top;
// 				item.Recalculate();
// 				Rectangle dimensions = item.OuterDimensions;
//
// 				if (i % columns == columns - 1 || i == visible.Count - 1)
// 				{
// 					top += dimensions.Height + Settings.ItemMargin;
// 					left = 0;
// 				}
// 				else left += dimensions.Width + Settings.ItemMargin;
// 			}
//
// 			innerListHeight = top - yOffset;
//
// 			Scrollbar?.SetView(InnerDimensions.Height, innerListHeight);
// 		}
//
// 		protected override void MouseScroll(MouseScrollEventArgs args)
// 		{
// 			// bug: first apply scroll to children
//
// 			Scrollbar.ViewPosition -= args.OffsetY;
//
// 			args.Handled = true;
// 		}
//
// 		public void Search()
// 		{
// 			if (SearchSelector == null) return;
//
// 			yOffset = 0;
//
// 			foreach (T item in Children) item.Display = SearchSelector.Invoke(item) ? Display.Visible : Display.None;
//
// 			RecalculateChildren();
// 		}
//
// 		private int SortMethod(BaseElement item1, BaseElement item2) => SortFunction((T)item1, (T)item2);
// 		
// 		private Queue<T> selected = new Queue<T>();
//
// 		private void SelectItem(MouseButtonEventArgs args, T item)
// 		{
// 			if (args.Button != MouseButton.Left || Settings.MaxSelectedItems <= 0) return;
//
// 			args.Handled = true;
//
// 			if (selected.Count >= Settings.MaxSelectedItems)
// 			{
// 				var deq = selected.Dequeue();
// 				if (deq is IGridElement element) element.Selected = false;
// 			}
//
// 			selected.Enqueue(item);
// 			if (item is IGridElement e) e.Selected = true;
// 		}
// 	}
// }