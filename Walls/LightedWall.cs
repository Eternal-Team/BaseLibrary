using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BaseLibrary.Walls
{
	public class LightedWall : ModWall
	{
		public override bool Autoload(ref string name, ref string texture)
		{
			texture = "Terraria/Wall_155";
			return base.Autoload(ref name, ref texture);
		}

		public override void SetDefaults()
		{
			Main.wallHouse[Type] = true;
			drop = mod.ItemType<Items.LightedWall>();
			AddMapEntry(Color.White);
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 1f;
			g = 1f;
			b = 1f;
		}
	}
}