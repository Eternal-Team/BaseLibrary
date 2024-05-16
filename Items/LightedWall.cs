using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BaseLibrary.Items;

public class LightedWallItem : BaseItem
{
	public override string Texture => "Terraria/Images/Item_2681";

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableWall(ModContent.WallType<LightedWall>());
	}
}
	
public class LightedWall : ModWall
{
	public override string Texture=>"Terraria/Images/Wall_155";

	public override void SetStaticDefaults()
	{
		Main.wallHouse[Type] = true;
		AddMapEntry(Color.White);
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		r = 1f;
		g = 1f;
		b = 1f;
	}
}