using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.Utilities;

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

		public static int NewItem(int X, int Y, int width, int height, Item item, bool noBroadcast = false, bool noGrabDelay = false, bool reverseLookup = false)
		{
			if (WorldGen.gen) return 0;
			if (Main.rand == null) Main.rand = new UnifiedRandom();

			int num = 400;
			int type = item.type;
			int stack = item.stack;

			Main.item[400] = new Item();
			if (NPCLoader.blockLoot.Contains(type)) return num;
			if (Main.halloween)
			{
				if (type == 58) type = 1734;
				if (type == 184) type = 1735;
			}

			if (Main.xMas)
			{
				if (type == 58) type = 1867;
				if (type == 184) type = 1868;
			}

			if (Item.itemCaches[type] != -1)
			{
				Item.itemCaches[type] += stack;
				return 400;
			}

			if (Main.netMode != 1)
			{
				if (reverseLookup)
				{
					for (int i = 399; i >= 0; i--)
					{
						if (!Main.item[i].active && Main.itemLockoutTime[i] == 0)
						{
							num = i;
							break;
						}
					}
				}
				else
				{
					for (int j = 0; j < 400; j++)
					{
						if (!Main.item[j].active && Main.itemLockoutTime[j] == 0)
						{
							num = j;
							break;
						}
					}
				}
			}

			if (num == 400 && Main.netMode != 1)
			{
				int num2 = 0;
				for (int k = 0; k < 400; k++)
				{
					if (Main.item[k].spawnTime - Main.itemLockoutTime[k] > num2)
					{
						num2 = Main.item[k].spawnTime - Main.itemLockoutTime[k];
						num = k;
					}
				}
			}

			Main.itemLockoutTime[num] = 0;

			ref Item newItem = ref Main.item[num];

			newItem = item.Clone();

			newItem.position.X = X + width / 2 - newItem.width / 2;
			newItem.position.Y = Y + height / 2 - newItem.height / 2;
			newItem.wet = Collision.WetCollision(newItem.position, newItem.width, newItem.height);
			newItem.velocity.X = Main.rand.Next(-30, 31) * 0.1f;
			newItem.velocity.Y = Main.rand.Next(-40, -15) * 0.1f;
			if (type == 859) newItem.velocity *= 0f;

			if (type == 520 || type == 521 || newItem.type >= 0 && ItemID.Sets.NebulaPickup[newItem.type])
			{
				newItem.velocity.X = Main.rand.Next(-30, 31) * 0.1f;
				newItem.velocity.Y = Main.rand.Next(-30, 31) * 0.1f;
			}

			newItem.active = true;
			newItem.spawnTime = 0;
			newItem.stack = stack;
			if (ItemSlot.Options.HighlightNewItems && newItem.type >= 0 && !ItemID.Sets.NeverShiny[newItem.type])
			{
				newItem.newAndShiny = true;
			}

			if (Main.netMode == 2 && !noBroadcast)
			{
				int num3 = 0;
				if (noGrabDelay)
				{
					num3 = 1;
				}

				NetMessage.SendData(21, -1, -1, null, num, num3);
				newItem.FindOwner(num);
			}
			else if (Main.netMode == 0)
			{
				newItem.owner = Main.myPlayer;
			}

			return num;
		}

		/*	public static Item TakeItemFromNearbyChest(Item item, Vector2 position)
		{
			if (Main.netMode == 1) return item;
			for (int i = 0; i < Main.chest.Length; i++)
			{
				bool hasItem = false;
				bool emptySlot = false;
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
									hasItem = true;
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

							else emptySlot = true;
						}

						if (hasItem && emptySlot && item.stack > 0)
						{
							for (int k = 0; k < Main.chest[i].item.Length; k++)
							{
								if (Main.chest[i].item[k].type == 0 || Main.chest[i].item[k].stack == 0)
								{
									Main.chest[i].item[k] = item.Clone();
									item.SetDefaults();
									return item;
								}
							}
						}
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