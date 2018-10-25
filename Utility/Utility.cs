using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace BaseLibrary.Utility
{
	public static partial class Utility
	{
		public static bool Contains(this Rectangle rectangle, Point16 point) => rectangle.Contains(point.X, point.Y);

		public static bool Contains(this Rectangle rectangle, Vector2 vector) => rectangle.Contains((int)vector.X, (int)vector.Y);

		public static byte[] CompressBytes(this byte[] data)
		{
			MemoryStream output = new MemoryStream();
			using (var deflateStream = new DeflateStream(output, CompressionLevel.Optimal, false)) deflateStream.Write(data, 0, data.Length);
			return output.ToArray();
		}

		public static byte[] DecompressBytes(this byte[] data)
		{
			MemoryStream input = new MemoryStream(data);
			MemoryStream output = new MemoryStream();
			using (var deflateStream = new DeflateStream(input, CompressionMode.Decompress)) deflateStream.CopyTo(output);
			return output.ToArray();
		}

		public static void UnloadNullableTypes()
		{
			foreach (Type type in Assembly.GetCallingAssembly().GetTypes())
			{
				foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
				{
					if (!field.FieldType.IsValueType && !field.IsLiteral && !type.HasAttribute<CompilerGeneratedAttribute>())
					{
						if (field.FieldType.IsGenericType)
						{
							if (field.FieldType.GetGenericTypeDefinition() == typeof(List<>)) ((IList)field.GetValue(null)).Clear();
							else if (field.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>)) ((IDictionary)field.GetValue(null)).Clear();
						}
						else field.SetValue(null, null);
					}
				}
			}
		}

		public static IEnumerable<object> ToEnumerable(this object tuple)
		{
			foreach (FieldInfo field in tuple.GetType().GetFields())
			{
				if (field.Name == "Rest")
					foreach (object o in field.GetValue(tuple).ToEnumerable())
						yield return o;
				else yield return field.GetValue(tuple);
			}
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