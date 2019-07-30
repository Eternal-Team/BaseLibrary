using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BaseLibrary
{
	public static partial class Utility
	{
		public static class Cache
		{
			public static Item[] ItemCache { get; internal set; }
			public static NPC[] NPCCache { get; internal set; }
			public static Projectile[] ProjectileCache { get; internal set; }

			public static void Load()
			{
				ItemCache = new Item[ItemLoader.ItemCount];
				NPCCache = new NPC[NPCLoader.NPCCount];
				ProjectileCache = new Projectile[ProjectileLoader.ProjectileCount];

				for (int type = 0; type < ItemLoader.ItemCount; type++)
				{
					try
					{
						Item item = new Item();
						item.SetDefaults(type);
						ItemCache[type] = item;
					}
					catch
					{
					}
				}

				for (int type = 0; type < NPCLoader.NPCCount; type++)
				{
					try
					{
						NPC npc = new NPC();
						npc.SetDefaults(type);
						NPCCache[type] = npc;
					}
					catch
					{
					}
				}

				for (int type = 0; type < ProjectileLoader.ProjectileCount; type++)
				{
					try
					{
						Projectile projectile = new Projectile();
						projectile.SetDefaults(type);
						ProjectileCache[type] = projectile;
					}
					catch
					{
					}
				}
			}
		}

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
						if (field.FieldType.GetGenericTypeDefinition() == typeof(List<>)) ((IList)field.GetValue(null))?.Clear();
						else if (field.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>)) ((IDictionary)field.GetValue(null))?.Clear();
					}
					else field.SetValue(null, null);
				}
			}
		}

		public static int AddVariable(this ILContext context, Type type)
		{
			context.Body.Variables.Add(new VariableDefinition(context.Import(type)));
			return context.Body.Variables.Count - 1;
		}

		public static int GetParameterIndex(this ILContext context, string name)
		{
			ParameterDefinition def = context.Method.Parameters.FirstOrDefault(parameter => parameter.Name == name);
			return def?.Index + 1 ?? throw new Exception($"Parameter with name '{name}' does not exist!");
		}

		public static void Write(this BinaryWriter writer, Point16 point)
		{
			writer.Write(point.X);
			writer.Write(point.Y);
		}

		public static Point16 ReadPoint16(this BinaryReader reader) => new Point16(reader.ReadInt16(), reader.ReadInt16());

		public static void Write(this BinaryWriter writer, Guid guid)
		{
			writer.Write(guid.ToString());
		}

		public static Guid ReadGUID(this BinaryReader reader) => Guid.Parse(reader.ReadString());

		public static string GetTranslation(this ModTranslation translation) => translation.GetTranslation(Language.ActiveCulture);

		public static void WriteFile(this BinaryWriter writer, string path)
		{
			int count = 0;
			byte[] array = new byte[0];
			if (File.Exists(path))
			{
				array = File.ReadAllBytes(path);
				count = array.Length;
			}

			writer.Write(count);
			writer.Write(array);
		}

		public static byte[] ReadFile(this BinaryReader reader)
		{
			int count = reader.ReadInt32();
			return count > 0 ? reader.ReadBytes(count) : null;
		}
	}
}