// TODO: move to container lib
// using System.Collections.Generic;
// using BaseLibrary.UI;
// using Terraria;
// using Terraria.Audio;
// using Terraria.ID;
// using Terraria.ModLoader;
// using Terraria.UI;
//
// namespace BaseLibrary
// {
// 	public class BaseLibraryPlayer : ModPlayer
// 	{
// 		private static List<int> ValidShiftClickSlots = new List<int> { ItemSlot.Context.InventoryItem, ItemSlot.Context.InventoryCoin, ItemSlot.Context.InventoryAmmo };
//
// 		public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
// 		{
// 			ref Item item = ref inventory[slot];
//
// 			if (item.favorited || item.IsAir || !ValidShiftClickSlots.Contains(context)) return false;
//
// 			bool block = false;
// 			foreach (BaseElement element in PanelUI.Instance.Children)
// 			{
// 				if (element.Display == Display.Visible && element is	IItemStorageUI ui)
// 				{
// 					ItemStorage storage = ui.GetItemStorage();
// 					if (storage.InsertItem(Player, ref item)) block = true;
// 				}
// 			}
//
// 			if (block)
// 			{
// 				SoundEngine.PlaySound(SoundID.Grab);
// 				return true;
// 			}
//
// 			return false;
// 		}
// 	}
// }