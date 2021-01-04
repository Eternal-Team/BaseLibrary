using System;
using BaseLibrary.UI;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Input;
using Terraria.ModLoader.IO;

namespace BaseLibrary
{
	public class BaseLibrary : Mod
	{
		public const string PlaceholderTexture = "BaseLibrary/Assets/PlaceholderTexture";

		public override void Load()
		{
			Hooking.Initialize();

			if (!Main.dedServ)
			{
				Input.Layers.PushLayer(new UILayer());
			}
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