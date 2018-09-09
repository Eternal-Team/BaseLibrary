using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Terraria.ModLoader;

namespace BaseLibrary.ModBook
{
	internal class CategoryConverter : JsonConverter
	{
		private readonly Mod mod;
		public CategoryConverter(Mod mod) => this.mod = mod;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();

		public override bool CanConvert(Type objectType) => objectType == typeof(List<Category>);

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.StartArray) return null;

			List<Category> categories = (List<Category>)existingValue;

			foreach (Category category in JArray.Load(reader).Select(x => x.ToObject<Category>()))
			{
				category.Mod = mod;
				if (!categories.Contains(category))
				{
					if (!ModBookLoader.textureCache.ContainsKey(category.Texture)) ModBookLoader.textureCache.Add(category.Texture, ModLoader.GetTexture(category.Texture));
					categories.Add(category);
				}
			}

			return categories;
		}
	}
}