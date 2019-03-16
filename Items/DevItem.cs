using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BaseLibrary.Items
{
	public class ThePreciousOne : BaseItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Precious One");
			Tooltip.SetDefault("Dev-only item, use at your own risk");
		}

		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 20;
			item.accessory = true;
			item.expert = true;
			item.rare = 12;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			if (typeof(ModLoader).GetValue<string>("SteamID64") == "76561198126875987")
			{
				player.AddBuff(BuffID.Spelunker, 1);
				player.statLife = player.statLifeMax2;
				player.statMana = player.statManaMax2;
				player.endurance += 1;
				player.statDefense += 100;
				player.rangedDamage *= 50;
				player.meleeDamage *= 50;
				player.magicDamage *= 50;
				player.minionDamage *= 50;
				player.maxMinions += 49;
				player.wallSpeed *= 100;
			}
			else
			{
				player.AddBuff(BuffID.Electrified, 36000);
				player.AddBuff(BuffID.Venom, 36000);
				player.AddBuff(BuffID.Blackout, 36000);
			}
		}
	}
}