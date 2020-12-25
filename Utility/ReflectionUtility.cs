using System;
using System.Reflection;

namespace BaseLibrary.Utility
{
	public static class ReflectionUtility
	{
		public const BindingFlags DefaultFlags_Static = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		public const BindingFlags DefaultFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		public static void SetValue(this Type type, string name, object value, object obj = null, BindingFlags flags = DefaultFlags)
		{
			PropertyInfo propertyInfo = type.GetProperty(name, flags);
			if (propertyInfo != null) propertyInfo.SetValue(obj, value);
			else type.GetField(name, flags)?.SetValue(obj, value);
		}

		public static void SetValue(this object obj, string name, object value, BindingFlags flags = DefaultFlags) => obj.GetType().SetValue(name, value, obj, flags);

		public static T Invoke<T>(this MethodInfo info, object target, params object[] args) => (T)info.Invoke(target, args);
		public static void Invoke(this MethodInfo info, object target, params object[] args) => info.Invoke(target, args);
	}
}