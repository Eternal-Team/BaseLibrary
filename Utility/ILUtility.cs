using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace BaseLibrary.Utility
{
	public static class ILUtility
	{
		public static int AddVariable<T>(this ILContext context)
		{
			context.Body.Variables.Add(new VariableDefinition(context.Import(typeof(T))));
			return context.Body.Variables.Count - 1;
		}
	}
}