using Microsoft.Xna.Framework.Input;

namespace BaseLibrary.Input;

public static class KeyboardUtil
{
	// todo: IME works afaik, only problem is with my UITextInput
	public static char? ToChar(Keys key, Modifiers modifiers = Modifiers.None) => key switch
	{
		Keys.A => ShiftDown(modifiers) ? 'A' : 'a',
		Keys.B => ShiftDown(modifiers) ? 'B' : 'b',
		Keys.C => ShiftDown(modifiers) ? 'C' : 'c',
		Keys.D => ShiftDown(modifiers) ? 'D' : 'd',
		Keys.E => ShiftDown(modifiers) ? 'E' : 'e',
		Keys.F => ShiftDown(modifiers) ? 'F' : 'f',
		Keys.G => ShiftDown(modifiers) ? 'G' : 'g',
		Keys.H => ShiftDown(modifiers) ? 'H' : 'h',
		Keys.I => ShiftDown(modifiers) ? 'I' : 'i',
		Keys.J => ShiftDown(modifiers) ? 'J' : 'j',
		Keys.K => ShiftDown(modifiers) ? 'K' : 'k',
		Keys.L => ShiftDown(modifiers) ? 'L' : 'l',
		Keys.M => ShiftDown(modifiers) ? 'M' : 'm',
		Keys.N => ShiftDown(modifiers) ? 'N' : 'n',
		Keys.O => ShiftDown(modifiers) ? 'O' : 'o',
		Keys.P => ShiftDown(modifiers) ? 'P' : 'p',
		Keys.Q => ShiftDown(modifiers) ? 'Q' : 'q',
		Keys.R => ShiftDown(modifiers) ? 'R' : 'r',
		Keys.S => ShiftDown(modifiers) ? 'S' : 's',
		Keys.T => ShiftDown(modifiers) ? 'T' : 't',
		Keys.U => ShiftDown(modifiers) ? 'U' : 'u',
		Keys.V => ShiftDown(modifiers) ? 'V' : 'v',
		Keys.W => ShiftDown(modifiers) ? 'W' : 'w',
		Keys.X => ShiftDown(modifiers) ? 'X' : 'x',
		Keys.Y => ShiftDown(modifiers) ? 'Y' : 'y',
		Keys.Z => ShiftDown(modifiers) ? 'Z' : 'z',
		Keys.D0 when !ShiftDown(modifiers) => '0',
		Keys.NumPad0 => '0',
		Keys.D1 when !ShiftDown(modifiers) => '1',
		Keys.NumPad1 => '1',
		Keys.D2 when !ShiftDown(modifiers) => '2',
		Keys.NumPad2 => '2',
		Keys.D3 when !ShiftDown(modifiers) => '3',
		Keys.NumPad3 => '3',
		Keys.D4 when !ShiftDown(modifiers) => '4',
		Keys.NumPad4 => '4',
		Keys.D5 when !ShiftDown(modifiers) => '5',
		Keys.NumPad5 => '5',
		Keys.D6 when !ShiftDown(modifiers) => '6',
		Keys.NumPad6 => '6',
		Keys.D7 when !ShiftDown(modifiers) => '7',
		Keys.NumPad7 => '7',
		Keys.D8 when !ShiftDown(modifiers) => '8',
		Keys.NumPad8 => '8',
		Keys.D9 when !ShiftDown(modifiers) => '9',
		Keys.NumPad9 => '9',
		Keys.D0 when ShiftDown(modifiers) => ')',
		Keys.D1 when ShiftDown(modifiers) => '!',
		Keys.D2 when ShiftDown(modifiers) => '@',
		Keys.D3 when ShiftDown(modifiers) => '#',
		Keys.D4 when ShiftDown(modifiers) => '$',
		Keys.D5 when ShiftDown(modifiers) => '%',
		Keys.D6 when ShiftDown(modifiers) => '^',
		Keys.D7 when ShiftDown(modifiers) => '&',
		Keys.D8 when ShiftDown(modifiers) => '*',
		Keys.D9 when ShiftDown(modifiers) => '(',
		Keys.Space => ' ',
		Keys.Tab => '\t',
		Keys.Enter => '\n',
		Keys.Add => '+',
		Keys.Subtract => '-',
		Keys.Decimal => '.',
		Keys.Divide => '/',
		Keys.Multiply => '*',
		Keys.OemBackslash => '\\',
		Keys.OemComma => ShiftDown(modifiers) ? '<' : ',',
		Keys.OemOpenBrackets => ShiftDown(modifiers) ? '[' : '{',
		Keys.OemCloseBrackets => ShiftDown(modifiers) ? ']' : '}',
		Keys.OemPeriod => ShiftDown(modifiers) ? '>' : '.',
		Keys.OemPipe => ShiftDown(modifiers) ? '|' : '\\',
		Keys.OemPlus => ShiftDown(modifiers) ? '+' : '=',
		Keys.OemMinus => ShiftDown(modifiers) ? '_' : '-',
		Keys.OemQuestion => ShiftDown(modifiers) ? '?' : '/',
		Keys.OemQuotes => ShiftDown(modifiers) ? '"' : '\'',
		Keys.OemSemicolon => ShiftDown(modifiers) ? ':' : ';',
		Keys.OemTilde => ShiftDown(modifiers) ? '~' : '`',
		_ => null
	};

	public static bool AltDown(Modifiers modifiers) => (modifiers & Modifiers.LeftAlt) == Modifiers.LeftAlt || (modifiers & Modifiers.RightAlt) == Modifiers.RightAlt;

	public static bool ControlDown(Modifiers modifiers) => (modifiers & Modifiers.LeftControl) == Modifiers.LeftControl || (modifiers & Modifiers.RightControl) == Modifiers.RightControl;

	public static bool ShiftDown(Modifiers modifiers) => (modifiers & Modifiers.LeftShift) == Modifiers.LeftShift || (modifiers & Modifiers.RightShift) == Modifiers.RightShift;

	public static bool IsWhitespace(Keys key) => key switch
	{
		Keys.Tab or Keys.Space or Keys.Enter => true,
		_ => false
	};

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

	public static bool IsNumeric(Keys key) => key switch
	{
		Keys.D0 or Keys.D1 or Keys.D2 or Keys.D3 or Keys.D4 or Keys.D5 or Keys.D6 or Keys.D7 or Keys.D8 or Keys.D9 or Keys.NumPad0 or Keys.NumPad1 or Keys.NumPad2 or Keys.NumPad3 or Keys.NumPad4 or Keys.NumPad5 or Keys.NumPad6 or Keys.NumPad7 or Keys.NumPad8 or Keys.NumPad9 => true,
		_ => false
	};
}