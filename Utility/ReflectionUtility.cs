using System;
using System.Reflection;

namespace BaseLibrary;

public readonly struct StaticField<T>(FieldInfo fieldInfo)
{
	public T? GetValue() => (T?)fieldInfo.GetValue(null);

	public void SetValue(T? value) => fieldInfo.SetValue(null, value);
}

public readonly struct StaticMethod<T>(MethodInfo methodInfo)
{
	public T? Invoke(object[]? args = null) => (T?)methodInfo.Invoke(null, args);
}

public readonly struct StaticMethod(MethodInfo methodInfo)
{
	public void Invoke(object[]? args = null) => methodInfo.Invoke(null, args);
}

public static class ReflectionUtility
{
	public const BindingFlags DefaultFlags_Static = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
	public const BindingFlags DefaultFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

	public static StaticField<K> GetField<T, K>(string field, BindingFlags flags = DefaultFlags_Static)
	{
		FieldInfo? fieldInfo = typeof(T).GetField(field, flags);
		if (fieldInfo is null) throw new Exception($"Failed to find field '{field}' in {typeof(T).FullName}");
		return new StaticField<K>(fieldInfo);
	}

	public static StaticMethod<K> GetMethod<T, K>(string method, BindingFlags flags = DefaultFlags_Static)
	{
		MethodInfo? methodInfo = typeof(T).GetMethod(method, flags);
		if (methodInfo is null) throw new Exception($"Failed to find method '{method}' in {typeof(T).FullName}");
		return new StaticMethod<K>(methodInfo);
	}

	public static StaticMethod GetMethod<T>(string method, BindingFlags flags = DefaultFlags | BindingFlags.Static)
	{
		MethodInfo? methodInfo = typeof(T).GetMethod(method, flags);
		if (methodInfo is null) throw new Exception($"Failed to find method '{method}' in {typeof(T).FullName}");
		return new StaticMethod(methodInfo);
	}

	public static T? GetValue<T>(this FieldInfo fieldInfo)
	{
		return (T?)fieldInfo.GetValue(null);
	}

	public static void SetValue<T>(this FieldInfo fieldInfo, T value)
	{
		fieldInfo.SetValue(null, value);
	}
}