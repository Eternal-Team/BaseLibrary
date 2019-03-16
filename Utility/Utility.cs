using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BaseLibrary
{
	public static partial class Utility
	{
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
				foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Where(field => !field.FieldType.IsValueType && !field.IsLiteral && !type.HasAttribute<CompilerGeneratedAttribute>()))
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
}