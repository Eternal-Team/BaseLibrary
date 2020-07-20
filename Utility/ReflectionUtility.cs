using System;
using System.Reflection;

namespace BaseLibrary.Utility
{
	public static class ReflectionUtility
	{
		public const BindingFlags DefaultFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

		public static void SetValue(this Type type, string name, object value, object obj = null, BindingFlags flags = DefaultFlags)
		{
			if (type.GetProperty(name, flags) != null) type.GetProperty(name, flags)?.SetValue(obj, value);
			else type.GetField(name, flags)?.SetValue(obj, value);
		}

		public static void SetValue(this object obj, string name, object value, BindingFlags flags = DefaultFlags) => obj.GetType().SetValue(name, value, obj, flags);
	}
}