using System;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BaseLibrary
{
	public class BaseLibrary : Mod
	{
		public override void Load()
		{
			Hooking.Initialize();

			TagSerializer.AddSerializer(new GuidSerializer());
		}
	}

	// todo: this should get integrated into tML
	public class GuidSerializer : TagSerializer<Guid, TagCompound>
	{
		public override TagCompound Serialize(Guid value) => new TagCompound
		{
			["Value"] = value.ToString()
		};

		public override Guid Deserialize(TagCompound tag)
		{
			try
			{
				return Guid.Parse(tag.GetString("Value"));
			}
			catch
			{
				return Guid.NewGuid();
			}
		}
	}
}