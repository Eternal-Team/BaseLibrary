using System.Collections.Generic;
using Terraria;

namespace BaseLibrary.Utility
{
	public static class InventoryUtility
	{
		public static IEnumerable<Item> InvArmor(Player player)
		{
			foreach (Item item in player.inventory) yield return item;
			foreach (Item item in player.armor) yield return item;
		}
	}
}