﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.ModLoader;

namespace BaseLibrary
{
	public static partial class Utility
	{
		public static class Cache
		{
			public static List<Item> ItemCache { get; internal set; }
			public static List<NPC> NPCCache { get; internal set; }
			public static List<Projectile> ProjectileCache { get; internal set; }

			public static void Load()
			{
				ItemCache = new List<Item>();
				NPCCache = new List<NPC>();
				ProjectileCache = new List<Projectile>();

				for (int type = 0; type < ItemLoader.ItemCount; type++)
				{
					try
					{
						Item item = new Item();
						item.SetDefaults(type, false);
						ItemCache.Add(item);
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
						NPCCache.Add(npc);
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
						ProjectileCache.Add(projectile);
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
	}
}