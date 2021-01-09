using System;
using BaseLibrary.UI;
using BaseLibrary.Utility;
using ReLogic.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Input;
using Terraria.ModLoader.IO;

namespace BaseLibrary
{
	public class BaseLibrary : Mod
	{
		public const string AssetPath = "BaseLibrary/Assets/";
		public const string TexturePath = AssetPath + "Textures/";
		public const string PlaceholderTexture = TexturePath + "PlaceholderTexture";

		public override void Load()
		{
			Hooking.Initialize();

			if (!Main.dedServ)
			{
				UITextInput.Font = GetFont("Assets/Fonts/Mouse_Text.xnb").Value;
				typeof(DynamicSpriteFont).SetValue("_characterSpacing", 1f, UITextInput.Font);

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