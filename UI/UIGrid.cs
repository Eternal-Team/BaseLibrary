using System;
using System.Collections.Generic;
using System.Linq;
using BaseLibrary.Input;
using Microsoft.Xna.Framework;

namespace BaseLibrary.UI;

public enum Direction
{
	Horizontal,
	Vertical
}

public struct UIGridSettings
{
	public static readonly UIGridSettings Default = new UIGridSettings {
		ItemMargin = 8,
		MaxSelectedItems = 0,
		Direction = Direction.Vertical,
		ItemAlignment = HorizontalAlignment.Left
	};

	public Direction Direction;
	public int ItemMargin;
	public int MaxSelectedItems;
	public HorizontalAlignment ItemAlignment;
}

public class UIGrid<T> : BaseElement where T : BaseElement
{
	public UIGridSettings Settings = UIGridSettings.Default;
	public UIScrollbar Scrollbar { get; }

	private readonly int wrapping;
	private float innerListSize;
	private int offset;

	public UIGrid(int wrapping = 1)
	{
		if (wrapping < 1)
			throw new Exception("UIGrid wrapping must be greater than zero.");

		this.wrapping = wrapping;
		Overflow = Overflow.Hidden;

		Scrollbar = new UIScrollbar();
		Scrollbar.OnScroll += () => {
			offset = (int)-Scrollbar.ViewPosition;
			RecalculateChildren();
		};
	}

	protected override void RecalculateChildren()
	{
		List<BaseElement[]> visible = Children.Where(item => item.Display != Display.None).Chunk(wrapping).ToList();

		if (Settings.Direction == Direction.Vertical)
		{
			int top = offset;

			foreach (BaseElement[] elements in visible)
			{
				int totalWidth = elements.Sum(x => x.OuterDimensions.Width) + (elements.Length - 1) * Settings.ItemMargin;
				int left = Settings.ItemAlignment switch {
					HorizontalAlignment.Left => 0,
					HorizontalAlignment.Center => InnerDimensions.Width / 2 - totalWidth / 2,
					_ => InnerDimensions.Width - totalWidth
				};
				int tallestElement = 0;

				foreach (BaseElement item in elements)
				{
					item.Position.PixelsX = left;
					item.Position.PixelsY = top + item.Margin.Top;
					item.Recalculate();
					Rectangle dimensions = item.OuterDimensions;
					left += dimensions.Width + Settings.ItemMargin;
					if (dimensions.Height > tallestElement) tallestElement = dimensions.Height;
				}

				top += tallestElement + Settings.ItemMargin;
			}

			innerListSize = top - offset - Settings.ItemMargin + 4;
			Scrollbar.SetView(InnerDimensions.Height, innerListSize);
		}
		else
		{
			int left = offset;

			foreach (BaseElement[] elements in visible)
			{
				int totalHeight = elements.Sum(x => x.OuterDimensions.Height) + (elements.Length - 1) * Settings.ItemMargin;
				int top = Settings.ItemAlignment switch {
					HorizontalAlignment.Left => 0,
					HorizontalAlignment.Center => InnerDimensions.Height / 2 - totalHeight / 2,
					_ => InnerDimensions.Height - totalHeight
				};
				int widestElement = 0;

				foreach (BaseElement item in elements)
				{
					item.Position.PixelsX = left + item.Margin.Left;
					item.Position.PixelsY = top;
					item.Recalculate();
					Rectangle dimensions = item.OuterDimensions;
					top += dimensions.Height + Settings.ItemMargin;
					if (dimensions.Width > widestElement) widestElement = dimensions.Width;
				}

				left += widestElement + Settings.ItemMargin;
			}

			innerListSize = left - offset - Settings.ItemMargin + 4;
			Scrollbar.SetView(InnerDimensions.Width, innerListSize);
		}
	}

	public override void Add(BaseElement item)
	{
		base.Add(item);

		RecalculateChildren();
	}

	public override void Clear()
	{
		base.Clear();

		innerListSize = 0f;
		offset = 0;
		Scrollbar.SetView(0f, 0f);
	}

	protected override void MouseScroll(MouseScrollEventArgs args)
	{
		foreach (BaseElement element in ElementsAt(args.Position))
		{
			element.InternalMouseScroll(args);
			if (args.Handled) return;
		}

		if (Settings.Direction == Direction.Vertical)
			Scrollbar.ViewPosition -= args.OffsetY;
		else
			Scrollbar.ViewPosition -= args.OffsetX;

		args.Handled = true;
	}
}