namespace BaseLibrary.UI;

public struct Padding
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

public struct Margin
{
	public static readonly Margin Zero = new(0);

	public int Left, Top, Right, Bottom;

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

public struct Dimension(int pixelsX, int pixelsY, int percentX, int percentY)
{
	public static readonly Dimension Zero = new(0, 0, 0, 0);

	public int PixelsX = pixelsX, PixelsY = pixelsY;
	public int PercentX = percentX, PercentY = percentY;

	public static Dimension FromPercent(int percent) => new(0, 0, percent, percent);

	public static Dimension FromPercent(int percentX, int percentY) => new(0, 0, percentX, percentY);

	public static Dimension FromPixels(int pixels) => new(pixels, pixels, 0, 0);

	public static Dimension FromPixels(int pixelsX, int pixelsY) => new(pixelsX, pixelsY, 0, 0);
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