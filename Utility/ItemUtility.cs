using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace BaseLibrary
{
	public static partial class Utility
	{
		private static int[] _coinTypes;

		public static int[] CoinTypes => _coinTypes ?? (_coinTypes = new int[]
		{
			ItemID.CopperCoin,
			ItemID.SilverCoin,
			ItemID.GoldCoin,
			ItemID.PlatinumCoin
		});

		/*	public static Item TakeItemFromNearbyChest(Item item, Vector2 position)
		{
			if (Main.netMode == 1) return item;
			for (int i = 0; i < Main.chest.Length; i++)
			{
				//bool hasItem = false;
				//bool emptySlot = false;
				if (Main.chest[i] != null && !IsPlayerInChest(i) && !Chest.isLocked(Main.chest[i].x, Main.chest[i].y))
				{
					Vector2 value = new Vector2(Main.chest[i].x * 16 + 16, Main.chest[i].y * 16 + 16);
					if ((value - position).Length() < 200f)
					{
						for (int j = 0; j < Main.chest[i].item.Length; j++)
						{
							if (Main.chest[i].item[j].type > 0 && Main.chest[i].item[j].stack > 0)
							{
								if (item.IsTheSameAs(Main.chest[i].item[j]))
								{
									//hasItem = true;
									int num = item.maxStack - item.stack;
									if (num > 0)
									{
										if (num > Main.chest[i].item[j].stack) num = Main.chest[i].item[j].stack;
										item.stack += num;
										Main.chest[i].item[j].stack -= num;
										if (Main.chest[i].item[j].stack <= 0) Main.chest[i].item[j].SetDefaults();
									}
								}
							}

							//else emptySlot = true;
						}

						//if (hasItem && emptySlot && item.stack > 0)
						//{
						//	for (int k = 0; k < Main.chest[i].item.Length; k++)
						//	{
						//		if (Main.chest[i].item[k].type == 0 || Main.chest[i].item[k].stack == 0)
						//		{
						//			Main.chest[i].item[k] = item.Clone();
						//			item.SetDefaults();
						//			return item;
						//		}
						//	}
						//}
					}
				}
			}

			return item;
		}

		public static bool HasSpace(this List<Item> Items, Item item) => Items.Any(t => t.IsAir) || Items.Any(x => x.type == item.type && x.stack < x.maxStack);

		public static IEnumerable<int> InsertItem(List<Item> from, List<Item> to) => from.SelectMany(x => InsertItem(x, to));

		public static IEnumerable<int> InsertItem(Item item, List<Item> to)
		{
			for (int i = 0; i < to.Count; i++)
			{
				if (to[i].type == item.type)
				{
					int count = Math.Min(item.stack, to[i].maxStack - to[i].stack);
					item.stack -= count;
					if (item.stack <= 0) item.TurnToAir();
					to[i].stack += count;
					yield return i;
				}
			}

			while (!item.IsAir && to.Any(x => x.IsAir))
			{
				Item next = to.FirstOrDefault(x => x.IsAir);
				if (next != null)
				{
					next.SetDefaults(item.type);
					int count = Math.Min(item.maxStack, item.stack);
					item.stack -= count;
					next.stack = count;
					if (item.stack <= 0) item.TurnToAir();
					yield return to.IndexOf(next);
				}
			}
		}*/

		public static bool IsCoin(this Item item) => CoinTypes.Contains(item.type);

		public static IEnumerable<T> OfType<T>(this IEnumerable<Item> items) where T : class => items.Where(item => item.modItem is T).Select(item => item.modItem as T);

		#region Player
		public static Item[] Inventory(this Player player)
		{
			Item[] inv = new Item[58];
			Array.Copy(player.inventory, inv, inv.Length);
			return inv;
		}

		public static ref Item GetHeldItem(this Player player)
		{
			if (!Main.mouseItem.IsAir) return ref Main.mouseItem;
			return ref player.inventory[player.selectedItem];
		}
		#endregion
	}
}