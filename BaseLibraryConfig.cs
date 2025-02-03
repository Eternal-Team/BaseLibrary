using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace BaseLibrary;

public class BaseLibraryConfig : ModConfig
{
	public override ConfigScope Mode => ConfigScope.ClientSide;

	[Header("UI")] [DefaultValue(8)] public int WindowSnapDistance;
}