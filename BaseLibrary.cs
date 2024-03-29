using System;
using BaseLibrary.UI;
using BaseLibrary.Utility;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BaseLibrary;

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
			UITextInput.Font = ModContent.Request<DynamicSpriteFont>(AssetPath + "Fonts/Mouse_Text", AssetRequestMode.ImmediateLoad);
			UITextInput.Font.Value.SetValue("_characterSpacing", 1f);

			Input.Layers.PushLayer(new UILayer());
		}
	}
}

public class GuidSerializer : TagSerializer<Guid, TagCompound>
{
	public override TagCompound Serialize(Guid value) => new()
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