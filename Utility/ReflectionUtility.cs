using System;
using System.Linq;
using System.Reflection;

namespace BaseLibrary
{
	public static partial class Utility
	{
		public const BindingFlags defaultFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

		public static FieldInfo GetField(this Type type, string name, BindingFlags flags = defaultFlags) => type.GetField(name, flags);

		public static PropertyInfo GetProperty(this Type type, string name, BindingFlags flags = defaultFlags) => type.GetProperty(name, flags);

		public static T GetValue<T>(this Type type, string name, object obj = null, BindingFlags flags = defaultFlags) => (T)(type.GetProperty(name, flags)?.GetValue(obj) ?? type.GetField(name, flags)?.GetValue(obj));

		public static T GetValue<T>(this object obj, string name, BindingFlags flags = defaultFlags) => obj.GetType().GetValue<T>(name, obj, flags);

		public static T GetValue<T>(this MemberInfo memberInfo, object obj)
		{
			switch (memberInfo.MemberType)
			{
				case MemberTypes.Field:
					return (T)Convert.ChangeType(((FieldInfo)memberInfo).GetValue(obj), typeof(T));
				case MemberTypes.Property:
					return (T)Convert.ChangeType(((PropertyInfo)memberInfo).GetValue(obj), typeof(T));
				default:
					throw new ArgumentException();
			}
		}

		public static void SetValue(this Type type, string name, object value, object obj = null, BindingFlags flags = defaultFlags)
		{
			if (type.GetProperty(name, flags) != null) type.GetProperty(name, flags)?.SetValue(obj, value);
			else type.GetField(name, flags)?.SetValue(obj, value);
		}

		public static void SetValue<T>(this Type type, string name, T value, object obj = null, BindingFlags flags = defaultFlags)
		{
			if (type.GetProperty(name, flags) != null) type.GetProperty(name, flags)?.SetValue(obj, value);
			else type.GetField(name, flags)?.SetValue(obj, value);
		}

		public static void SetValue(this object obj, string name, object value, BindingFlags flags = defaultFlags) => obj.GetType().SetValue(name, value, obj, flags);

		public static void SetValue(this MemberInfo memberInfo, object value, object obj = null)
		{
			switch (memberInfo.MemberType)
			{
				case MemberTypes.Field:
					((FieldInfo)memberInfo).SetValue(obj, value);
					break;
				case MemberTypes.Property:
					((PropertyInfo)memberInfo).SetValue(obj, value);
					break;
				default:
					throw new ArgumentException();
			}
		}

		// instance, w flags
		public static T InvokeMethod<T>(this Type type, string name, object obj, BindingFlags flags, params object[] args)
		{
			Type[] types = Type.EmptyTypes;
			if (args.Length > 0) types = args.Select(x => x.GetType()).ToArray();
			MethodInfo info = type.GetMethod(name, flags, null, types, null);
			if (info != null)
			{
				if (args != null)
				{
					int oldLength = args.Length;
					int newLength = info.GetParameters().Length;

					Array.Resize(ref args, newLength);

					for (int i = newLength - 1; i > oldLength; i--) args[i] = Type.Missing;
				}

				return (T)info.Invoke(obj, args);
			}

			return default;
		}

		// instance object, w/o flags
		public static T InvokeMethod<T>(this Type type, string name, object obj, params object[] args) => type.InvokeMethod<T>(name, obj, defaultFlags, args);

		// static object, w flags
		public static T InvokeMethod<T>(this Type type, string name, BindingFlags flags, params object[] args) => type.InvokeMethod<T>(name, null, flags, args);

		// static objects, w/o flags
		public static T InvokeMethod<T>(this Type type, string name, params object[] args) => type.InvokeMethod<T>(name, null, defaultFlags, args);

		// instance, w flags
		public static T InvokeMethod<T>(this object obj, string name, BindingFlags flags, params object[] args) => obj.GetType().InvokeMethod<T>(name, obj, flags, args);

		// instance, w/o flags
		public static T InvokeMethod<T>(this object obj, string name, params object[] args) => obj.GetType().InvokeMethod<T>(name, obj, defaultFlags, args);

		public static void InvokeMethod(this object obj, string name, params object[] args) => obj.GetType().InvokeMethod<object>(name, obj, defaultFlags, args);

		public static void InvokeMethod(this Type type, string name, params object[] args) => type.InvokeMethod<object>(name, null, defaultFlags, args);

		public static bool HasAttribute<T>(this MemberInfo field) where T : Attribute => field.GetCustomAttribute<T>() != null;

		public static bool TryGetAttribute<T>(this MemberInfo field, out T attribute) where T : Attribute
		{
			attribute = field.GetCustomAttribute<T>();
			return attribute != null;
		}

		public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
		{
			while (toCheck != null && toCheck != typeof(object))
			{
				var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
				if (generic == cur) return true;
				toCheck = toCheck.BaseType;
			}

			return false;
		}
	}
}