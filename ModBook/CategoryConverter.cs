using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Terraria.ModLoader;

namespace BaseLibrary.ModBook
{
	public class CategoryConverter : JsonConverter
	{
		private Mod mod;

		public CategoryConverter(Mod mod) => this.mod = mod;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jsonCategory = JObject.Load(reader);

			Category category = new Category
			{
				Mod = mod,
				Name = jsonCategory["Name"].ToObject<string>(),
				Texture = jsonCategory["Texture"].ToObject<string>()
			};

			if (!ModBookLoader.textureCache.ContainsKey(category.Texture)) ModBookLoader.textureCache.Add(category.Texture, ModLoader.GetTexture(category.Texture));

			return category;
		}

		public override bool CanConvert(Type objectType) => true;
	}

	public class ModBookContractResolver : DefaultContractResolver
	{
		private Mod mod;

		public ModBookContractResolver(Mod mod) => this.mod = mod;

		protected override JsonContract CreateContract(Type objectType)
		{
			JsonContract contract = base.CreateContract(objectType);

			if (objectType == typeof(Category)) contract.Converter = new CategoryConverter(mod);

			return contract;
		}
	}
}