using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.UI.Chat;

namespace BaseLibrary
{
	public interface IItemHandler
	{
		bool InsertItem(int slot, ref Item item, bool user);

		bool ExtractItem(int slot, out Item item, bool user);

		ItemHandler GetItemHandler();
	}

	public class ItemHandler
	{
		public delegate void OnContentsChangedCallback(int slot, bool user);

		public delegate int GetSlotSizeCallback(int slot);

		public delegate bool IsItemValidCallback(int slot, Item item);

		public Item[] Items { get; private set; }

		public int Slots => Items.Length;

		public OnContentsChangedCallback OnContentsChanged;
		public GetSlotSizeCallback GetSlotSize;
		public IsItemValidCallback IsItemValid;

		public ItemHandler(int size = 1) {
			Items = new Item[size];

			for (int i = 0; i < size; i++) Items[i] = new Item();
		}

		private ItemHandler(Item[] items) {
			Items = items;
		}

		public ItemHandler Clone() => new ItemHandler(Items.Select(x => x.Clone()).ToArray()) {
			IsItemValid = (IsItemValidCallback)IsItemValid?.Clone(),
			GetSlotSize = (GetSlotSizeCallback)GetSlotSize?.Clone(),
			OnContentsChanged = (OnContentsChangedCallback)OnContentsChanged?.Clone()
		};

		public void SetSize(int size) {
			Items = new Item[size];

			for (int i = 0; i < size; i++) Items[i] = new Item();
		}

		public void SetItemInSlot(int slot, Item stack, bool user = false) {
			ValidateSlotIndex(slot);
			Items[slot] = stack;
			OnContentsChanged?.Invoke(slot, user);
		}

		public ref Item GetItemInSlot(int slot) {
			ValidateSlotIndex(slot);
			return ref Items[slot];
		}

		public static bool CanItemsStack(Item a, Item b) {
			if (!a.IsTheSameAs(b)) return false;

			// if (a.modItem != null && b.modItem != null) return a.modItem.CanStack(b.modItem);
			return true;
		}

		public static Item CloneItemWithSize(Item itemStack, int size) {
			if (size == 0) return new Item();
			Item copy = itemStack.Clone();
			copy.stack = size;
			return copy;
		}

		/// <summary>
		/// </summary>
		/// <param name="slot"></param>
		/// <param name="item"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		public bool InsertItem(int slot, ref Item item, bool user = false) {
			if (item == null || item.IsAir) return false;

			ValidateSlotIndex(slot);

			if (!IsItemValid?.Invoke(slot, item) ?? false) return false;

			Item existing = Items[slot];
			if (!existing.IsAir && !CanItemsStack(item, existing)) return false;

			int slotSize = GetItemLimit(slot) ?? item.maxStack;
			int toInsert = MathUtility.Min(slotSize, slotSize - existing.stack, item.stack);
			if (toInsert <= 0) return false;

			bool reachedLimit = item.stack > toInsert;

			if (existing.IsAir) Items[slot] = reachedLimit ? CloneItemWithSize(item, toInsert) : item;
			else item.stack += toInsert;

			OnContentsChanged?.Invoke(slot, user);

			item = reachedLimit ? CloneItemWithSize(item, item.stack - toInsert) : new Item();
			return true;
		}

		public void InsertItem(ref Item item, bool user = false) {
			for (int i = 0; i < Slots; i++) {
				Item other = Items[i];
				if (CanItemsStack(item, other) && other.stack < other.maxStack) {
					InsertItem(i, ref item, user);
					if (item.IsAir || !item.active) return;
				}
			}

			for (int i = 0; i < Slots; i++) {
				InsertItem(i, ref item, user);
				if (item.IsAir) return;
			}
		}

		/// <summary>
		///     Extracts an item from the handler
		/// </summary>
		/// <param name="slot">The slot.</param>
		/// <param name="item">The item. Returns null if unsuccessful</param>
		/// <param name="amount">The amount.</param>
		/// <param name="user">Pass true if interaction was caused by user</param>
		/// <returns>Returns true if successful, otherwise returns false</returns>
		public bool ExtractItem(int slot, out Item item, int? amount = null, bool user = false) {
			item = null;

			if (amount == null || amount <= 0) return false;

			ValidateSlotIndex(slot);

			Item existing = Items[slot];
			if (existing.IsAir) return false;

			int toExtract = MathUtility.Min(amount.Value, existing.maxStack, existing.stack);

			if (existing.stack <= toExtract) {
				item = existing;
				Items[slot] = new Item();

				OnContentsChanged?.Invoke(slot, user);

				return true;
			}

			item = CloneItemWithSize(existing, toExtract);
			Items[slot] = CloneItemWithSize(existing, existing.stack - toExtract);

			OnContentsChanged?.Invoke(slot, user);

			return true;
		}

		public int? GetItemLimit(int slot) {
			int? limit = GetSlotSize?.Invoke(slot);
			return limit;
		}

		protected void ValidateSlotIndex(int slot) {
			if (slot < 0 || slot >= Slots) throw new Exception($"Slot {slot} not in valid range - [0, {Slots})");
		}

