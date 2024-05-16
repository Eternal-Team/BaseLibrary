using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BaseLibrary.Items;

public class LightedBlackWallItem : BaseItem
{
	public override string Texture => "Terraria/Images/Item_2681";

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableWall(ModContent.WallType<LightedBlackWall>());
	}
}

public class LightedBlackWall : ModWall
{
	public override string Texture => "Terraria/Images/Wall_155";

	public override void SetStaticDefaults()
	{
		Main.wallHouse[Type] = true;
		AddMapEntry(new Color(150, 150, 150));
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		r = 1f;
		g = 1f;
		b = 1f;
	}
}