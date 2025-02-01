using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace BaseLibrary;

public static class ILUtility
{
	public static void TryGotoNext(ILCursor cursor, params Func<Instruction, bool>[] predicates)
	{
		if (!cursor.TryGotoNext(predicates))
			throw new Exception($"Could not find matching instruction in {cursor.Method.FullName}");
	}

	public static void TryGotoNext(ILCursor cursor, MoveType moveType, params Func<Instruction, bool>[] predicates)
	{
		if (!cursor.TryGotoNext(moveType, predicates))
			throw new Exception($"Could not find matching instruction in {cursor.Method.FullName}");
	}
}