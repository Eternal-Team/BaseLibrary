using Terraria;
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
		Item.rare = 12;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.GetDamage(DamageClass.Generic) += 50f;
            
		player.statLife = player.statLifeMax2;
		player.statMana = player.statManaMax2;
		player.endurance += 1;
		player.statDefense += 200;
		player.maxMinions += 49;
		player.wallSpeed *= 100;
	}
}