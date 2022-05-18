using Microsoft.Xna.Framework.Input;

namespace BaseLibrary;

public static class KeyboardUtil
{
	// todo: IME works afaik, only problem is with my UITextInput
	public static char? ToChar(Keys key, Modifiers modifiers = Modifiers.None)
	{
		switch (key)
		{
			case Keys.A:
				return ShiftDown(modifiers) ? 'A' : 'a';
			case Keys.B:
				return ShiftDown(modifiers) ? 'B' : 'b';
			case Keys.C:
				return ShiftDown(modifiers) ? 'C' : 'c';
			case Keys.D:
				return ShiftDown(modifiers) ? 'D' : 'd';
			case Keys.E:
				return ShiftDown(modifiers) ? 'E' : 'e';
			case Keys.F:
				return ShiftDown(modifiers) ? 'F' : 'f';
			case Keys.G:
				return ShiftDown(modifiers) ? 'G' : 'g';
			case Keys.H:
				return ShiftDown(modifiers) ? 'H' : 'h';
			case Keys.I:
				return ShiftDown(modifiers) ? 'I' : 'i';
			case Keys.J:
				return ShiftDown(modifiers) ? 'J' : 'j';
			case Keys.K:
				return ShiftDown(modifiers) ? 'K' : 'k';
			case Keys.L:
				return ShiftDown(modifiers) ? 'L' : 'l';
			case Keys.M:
				return ShiftDown(modifiers) ? 'M' : 'm';
			case Keys.N:
				return ShiftDown(modifiers) ? 'N' : 'n';
			case Keys.O:
				return ShiftDown(modifiers) ? 'O' : 'o';
			case Keys.P:
				return ShiftDown(modifiers) ? 'P' : 'p';
			case Keys.Q:
				return ShiftDown(modifiers) ? 'Q' : 'q';
			case Keys.R:
				return ShiftDown(modifiers) ? 'R' : 'r';
			case Keys.S:
				return ShiftDown(modifiers) ? 'S' : 's';
			case Keys.T:
				return ShiftDown(modifiers) ? 'T' : 't';
			case Keys.U:
				return ShiftDown(modifiers) ? 'U' : 'u';
			case Keys.V:
				return ShiftDown(modifiers) ? 'V' : 'v';
			case Keys.W:
				return ShiftDown(modifiers) ? 'W' : 'w';
			case Keys.X:
				return ShiftDown(modifiers) ? 'X' : 'x';
			case Keys.Y:
				return ShiftDown(modifiers) ? 'Y' : 'y';
			case Keys.Z:
				return ShiftDown(modifiers) ? 'Z' : 'z';
			case Keys.D0 when !ShiftDown(modifiers):
			case Keys.NumPad0:
				return '0';
			case Keys.D1 when !ShiftDown(modifiers):
			case Keys.NumPad1:
				return '1';
			case Keys.D2 when !ShiftDown(modifiers):
			case Keys.NumPad2:
				return '2';
			case Keys.D3 when !ShiftDown(modifiers):
			case Keys.NumPad3:
				return '3';
			case Keys.D4 when !ShiftDown(modifiers):
			case Keys.NumPad4:
				return '4';
			case Keys.D5 when !ShiftDown(modifiers):
			case Keys.NumPad5:
				return '5';
			case Keys.D6 when !ShiftDown(modifiers):
			case Keys.NumPad6:
				return '6';
			case Keys.D7 when !ShiftDown(modifiers):
			case Keys.NumPad7:
				return '7';
			case Keys.D8 when !ShiftDown(modifiers):
			case Keys.NumPad8:
				return '8';
			case Keys.D9 when !ShiftDown(modifiers):
			case Keys.NumPad9:
				return '9';
			case Keys.D0 when ShiftDown(modifiers):
				return ')';
			case Keys.D1 when ShiftDown(modifiers):
				return '!';
			case Keys.D2 when ShiftDown(modifiers):
				return '@';
			case Keys.D3 when ShiftDown(modifiers):
				return '#';
			case Keys.D4 when ShiftDown(modifiers):
				return '$';
			case Keys.D5 when ShiftDown(modifiers):
				return '%';
			case Keys.D6 when ShiftDown(modifiers):
				return '^';
			case Keys.D7 when ShiftDown(modifiers):
				return '&';
			case Keys.D8 when ShiftDown(modifiers):
				return '*';
			case Keys.D9 when ShiftDown(modifiers):
				return '(';
			case Keys.Space:
				return ' ';
			case Keys.Tab:
				return '\t';
			case Keys.Enter:
				return '\n';
			case Keys.Add:
				return '+';
			case Keys.Subtract:
				return '-';
			case Keys.Decimal:
				return '.';
			case Keys.Divide:
				return '/';
			case Keys.Multiply:
				return '*';
			case Keys.OemBackslash:
				return '\\';
			case Keys.OemComma:
				return ShiftDown(modifiers) ? '<' : ',';
			case Keys.OemOpenBrackets:
				return ShiftDown(modifiers) ? '[' : '{';
			case Keys.OemCloseBrackets:
				return ShiftDown(modifiers) ? ']' : '}';
			case Keys.OemPeriod:
				return ShiftDown(modifiers) ? '>' : '.';
			case Keys.OemPipe:
				return ShiftDown(modifiers) ? '|' : '\\';
			case Keys.OemPlus:
				return ShiftDown(modifiers) ? '+' : '=';
			case Keys.OemMinus:
				return ShiftDown(modifiers) ? '_' : '-';
			case Keys.OemQuestion:
				return ShiftDown(modifiers) ? '?' : '/';
			case Keys.OemQuotes:
				return ShiftDown(modifiers) ? '"' : '\'';
			case Keys.OemSemicolon:
				return ShiftDown(modifiers) ? ':' : ';';
			case Keys.OemTilde:
				return ShiftDown(modifiers) ? '~' : '`';
			default:
				return null;
		}
	}

