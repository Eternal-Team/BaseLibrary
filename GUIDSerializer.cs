using System;
using Terraria.ModLoader.IO;

namespace BaseLibrary
{
	public class GUIDSerializer : TagSerializer<Guid, TagCompound>
	{
		public override TagCompound Serialize(Guid value) => new TagCompound
		{
			["Value"] = value.ToString()
		};

		public override Guid Deserialize(TagCompound tag)
		{
			return Guid.Parse(tag.GetString("Value"));
		}
	}
}