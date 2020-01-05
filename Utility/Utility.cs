using log4net;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

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

		public static void UnloadNullableTypes(this Mod mod)
		{
			ILog logger = LogManager.GetLogger("BaseLibrary");

			logger.Info($"Unloading nullable fields for mod: {mod.DisplayName}");

			foreach (Type type in mod.Code.GetTypes())
			{
				foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Where(field => !field.FieldType.IsValueType && !field.IsLiteral && !type.HasAttribute<CompilerGeneratedAttribute>()))
				{
					logger.Info($"Unloading field: {type.FullName}.{field.Name} [of type: {field.FieldType.Name}]");

					if (field.FieldType.IsGenericType)
					{
						if (field.FieldType.GetGenericTypeDefinition() == typeof(List<>)) ((IList)field.GetValue(null))?.Clear();
						else if (field.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>)) ((IDictionary)field.GetValue(null))?.Clear();
					}
					else field.SetValue(null, null);
				}
			}
		}

		public static int AddVariable<T>(this ILContext context)
		{
			context.Body.Variables.Add(new VariableDefinition(context.Import(typeof(T))));
			return context.Body.Variables.Count - 1;
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

		public static void Send(this BinaryWriter writer, Item item, bool writeStack = false, bool writeFavourite = false)
		{
			writer.Write((short)item.netID);
			writer.Write(item.prefix);
			if (writeStack) writer.Write(item.stack);
			if (writeFavourite) writer.Write(item.favorited);
			ItemIO.SendModData(item, writer);
		}

		public static Item Receive(this BinaryReader reader, bool readStack = false, bool readFavorite = false)
		{
			var item = new Item();

			item.netDefaults(reader.ReadInt16());
			item.Prefix(reader.ReadByte());
			if (readStack) item.stack = reader.ReadInt32();
			if (readFavorite) item.favorited = reader.ReadBoolean();
			ItemIO.ReceiveModData(item, reader);

			return item;
		}

		public static bool Contains(this Rectangle rectangle, Vector2 position) => rectangle.Contains(position.ToPoint());

		public static bool PingHost(string hostUri, int portNumber)
		{
			try
			{
				using (var client = new TcpClient())
				{
					client.Connect(hostUri, portNumber);
					return true;
				}
			}
			catch
			{
				return false;
			}
		}

		public static bool CheckAABBvAABBCollision(Rectangle a, Rectangle b) => a.X < b.X + b.Width && a.X + a.Width > b.X && a.Y < b.Y + b.Height && a.Y + a.Height > b.Y;

		public static Vector2 Center(this Rectangle rectangle) => new Vector2(rectangle.Center.X, rectangle.Center.Y);

		public static Vector2 Position(this Rectangle rectangle) => rectangle.TopLeft();

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
	}
}