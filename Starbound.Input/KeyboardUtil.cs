using Microsoft.Xna.Framework.Input;

namespace BaseLibrary.Input
{
	public static class KeyboardUtil
	{
		/// <summary>
		///     Returns a character that represents the key that was pressed, including accounting for pressing
		///     the shift key. In the event that they supplied key doesn't translate into a character, null
		///     is returned instead.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="modifiers"></param>
		/// <returns></returns>
		public static char? ToChar(Keys key, Modifiers modifiers = Modifiers.None)
		{
			if (key == Keys.A) return ShiftDown(modifiers) ? 'A' : 'a';
			if (key == Keys.B) return ShiftDown(modifiers) ? 'B' : 'b';
			if (key == Keys.C) return ShiftDown(modifiers) ? 'C' : 'c';
			if (key == Keys.D) return ShiftDown(modifiers) ? 'D' : 'd';
			if (key == Keys.E) return ShiftDown(modifiers) ? 'E' : 'e';
			if (key == Keys.F) return ShiftDown(modifiers) ? 'F' : 'f';
			if (key == Keys.G) return ShiftDown(modifiers) ? 'G' : 'g';
			if (key == Keys.H) return ShiftDown(modifiers) ? 'H' : 'h';
			if (key == Keys.I) return ShiftDown(modifiers) ? 'I' : 'i';
			if (key == Keys.J) return ShiftDown(modifiers) ? 'J' : 'j';
			if (key == Keys.K) return ShiftDown(modifiers) ? 'K' : 'k';
			if (key == Keys.L) return ShiftDown(modifiers) ? 'L' : 'l';
			if (key == Keys.M) return ShiftDown(modifiers) ? 'M' : 'm';
			if (key == Keys.N) return ShiftDown(modifiers) ? 'N' : 'n';
			if (key == Keys.O) return ShiftDown(modifiers) ? 'O' : 'o';
			if (key == Keys.P) return ShiftDown(modifiers) ? 'P' : 'p';
			if (key == Keys.Q) return ShiftDown(modifiers) ? 'Q' : 'q';
			if (key == Keys.R) return ShiftDown(modifiers) ? 'R' : 'r';
			if (key == Keys.S) return ShiftDown(modifiers) ? 'S' : 's';
			if (key == Keys.T) return ShiftDown(modifiers) ? 'T' : 't';
			if (key == Keys.U) return ShiftDown(modifiers) ? 'U' : 'u';
			if (key == Keys.V) return ShiftDown(modifiers) ? 'V' : 'v';
			if (key == Keys.W) return ShiftDown(modifiers) ? 'W' : 'w';
			if (key == Keys.X) return ShiftDown(modifiers) ? 'X' : 'x';
			if (key == Keys.Y) return ShiftDown(modifiers) ? 'Y' : 'y';
			if (key == Keys.Z) return ShiftDown(modifiers) ? 'Z' : 'z';

			if (key == Keys.D0 && !ShiftDown(modifiers) || key == Keys.NumPad0) return '0';
			if (key == Keys.D1 && !ShiftDown(modifiers) || key == Keys.NumPad1) return '1';
			if (key == Keys.D2 && !ShiftDown(modifiers) || key == Keys.NumPad2) return '2';
			if (key == Keys.D3 && !ShiftDown(modifiers) || key == Keys.NumPad3) return '3';
			if (key == Keys.D4 && !ShiftDown(modifiers) || key == Keys.NumPad4) return '4';
			if (key == Keys.D5 && !ShiftDown(modifiers) || key == Keys.NumPad5) return '5';
			if (key == Keys.D6 && !ShiftDown(modifiers) || key == Keys.NumPad6) return '6';
			if (key == Keys.D7 && !ShiftDown(modifiers) || key == Keys.NumPad7) return '7';
			if (key == Keys.D8 && !ShiftDown(modifiers) || key == Keys.NumPad8) return '8';
			if (key == Keys.D9 && !ShiftDown(modifiers) || key == Keys.NumPad9) return '9';
			if (key == Keys.D0 && ShiftDown(modifiers)) return ')';
			if (key == Keys.D1 && ShiftDown(modifiers)) return '!';
			if (key == Keys.D2 && ShiftDown(modifiers)) return '@';
			if (key == Keys.D3 && ShiftDown(modifiers)) return '#';
			if (key == Keys.D4 && ShiftDown(modifiers)) return '$';
			if (key == Keys.D5 && ShiftDown(modifiers)) return '%';
			if (key == Keys.D6 && ShiftDown(modifiers)) return '^';
			if (key == Keys.D7 && ShiftDown(modifiers)) return '&';
			if (key == Keys.D8 && ShiftDown(modifiers)) return '*';
			if (key == Keys.D9 && ShiftDown(modifiers)) return '(';

			if (key == Keys.Space) return ' ';
			if (key == Keys.Tab) return '\t';
			if (key == Keys.Enter) return '\n';

			if (key == Keys.Add) return '+';
			if (key == Keys.Subtract) return '-';
			if (key == Keys.Decimal) return '.';
			if (key == Keys.Divide) return '/';
			if (key == Keys.Multiply) return '*';

			if (key == Keys.OemBackslash) return '\\';
			if (key == Keys.OemComma) return ShiftDown(modifiers) ? '<' : ',';
			if (key == Keys.OemOpenBrackets) return ShiftDown(modifiers) ? '[' : '{';
			if (key == Keys.OemCloseBrackets) return ShiftDown(modifiers) ? ']' : '}';
			if (key == Keys.OemPeriod) return ShiftDown(modifiers) ? '>' : '.';
			if (key == Keys.OemPipe) return ShiftDown(modifiers) ? '|' : '\\';
			if (key == Keys.OemPlus) return ShiftDown(modifiers) ? '+' : '=';
			if (key == Keys.OemMinus) return ShiftDown(modifiers) ? '_' : '-';
			if (key == Keys.OemQuestion) return ShiftDown(modifiers) ? '?' : '/';
			if (key == Keys.OemQuotes) return ShiftDown(modifiers) ? '"' : '\'';
			if (key == Keys.OemSemicolon) return ShiftDown(modifiers) ? ':' : ';';
			if (key == Keys.OemTilde) return ShiftDown(modifiers) ? '~' : '`';

			return null;
		}

		/// <summary>
		///     Returns whether the given modifiers includes the Alt key.
		/// </summary>
		/// <param name="modifiers"></param>
		/// <returns></returns>
		public static bool AltDown(Modifiers modifiers)
		{
			return (modifiers & Modifiers.Alt) == Modifiers.Alt;
		}

		/// <summary>
		///     Returns whether the given modifiers includes the Control key.
		/// </summary>
		/// <param name="modifiers"></param>
		/// <returns></returns>
		public static bool ControlDown(Modifiers modifiers)
		{
			return (modifiers & Modifiers.Control) == Modifiers.Control;
		}

		/// <summary>
		///     Returns whether given modifiers include the Shift key.
		/// </summary>
		/// <param name="modifiers"></param>
		/// <returns></returns>
		public static bool ShiftDown(Modifiers modifiers)
		{
			return (modifiers & Modifiers.Shift) == Modifiers.Shift;
		}

		/// <summary>
		///     Returns whether the key represents a whitespace character.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
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

		/// <summary>
		///     Returns whether the key represents an alphabetical symbol.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool IsAlpha(Keys key)
		{
			return (int)key >= (int)Keys.A && (int)key <= (int)Keys.Z;
		}

		/// <summary>
		///     Returns whether the key represents a numeric symbol.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
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
}