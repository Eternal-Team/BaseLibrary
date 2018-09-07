using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace BaseLibrary.Utility
{
	public static partial class Utility
	{
		public static readonly int[] CoinTypes =
		{
			ItemID.CopperCoin,
			ItemID.SilverCoin,
			ItemID.GoldCoin,
			ItemID.PlatinumCoin
		};

		public static bool IsPlayerInChest(int chestIndex) => Main.player.Any(x => x.chest == chestIndex);
		public static bool IsPlayerInChest(this Chest chest) => Main.player.Any(x => x.chest == Main.chest.ToList().FindIndex(y => y.x == chest.x && y.y == chest.y));

		public static Item TakeItemFromNearbyChest(Item item, Vector2 position)
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

		public static bool HasItem(this Player player, int type, int stack = -1)
		{
			int count = player.inventory.Where(t => type == t.type).Sum(t => t.stack);
			return stack == -1 ? count > 0 : count >= stack;
		}

		public static bool HasItems(this Player player, List<Item> items) => items.All(t => player.HasItem(t.type, t.stack));

		public static bool ConsumeItem(this Player player, int type, int stack = 1)
		{
			if (player.HasItem(type, stack))
			{
				int remaining = stack;

				for (int i = 0; i < player.inventory.Length; i++)
				{
					if (player.inventory[i].type == type)
					{
						int removed = Math.Min(player.inventory[i].stack, remaining);
						player.inventory[i].stack = remaining -= removed;
						if (player.inventory[i].stack <= 0) player.inventory[i].SetDefaults();
						if (remaining == 0) return true;
					}
				}

				return true;
			}

			return false;
		}

		public static bool ConsumeItems(this Player player, List<Item> items)
		{
			if (player.HasItems(items))
			{
				for (int i = 0; i < items.Count; i++) player.ConsumeItem(items[i].type, items[i].stack);
				return true;
			}

			return false;
		}

		public static void SpawnItems(this Player player, List<Item> items)
		{
			for (int i = 0; i < items.Count; i++) Item.NewItem(player.position, player.Size, items[i].type, items[i].stack, noGrabDelay: true);
		}

		public static bool HasSpace(List<Item> items, Item item) => items.FindAll(x => x.type == item.type).Select(x => x.maxStack - x.stack).Sum(x => x) >= item.stack || items.Any(t => t.IsAir);
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
		}

		public static List<Item> Armor(this Player player) => player.armor.Where((x, i) => i > 0 && i < 3).ToList();
		public static List<Item> Accessory(this Player player) => player.armor.Where((x, i) => i >= 3 && i < 8 + Main.LocalPlayer.extraAccessorySlots).ToList();
		public static List<Item> Ammo(this Player player) => player.inventory.Where((x, i) => i >= 54 && i <= 57).ToList();
		public static bool HasArmor(this Player player, int type) => player.Armor().Any(x => x.type == type);
		public static bool HasAccessory(this Player player, int type) => player.Accessory().Any(x => x.type == type);
		public static Item GetHeldItem(this Player player) => Main.mouseItem.IsAir ? Main.LocalPlayer.HeldItem : Main.mouseItem;
		public static bool IsCoin(this Item item) => CoinTypes.Contains(item.type);
		public static TagCompound Save(this IList<Item> items) => new TagCompound { ["Items"] = items.Select(ItemIO.Save).ToList() };
		public static List<Item> Load(TagCompound tag) => tag["Items"] is List<TagCompound> ? tag.GetList<Item>("Items").ToList() : tag.GetCompound("Items").GetList<Item>("Items").ToList();
		public static void Write(this BinaryWriter writer, IList<Item> items) => TagIO.Write(items.Save(), writer);
		public static List<Item> Read(this BinaryReader reader) => Load(TagIO.Read(reader));
	}
}