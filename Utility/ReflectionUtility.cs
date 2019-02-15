using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace BaseLibrary.Utility
{
	public static partial class Utility
	{
		public const BindingFlags defaultFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

		public static FieldInfo GetFieldInfo(this Type type, string name, BindingFlags flags = defaultFlags) => type.GetField(name, flags);

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
					throw new NotImplementedException();
			}
		}

		public static void SetValue(this Type type, string name, object value, object obj = null, BindingFlags flags = defaultFlags)
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
					throw new NotImplementedException();
			}
		}

		// instance, w flags
		public static T InvokeMethod<T>(this Type type, string name, object obj, BindingFlags flags, params object[] args)
		{
			MethodInfo info = type.GetMethod(name, flags);
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

			return default(T);
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

		public static bool HasAttribute<T>(this MemberInfo field) => field.GetCustomAttributes().Any(x => x.GetType() == typeof(T));

		public static Type GetUnderlyingType(this MemberInfo member)
		{
			switch (member.MemberType)
			{
				case MemberTypes.Event:
					return ((EventInfo)member).EventHandlerType;
				case MemberTypes.Field:
					return ((FieldInfo)member).FieldType;
				case MemberTypes.Method:
					return ((MethodInfo)member).ReturnType;
				case MemberTypes.Property:
					return ((PropertyInfo)member).PropertyType;
				default:
					throw new ArgumentException("Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo");
			}
		}

		public static T MemberwiseClone<T>(this T source)
		{
			var clone = Activator.CreateInstance(source.GetType());
			for (Type type = source.GetType(); type != null; type = type.BaseType)
			{
				FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
				foreach (FieldInfo field in fields) field.SetValue(clone, field.GetValue(source));
			}

			return (T)clone;
		}

		public static bool IsEnumerable(this PropertyInfo property) => property.PropertyType.GetInterface(typeof(IEnumerable).FullName) != null;

		public static bool IsList(this PropertyInfo property) => property.PropertyType.GetInterface(typeof(IList).FullName) != null;
	}
}