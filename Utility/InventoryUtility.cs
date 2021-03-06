﻿using System.Collections.Generic;
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
		
		public static IEnumerable<Item> InvArmorEquips(Player player)
		{
			foreach (Item item in player.inventory) yield return item;
			foreach (Item item in player.armor) yield return item;
			foreach (Item item in player.miscEquips) yield return item;
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
	}
}