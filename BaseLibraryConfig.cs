using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace BaseLibrary
{
	public class BaseLibraryConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		[DefaultValue(44), Label("Slot size")]
		public int SlotSize;

		[DefaultValue(4), Label("Slot margin")]
		public int SlotMargin;
	}
}