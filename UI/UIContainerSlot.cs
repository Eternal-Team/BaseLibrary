using System;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Container;
using Terraria.ModLoader.Input;
using Terraria.ModLoader.Input.Mouse;
using Terraria.UI;
using Terraria.UI.Chat;

namespace BaseLibrary.UI
{
	public class UIContainerSlot : BaseElement
	{
		public struct Options
		{
			public static readonly Options Default = new Options
			{
				ShortStackSize = false,
				GhostItem = null,
				SlotTexture = TextureAssets.InventoryBack.Value,
				FavoritedSlotTexture = TextureAssets.InventoryBack10.Value
			};

			public bool ShortStackSize;
			public Item GhostItem;
			public Texture2D SlotTexture;
			public Texture2D FavoritedSlotTexture;
		}

		private ItemStorage storage;
		private Options options;

		public Item Item
		{
			get => storage[slot];
			// set => storage[slot] = value;
		}

		private int slot;

		public UIContainerSlot(ItemStorage itemStorage, int slot, Options? options = null)
		{
			this.options = options ?? Options.Default;

			Width.Pixels = 44;
			Height.Pixels = 44;

			this.slot = slot;
			storage = itemStorage;
		}

		protected override void MouseClick(MouseButtonEventArgs args)
		{
			if (args.Button != MouseButton.Left) return;

			if (storage.IsItemValid(slot, Main.mouseItem) || Main.mouseItem.IsAir)
			{
				args.Handled = true;

				Item.newAndShiny = false;
				Player player = Main.LocalPlayer;

				if (ItemSlot.ShiftInUse)
				{
					Main.LocalPlayer.Loot(storage, slot);
					return;
				}

				if (Main.mouseItem.IsAir) storage.RemoveItem(Main.LocalPlayer, slot, out Main.mouseItem, Item.maxStack);
				else
				{
					if (Item.IsTheSameAs(Main.mouseItem)) storage.InsertItem(Main.LocalPlayer, slot, ref Main.mouseItem);
					else
					{
						if (Item.stack <= Item.maxStack)
						{
							storage.SwapStacks(Main.LocalPlayer, slot, ref Main.mouseItem);
						}
					}
				}

				if (Item.stack > 0) AchievementsHelper.NotifyItemPickup(player, Item);

				if (Main.mouseItem.type > ItemID.None || Item.type > ItemID.None)
				{
					Recipe.FindRecipes();
					SoundEngine.PlaySound(SoundID.Grab);
				}
			}
		}

		public override int CompareTo(BaseElement other) => other is UIContainerSlot uiSlot ? slot.CompareTo(uiSlot.slot) : 0;

		private void DrawItem(SpriteBatch spriteBatch, Item item, float scale)
		{
			Main.instance.LoadItem(item.type);
			Texture2D itemTexture = TextureAssets.Item[item.type].Value;

			Rectangle rect = Main.itemAnimations[item.type] != null ? Main.itemAnimations[item.type].GetFrame(itemTexture) : itemTexture.Frame();
			Color newColor = Color.White;
			float pulseScale = 1f;
			ItemSlot.GetItemLight(ref newColor, ref pulseScale, item);
			int height = rect.Height;
			int width = rect.Width;
			float drawScale = 1f;

			float availableWidth = InnerDimensions.Width;
			if (width > availableWidth || height > availableWidth)
			{
				if (width > height) drawScale = availableWidth / width;
				else drawScale = availableWidth / height;
			}

			drawScale *= scale;
			Vector2 position = Dimensions.TopLeft() + Dimensions.Size() * 0.5f;
			Vector2 origin = rect.Size() * 0.5f;

			if (ItemLoader.PreDrawInInventory(item, spriteBatch, position - rect.Size() * 0.5f * drawScale, rect, item.GetAlpha(newColor), item.GetColor(Color.White), origin, drawScale * pulseScale))
			{
				spriteBatch.Draw(itemTexture, position, rect, item.GetAlpha(newColor), 0f, origin, drawScale * pulseScale, SpriteEffects.None, 0f);
				if (item.color != Color.Transparent) spriteBatch.Draw(itemTexture, position, rect, item.GetColor(Color.White), 0f, origin, drawScale * pulseScale, SpriteEffects.None, 0f);
			}

			ItemLoader.PostDrawInInventory(item, spriteBatch, position - rect.Size() * 0.5f * drawScale, rect, item.GetAlpha(newColor), item.GetColor(Color.White), origin, drawScale * pulseScale);
			if (ItemID.Sets.TrapSigned[item.type]) spriteBatch.Draw(TextureAssets.Wire.Value, position + new Vector2(40f, 40f) * scale, new Rectangle(4, 58, 8, 8), Color.White, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);
			if (item.stack > 1)
			{
				string text = !options.ShortStackSize || item.stack < 1000 ? item.stack.ToString() : TextUtility.ToSI(item.stack, "N1");
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, InnerDimensions.TopLeft() + new Vector2(8, InnerDimensions.Height - FontAssets.MouseText.Value.MeasureString(text).Y * scale), Color.White, 0f, Vector2.Zero, new Vector2(0.85f), -1f, scale);
			}

