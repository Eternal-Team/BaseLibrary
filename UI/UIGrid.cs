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
	public static readonly UIGridSettings Default = new()
	{
		ItemMargin = 8,
		MaxSelectedItems = 0,
		Direction = Direction.Vertical
	};

	public Direction Direction;
	public int ItemMargin;
	public int MaxSelectedItems;
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
		this.wrapping = wrapping;
		Overflow = Overflow.Hidden;

		Scrollbar = new UIScrollbar();
		Scrollbar.OnScroll += () =>
		{
			offset = (int)-Scrollbar.ViewPosition;
			RecalculateChildren();
		};
	}

	protected override void RecalculateChildren()
	{
		var visible = Children.Where(item => item.Display != Display.None).ToList();

		if (Settings.Direction == Direction.Vertical)
		{
			int left = 0;
			int top = offset;

			for (int i = 0; i < visible.Count; i++)
			{
				BaseElement item = visible[i];

				item.Position.PixelsX = left;
				item.Position.PixelsY = top + item.Margin.Top;
				item.Recalculate();
				Rectangle dimensions = item.OuterDimensions;

				if (i % wrapping == wrapping - 1 || i == visible.Count - 1)
				{
					top += dimensions.Height + Settings.ItemMargin;
					left = 0;
				}
				else
				{
					left += dimensions.Width + Settings.ItemMargin;
				}
			}

			innerListSize = top - offset - Settings.ItemMargin;
			Scrollbar.SetView(InnerDimensions.Height, innerListSize);
		}
		else
		{
			int left = offset;
			int top = 0;

			for (int i = 0; i < visible.Count; i++)
			{
				BaseElement item = visible[i];

				item.Position.PixelsX = left + item.Margin.Left;
				item.Position.PixelsY = top;
				item.Recalculate();
				Rectangle dimensions = item.OuterDimensions;

				if (i % wrapping == wrapping - 1 || i == visible.Count - 1)
				{
					left += dimensions.Width + Settings.ItemMargin;
					top = 0;
				}
				else
				{
					top += dimensions.Height + Settings.ItemMargin;
				}
			}

			innerListSize = left - offset - Settings.ItemMargin;
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