	public static bool AltDown(Modifiers modifiers) => (modifiers & Modifiers.LeftAlt) == Modifiers.LeftAlt || (modifiers & Modifiers.RightAlt) == Modifiers.RightAlt;

	public static bool ControlDown(Modifiers modifiers) => (modifiers & Modifiers.LeftControl) == Modifiers.LeftControl || (modifiers & Modifiers.RightControl) == Modifiers.RightControl;

	public static bool ShiftDown(Modifiers modifiers) => (modifiers & Modifiers.LeftShift) == Modifiers.LeftShift || (modifiers & Modifiers.RightShift) == Modifiers.RightShift;

	public static bool IsWhitespace(Keys key)
	{
		switch (key)
		{
			case Keys.Tab:
			case Keys.Space:
			case Keys.Enter:
				return true;
			default:
				return false;
		}
	}

	public static Modifiers GetModifiers(KeyboardState state)
	{
		Modifiers modifiers = Modifiers.None;

		if (state.IsKeyDown(Keys.LeftControl)) modifiers |= Modifiers.LeftControl;
		if (state.IsKeyDown(Keys.RightControl)) modifiers |= Modifiers.RightControl;
		if (state.IsKeyDown(Keys.LeftShift)) modifiers |= Modifiers.LeftShift;
		if (state.IsKeyDown(Keys.RightShift)) modifiers |= Modifiers.RightShift;
		if (state.IsKeyDown(Keys.LeftAlt)) modifiers |= Modifiers.LeftAlt;
		if (state.IsKeyDown(Keys.RightAlt)) modifiers |= Modifiers.RightAlt;

		return modifiers;
	}

	public static bool IsAlpha(Keys key) => (int)key >= (int)Keys.A && (int)key <= (int)Keys.Z;

	public static bool IsNumeric(Keys key)
	{
		switch (key)
		{
			case Keys.D0:
			case Keys.D1:
			case Keys.D2:
			case Keys.D3:
			case Keys.D4:
			case Keys.D5:
			case Keys.D6:
			case Keys.D7:
			case Keys.D8:
			case Keys.D9:
			case Keys.NumPad0:
			case Keys.NumPad1:
			case Keys.NumPad2:
			case Keys.NumPad3:
			case Keys.NumPad4:
			case Keys.NumPad5:
			case Keys.NumPad6:
			case Keys.NumPad7:
			case Keys.NumPad8:
			case Keys.NumPad9:
				return true;
			default:
				return false;
		}
	}
}