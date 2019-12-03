using Terraria;
using Terraria.ModLoader;

namespace BaseLibrary.Items
{
	public class LightedWall : BaseItem
	{
		public override string Texture => "Terraria/Item_2681";

		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Emits light.");
		}

		public override void SetDefaults()
		{
			item.width = 12;
			item.height = 12;
			item.maxStack = 999;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 7;
			item.useStyle = 1;
			item.consumable = true;
			item.createWall = ModContent.WallType<Walls.LightedWall>();
		}
	}
}