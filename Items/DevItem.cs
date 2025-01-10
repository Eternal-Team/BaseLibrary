using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BaseLibrary.Items;

public class ThePreciousOne : BaseItem
{
	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 20;
		Item.accessory = true;
		Item.expert = true;
		Item.rare = ItemRarityID.Master;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.GetDamage(DamageClass.Generic) += 50f;

		player.statLifeMax2 = 1000;
		player.statManaMax2 = 400;

		player.statLife = player.statLifeMax2;
		player.statMana = player.statManaMax2;

		player.endurance += 1;
		player.maxMinions += 9;
		player.statDefense += 200;

		player.wallSpeed *= 100;
		player.tileSpeed *= 100;
	}
}