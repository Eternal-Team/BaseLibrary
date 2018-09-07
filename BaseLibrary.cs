using BaseLibrary.Utility;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.UI;
using static BaseLibrary.Utility.Utility;

namespace BaseLibrary
{
	public class BaseLibrary : Mod
	{
		private LegacyGameInterfaceLayer MouseInterface;

		public override void Load()
		{
			this.LoadTextures();

			MouseInterface = new LegacyGameInterfaceLayer("TheOneLibrary: MouseText", DrawMouseText, InterfaceScaleType.UI);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (MouseTextIndex != -1) layers.Insert(MouseTextIndex + 1, MouseInterface);
		}
	}
}