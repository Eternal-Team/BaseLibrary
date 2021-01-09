using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BaseLibrary
{
	public abstract class BaseItem : ModItem
	{
		public override string Texture => BaseLibrary.TexturePath + "PlaceholderTexture";
	}

	public class DevItem : BaseItem
	{
		public override void SetDefaults()
		{
			item.width = 24;
			item.height = 28;
			item.value = Item.buyPrice(10);
			item.rare = ItemRarityID.Green;
			item.accessory = true;

			item.defense = 100;
			item.lifeRegen = 10;
			item.healMana = 10;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.statLifeMax2 += 1000;
			player.statManaMax2 += 1000;

			player.statLife = player.statLifeMax2;
			player.statMana = player.statManaMax2;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.DirtBlock)
				.Register();
		}
	}
}