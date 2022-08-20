using System.Collections.Generic;
using Terraria;

namespace BaseLibrary.Utility;

public static class InventoryUtility
{
	public static IEnumerable<Item> InvArmor(Player player)
	{
		foreach (Item item in player.inventory) yield return item;
		foreach (Item item in player.armor) yield return item;
	}

	public static IEnumerable<Item> InvArmorEquips(Player player)
	{
		foreach (Item item in player.inventory) yield return item;
		foreach (Item item in player.armor) yield return item;
		foreach (Item item in player.miscEquips) yield return item;
	}

	public static bool IsWearing<T>(Player player)
	{
		return !player.armor[0].IsAir && player.armor[0].ModItem is T || !player.armor[1].IsAir && player.armor[1].ModItem is T || !player.armor[2].IsAir && player.armor[2].ModItem is T;
	}

	public static bool HasAccessory(this Player player, int type)
	{
		for (int i = 3; i < 8; i++)
		{
			Item item = player.armor[i];
			if (!item.IsAir && item.type == type) return true;
		}

		return false;
	}
	
	public static IEnumerable<T> OfModItemType<T>(this IEnumerable<Item> source)
	{
		foreach (Item obj in source)
		{
			if (!obj.IsAir && obj.ModItem is T result)
			{
				yield return result;
			}
		}
	}
}