			if (IsMouseHovering)
			{
				Main.LocalPlayer.cursorItemIconEnabled = false;
				Main.ItemIconCacheUpdate(0);
				Main.HoverItem = item.Clone();
				Main.hoverItemName = Main.HoverItem.Name;

				// if (ItemSlot.ShiftInUse) BaseLibrary.Hooking.SetCursor("Terraria/UI/Cursor_7");
			}
		}

		protected override void Draw(SpriteBatch spriteBatch)
		{
			var texture = !Item.IsAir && Item.favorited ? options.FavoritedSlotTexture : options.SlotTexture;
			DrawingUtility.DrawSlot(spriteBatch, Dimensions, texture, Color.White);

			float scale = Math.Min(InnerDimensions.Width / (float)texture.Width, InnerDimensions.Height / (float)texture.Height);

			if (!Item.IsAir) DrawItem(spriteBatch, Item, scale);
			// else if (options.GhostItem != null && !options.GhostItem.IsAir) spriteBatch.DrawWithEffect(BaseLibrary.BaseLibrary.DesaturateShader, () => DrawItem(spriteBatch, options.GhostItem, scale));
		}

		protected override void MouseHeld(MouseButtonEventArgs args)
		{
			if (args.Button != MouseButton.Right) return;

			if (storage.IsItemValid(slot, Main.mouseItem) || Main.mouseItem.IsAir)
			{
				args.Handled = true;

				Player player = Main.LocalPlayer;
				Item.newAndShiny = false;

				if (Main.stackSplit <= 1)
				{
					if ((Main.mouseItem.IsTheSameAs(Item) || Main.mouseItem.type == ItemID.None) && (Main.mouseItem.stack < Main.mouseItem.maxStack || Main.mouseItem.type == ItemID.None))
					{
						if (Main.mouseItem.type == ItemID.None)
						{
							Main.mouseItem = Item.Clone();
							Main.mouseItem.stack = 0;
							if (Item.favorited && Item.maxStack == 1) Main.mouseItem.favorited = true;
							Main.mouseItem.favorited = false;
						}

						Main.mouseItem.stack++;
						storage.ModifyStackSize(Main.LocalPlayer, slot, -1);

						Recipe.FindRecipes();

						SoundEngine.PlaySound(12);
						ItemSlot.RefreshStackSplitCooldown();
					}
				}
			}
		}

		protected override void MouseScroll(MouseScrollEventArgs args)
		{
			if (!Main.keyState.IsKeyDown(Keys.LeftAlt)) return;

			if (args.OffsetY > 0)
			{
				if (Main.mouseItem.type == Item.type && Main.mouseItem.stack < Main.mouseItem.maxStack)
				{
					Main.mouseItem.stack++;
					storage.ModifyStackSize(Main.LocalPlayer, slot, -1);
				}
				else if (Main.mouseItem.IsAir)
				{
					Main.mouseItem = Item.Clone();
					Main.mouseItem.stack = 1;
					storage.ModifyStackSize(Main.LocalPlayer, slot, -1);
				}
			}
			else if (args.OffsetY < 0)
			{
				if (Item.type == Main.mouseItem.type && storage.ModifyStackSize(Main.LocalPlayer, slot, 1))
				{
					if (--Main.mouseItem.stack <= 0) Main.mouseItem.TurnToAir();
				}
				else if (Item.IsAir)
				{
					Item cloned = Main.mouseItem.Clone();
					cloned.stack = 1;
					storage.InsertItem(Main.LocalPlayer, slot, ref cloned);
					if (--Main.mouseItem.stack <= 0) Main.mouseItem.TurnToAir();
				}
			}
		}
	}
}