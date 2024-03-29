﻿using System;
using BaseLibrary.Utility;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace BaseLibrary.UI;

public struct UIContainerSlotSettings
{
	public static readonly UIContainerSlotSettings Default = new UIContainerSlotSettings
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

[ExtendsFromMod("ContainerLibrary")]
public class UIContainerSlot : BaseElement
{
	protected ItemStorage storage;
	public UIContainerSlotSettings Settings = UIContainerSlotSettings.Default;
	protected int slot;

	public Item Item => storage[slot];

	public UIContainerSlot(ItemStorage itemStorage, int slot)
	{
		Width.Pixels = 44;
		Height.Pixels = 44;

		this.slot = slot;
		storage = itemStorage;
	}

	protected override void MouseDown(MouseButtonEventArgs args)
	{
		if (args.Button != MouseButton.Left) return;

		args.Handled = true;

		Player player = Main.LocalPlayer;

		if (player.itemAnimation != 0 || player.itemTime != 0)
			return;

		if (storage.IsItemValid(slot, Main.mouseItem) || Main.mouseItem.IsAir)
		{
			Item.newAndShiny = false;

			if (ItemSlot.ShiftInUse)
			{
				Main.LocalPlayer.Loot(storage, slot);
				return;
			}

			if (Main.mouseItem.IsAir) storage.RemoveItem(Main.LocalPlayer, slot, out Main.mouseItem, Item.maxStack);
			else
			{
				if (Item.type == Main.mouseItem.type) storage.InsertItem(Main.LocalPlayer, slot, ref Main.mouseItem);
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

	protected void DrawItem(SpriteBatch spriteBatch, Item item, float scale)
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
			string text = !Settings.ShortStackSize || item.stack < 1000 ? item.stack.ToString() : TextUtility.ToSI(item.stack, "N1");
			float texscale = 0.75f;
			// note: i dont think this will scale well with larger slot sizes
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, InnerDimensions.TopLeft() + new Vector2(8, InnerDimensions.Height - FontAssets.MouseText.Value.MeasureString(text).Y * texscale), Color.White, 0f, Vector2.Zero, new Vector2(texscale));
		}

		if (IsMouseHovering)
		{
			Main.LocalPlayer.cursorItemIconEnabled = false;
			Main.ItemIconCacheUpdate(0);
			Main.HoverItem = item.Clone();
			Main.hoverItemName = Main.HoverItem.Name;

			if (ItemSlot.ShiftInUse) CustomCursor.CustomCursor.SetCursor("Terraria/Images/UI/Cursor_7");
		}
	}

	protected override void Draw(SpriteBatch spriteBatch)
	{
		var texture = Item is { IsAir: false, favorited: true } ? Settings.FavoritedSlotTexture : Settings.SlotTexture;
		DrawingUtility.DrawSlot(spriteBatch, Dimensions, texture, Color.White);

		float scale = Math.Min(InnerDimensions.Width / (float)texture.Width, InnerDimensions.Height / (float)texture.Height);

		if (!Item.IsAir) DrawItem(spriteBatch, Item, scale);
		// else if (options.GhostItem != null && !options.GhostItem.IsAir) spriteBatch.DrawWithEffect(BaseLibrary.BaseLibrary.DesaturateShader, () => DrawItem(spriteBatch, options.GhostItem, scale));
	}

	protected override void MouseHeld(MouseButtonEventArgs args)
	{
		if (args.Button != MouseButton.Right) return;

		args.Handled = true;

		if (storage.IsItemValid(slot, Main.mouseItem) || Main.mouseItem.IsAir)
		{
			Item.newAndShiny = false;

			if (Main.stackSplit <= 1)
			{
				if ((Main.mouseItem.type == Item.type || Main.mouseItem.type == ItemID.None) && (Main.mouseItem.stack < Main.mouseItem.maxStack || Main.mouseItem.type == ItemID.None))
				{
					if (Main.mouseItem.type == ItemID.None)
					{
						Main.mouseItem = Item.Clone();
						Main.mouseItem.stack = 0;
						if (Item is { favorited: true, maxStack: 1 }) Main.mouseItem.favorited = true;
						Main.mouseItem.favorited = false;
					}

					Main.mouseItem.stack++;
					storage.ModifyStackSize(Main.LocalPlayer, slot, -1);

					SoundEngine.PlaySound(SoundID.MenuTick);
					ItemSlot.RefreshStackSplitCooldown();
				}
			}
		}
	}

	protected override void MouseScroll(MouseScrollEventArgs args)
	{
		if (!Main.keyState.IsKeyDown(Keys.LeftAlt)) return;

		args.Handled = true;

		// note: this might need some redesigning
		if (args.OffsetY > 0)
		{
			if (Main.mouseItem.type == Item.type && Main.mouseItem.stack < Main.mouseItem.maxStack && storage.ModifyStackSize(Main.LocalPlayer, slot, -1))
			{
				Main.mouseItem.stack++;
			}
			else if (Main.mouseItem.IsAir)
			{
				Item cloned = Item.Clone();
				cloned.stack = 1;
				if (storage.ModifyStackSize(Main.LocalPlayer, slot, -1))
					Main.mouseItem = cloned;
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
				if (storage.InsertItem(Main.LocalPlayer, slot, ref cloned))
				{
					if (--Main.mouseItem.stack <= 0) Main.mouseItem.TurnToAir();
				}
			}
		}
	}
}