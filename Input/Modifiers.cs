using System;

namespace BaseLibrary;

[Flags]
public enum Modifiers
{
	None = 0,
	LeftControl = 1,
	RightControl = 2,
	LeftShift = 4,
	RightShift = 8,
	LeftAlt = 16,
	RightAlt = 32
}