		public bool Grow(int slot, int quantity, bool user = false) {
			ref Item item = ref GetItemInSlot(slot);
			int limit = GetItemLimit(slot) ?? item.maxStack;
			if (item.IsAir || quantity <= 0 || item.stack + quantity > limit) return false;

			item.stack += quantity;
			OnContentsChanged?.Invoke(slot, user);

			return true;
		}

		public bool Shrink(int slot, int quantity, bool user = false) {
			ref Item item = ref GetItemInSlot(slot);

			if (item.IsAir || quantity <= 0 || item.stack - quantity < 0) return false;

			item.stack -= quantity;
			if (item.stack <= 0) item.TurnToAir();
			OnContentsChanged?.Invoke(slot, user);

			return true;
		}

		public bool Contains(int type) => Items.Any(item => !item.IsAir && item.type == type);

		public bool Contains(Item item) => Items.Any(item.IsTheSameAs);

		public long CountCoins() {
			long num = 0L;
			for (int i = 0; i < Slots; i++)
			{
				Item item = Items[i];
				switch (item.type)
				{
					case 71:
						num = item.stack;
						break;
					case 72:
						num = item.stack * 100;
						break;
					case 73:
						num = item.stack * 10000;
						break;
					case 74:
						num = item.stack * 1000000;
						break;
					default:
						num = 0;
						break;
				}
			}

			return num;
		}

		#region IO
		public TagCompound Save() => new TagCompound {
			["Items"] = Items.Select(ItemIO.Save).ToList(),
		};

		public void Load(TagCompound tag) {
			IList<TagCompound> items = tag.GetList<TagCompound>("Items");
			Items = items.Select(ItemIO.Load).ToArray();
		}

		// public void Write(BinaryWriter writer) {
		// 	writer.Write(Slots);
		//
		// 	for (int i = 0; i < Slots; i++) writer.Write(Items[i], true, true);
		// }
		//
		// public void Read(BinaryReader reader) {
		// 	int size = reader.ReadInt32();
		// 	SetSize(size);
		//
		// 	for (int i = 0; i < Slots; i++) Items[i] = reader.Receive(true, true);
		// }
		#endregion
	}

	public static class ItemHandlerUtility
	{
		/// <summary>
		///     Quick stacks player's items into the ItemHandler
		/// </summary>
		public static void QuickStack(this Player player, ItemHandler handler) {
			for (int i = 49; i >= 10; i--) {
				ref Item inventory = ref player.inventory[i];

				if (!inventory.IsAir && handler.Contains(inventory.type)) handler.InsertItem(ref inventory);
			}

			SoundEngine.PlaySound(SoundID.Grab);
		}

		/// <summary>
		///     Loots ItemHandler's items into player's inventory
		/// </summary>
		public static void LootAll(this Player player, ItemHandler handler) {
			for (int i = 0; i < handler.Slots; i++) {
				ref Item item = ref handler.GetItemInSlot(i);
				if (!item.IsAir) {
					item.position = player.Center;

					item = Combine(item.Split().Select(split => player.GetItem(player.whoAmI, split, GetItemSettings.LootAllSettings)));

					handler.OnContentsChanged?.Invoke(i, true);
				}
			}
		}

		/// <summary>
		///     Loots ItemHandler's items into player's inventory
		/// </summary>
		public static void Loot(this Player player, ItemHandler handler, int slot) {
			ref Item item = ref handler.GetItemInSlot(slot);
			if (!item.IsAir) {
				Item n = new Item(item.type);

				int count = Math.Min(item.stack, item.maxStack);
				n.stack = count;
				n.position = player.Center;
				player.GetItem(player.whoAmI, n, GetItemSettings.LootAllSettings);

				item.stack -= count;
				if (item.stack <= 0) item.TurnToAir();

				handler.OnContentsChanged?.Invoke(slot, true);
			}
		}

		/// <summary>
		///     Deposits player's items into the ItemHandler
		/// </summary>
		public static void DepositAll(this Player player, ItemHandler handler) {
			for (int i = 53; i >= 10; i--) {
				ref Item item = ref player.inventory[i];
				if (item.IsAir || item.favorited) continue;
				handler.InsertItem(ref item);
			}

			SoundEngine.PlaySound(SoundID.Grab);
		}

		public static Item Combine(IEnumerable<Item> items) {
			List<Item> list = items.ToList();

			Item ret = new Item();

			foreach (Item item in list) {
				if (ret.IsAir && !item.IsAir) {
					ret = item.Clone();
					ret.stack = 0;
				}

				if (ret.type == item.type) ret.stack += item.stack;
			}

			return ret;
		}

		public static IEnumerable<Item> Split(this Item item) {
			while (item.stack > 0) {
				Item clone = item.Clone();
				int count = Math.Min(item.stack, item.maxStack);
				clone.stack = count;
				yield return clone;

				item.stack -= count;
				if (item.stack <= 0) {
					item.TurnToAir();
					yield break;
				}
			}
		}
	}
}