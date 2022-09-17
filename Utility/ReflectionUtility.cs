using System;
using System.Reflection;

namespace BaseLibrary.Utility;

public static class ReflectionUtility
{
	public const BindingFlags DefaultFlags_Static = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
	public const BindingFlags DefaultFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

	#region SetValue
	public static void SetValue(this Type type, string name, object value, object? obj = null, BindingFlags flags = DefaultFlags)
	{
		PropertyInfo? propertyInfo = type.GetProperty(name, flags);
		if (propertyInfo is not null)
		{
			propertyInfo.SetValue(obj, value);
			return;
		}

		FieldInfo? fieldInfo = type.GetField(name, flags);
		if (fieldInfo is not null)
		{
			fieldInfo.SetValue(obj, value);
			return;
		}

		throw new Exception("Unknown field or property " + name);
	}

	public static void SetValue(this object obj, string name, object value, BindingFlags flags = DefaultFlags) => obj.GetType().SetValue(name, value, obj, flags);
	#endregion

	#region GetValue
	public static T? GetValue<T>(this FieldInfo fieldInfo, object obj) => (T?)fieldInfo.GetValue(obj);

	public static T? GetValueStatic<T>(this FieldInfo fieldInfo) => (T?)fieldInfo.GetValue(null);

	public static T? GetValue<T>(this PropertyInfo propertyInfo, object obj) => (T?)propertyInfo.GetValue(obj);

	public static T? GetValueStatic<T>(this PropertyInfo propertyInfo) => (T?)propertyInfo.GetValue(null);

	public static T? GetValue<T>(this Type type, string name, object? obj = null, BindingFlags flags = DefaultFlags)
	{
		PropertyInfo? propertyInfo = type.GetProperty(name, flags);
		if (propertyInfo is not null)
			return (T?)propertyInfo.GetValue(obj);

		return (T?)type.GetField(name, flags)?.GetValue(obj);
	}

	public static T? GetValue<T>(this object obj, string name, BindingFlags flags = DefaultFlags) => obj.GetType().GetValue<T>(name, obj, flags);
	#endregion

	public static T? Invoke<T>(this MethodInfo info, object target, params object[] args) => (T)info.Invoke(target, args);

	public static T? InvokeStatic<T>(this MethodInfo info, params object[] args) => (T)info.Invoke(null, args);
	public static void InvokeStatic(this MethodInfo info, params object[] args) => info.Invoke(null, args);

	public static TRet InvokeGeneric<TGen, TRet>(this MethodInfo info, object target, params object[] args)
	{
		return (TRet)info.MakeGenericMethod(typeof(TGen)).Invoke(target, args);
	}

	public static void Invoke(this MethodInfo info, object target, params object[] args) => info.Invoke(target, args);

	public static void Invoke(this object obj, string name, BindingFlags flags = DefaultFlags, params object[] args)
	{
		obj.GetType().GetMethod(name, flags)?.Invoke(obj, args);
	}

	public static T Invoke<T>(this object obj, string name, BindingFlags flags = DefaultFlags, params object[] args)
	{
		return (T)obj.GetType().GetMethod(name, flags)?.Invoke(obj, args);
	}

	public static bool IsSubclassOfRawGeneric(Type toCheck, Type generic)
	{
		while (toCheck != null && toCheck != typeof(object))
		{
			var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
			if (generic == cur) return true;

			toCheck = toCheck.BaseType;
		}

		return false;
	}

	// public static T CreateInstance<T>(params object[] args)
	// {
	// 	if (args.Length == 0)
	// 	{
	// 		var constuctors = typeof(T).GetConstructors()
	// 			.Where(ctor =>
	// 			{
	// 				var parameters = ctor.GetParameters();
	// 				return !parameters.Any() || parameters.All(x => x.GetCustomAttribute<OptionalAttribute>() != null);
	// 			});
	// 	}
	// 	
	//
	//
	// 	NewExpression newExp = Expression.New(typeof(T));
	//
	// 	// Create a new lambda expression with the NewExpression as the body.
	// 	var lambda = Expression.Lambda<Func<T>>(newExp);
	//
	// 	// Compile our new lambda expression.
	// 	return lambda.Compile().Invoke();
	//
	// 	return args.Length <= 0 ? Activator.CreateInstance<T>() : (T)Activator.CreateInstance(typeof(T), args);
	// }
}