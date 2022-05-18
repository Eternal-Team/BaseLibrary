using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BaseLibrary;

public abstract class BaseItem : ModItem
{
	public override string Texture => BaseLibrary.TexturePath + "PlaceholderTexture";
}

public class DevItem : BaseItem
{
	public override void SetDefaults()
	{
		Item.width = 24;
		Item.height = 28;
		Item.value = Item.buyPrice(10);
		Item.rare = ItemRarityID.Green;
		Item.accessory = true;

		Item.defense = 100;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.statLifeMax2 += 1000;
		player.statManaMax2 += 1000;

		player.statLife = player.statLifeMax2;
		player.statMana = player.statManaMax2;

		player.pickSpeed = 0.01f;
			
		player.noKnockback = true;
	}
}