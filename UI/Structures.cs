namespace BaseLibrary.UI;

public readonly struct Padding
{
	public static readonly Padding Zero = new(0);

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
	public static readonly Margin Zero = new(0);

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

public struct StyleDimension
{
	public int Pixels;
	public int Percent;

	public StyleDimension()
	{
		Pixels = 0;
		Percent = 0;
	}

	public StyleDimension(int pixels, int percent)
	{
		Pixels = pixels;
		Percent = percent;
	}
}

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

public enum HorizontalAlignment
{
	Left,
	Center,
	Right
}

public enum VerticalAlignment
{
	Top,
	Center,
	Bottom
}