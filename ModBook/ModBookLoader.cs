using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Terraria.ModLoader;

namespace BaseLibrary.ModBook
{
	public static class ModBookLoader
	{
		public static Dictionary<string, ModBook> modBooks = new Dictionary<string, ModBook>();
		public static Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();

		public static ModBook GetModBook(string name) => modBooks.TryGetValue(name, out ModBook item) ? item : null;

		public static T GetModBook<T>() where T : ModBook => (T)GetModBook(typeof(T).Name);

		public static void Load()
		{
			foreach (Mod mod in ModLoader.LoadedMods.Where(mod => mod != null && mod.Code != null))
			{
				foreach (Type type in mod.Code.GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ModBook)))) AutoloadModBook(type, mod);
			}
		}

		public static void Unload()
		{
			modBooks.Clear();
			textureCache.Clear();
		}

		private static void AutoloadModBook(Type type, Mod mod)
		{
			ModBook modBook = (ModBook)Activator.CreateInstance(type);
			string name = type.Name;

			modBook.Mod = mod;
			modBook.Name = name;
			modBook.DisplayName = mod.CreateTranslation(mod.Name + "." + name);
			modBooks[name] = modBook;

			modBook.Initialize();

			string json = Encoding.Default.GetString(ModLoader.GetFileBytes($"{mod.Name}/{name}.json"));
			JsonConvert.PopulateObject(json, modBook, new JsonSerializerSettings { Converters = { new CategoryConverter(mod) } });
		}
	}
}