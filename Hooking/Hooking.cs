using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using On.Terraria;
using On.Terraria.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace BaseLibrary
{
	public static partial class Hooking
	{
		public static bool BlockScrolling;
		internal static List<IHasUI> ClosedUICache = new List<IHasUI>();

		internal static void Load()
		{
			ItemSlot.LeftClick_ItemArray_int_int += ItemSlot_LeftClick;
			Player.DropSelectedItem += Player_DropSelectedItem;
			UIElement.GetElementAt += UIElement_GetElementAt;

			UserInterface.Update += UserInterface_Update;

			IL.Terraria.Main.DrawInterface_36_Cursor += Main_DrawInterface_36_Cursor;
			On.Terraria.GameInput.PlayerInput.KeyboardInput += PlayerInput_KeyboardInput;

			Main.DoUpdate_Enter_ToggleChat += Main_DoUpdate_Enter_ToggleChat;

			Player.ToggleInv += Player_ToggleInv;

			IL.Terraria.Player.Update += Player_Update;

			HookEndpointManager.Modify(typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.UI.DownloadManager.UILoadModsProgress").GetMethod("OnDeactivate", Utility.defaultFlags), new Action<ILContext>(ShowIntroMessage));
		}

		private static void Player_Update(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);

			if (cursor.TryGotoNext(i => i.MatchLdloc(28), i => i.MatchBrtrue(out _)))
			{
				cursor.Index += 2;

				cursor.RemoveRange(113);

				cursor.Emit(OpCodes.Ldarg, 0);
				cursor.EmitDelegate<Action<Terraria.Player>>(player =>
				{
					if (BlockScrolling)
					{
						BlockScrolling = false;
						return;
					}

					if (!Terraria.Main.playerInventory) player.InvokeMethod<object>("HandleHotbar");
					else
					{
						int delta = Terraria.GameInput.PlayerInput.ScrollWheelDelta / 120;
						if (Terraria.Main.recBigList)
						{
							const int height = 42;
							const int y = 340;
							const int x = 310;
							Terraria.GameInput.PlayerInput.SetZoom_UI();
							int mulX = (Terraria.Main.screenWidth - x - 280) / height;
							int mulY = (Terraria.Main.screenHeight - y - 20) / height;
							if (new Rectangle(x, y, mulX * height, mulY * height).Contains(Terraria.Utils.ToPoint(Terraria.Main.MouseScreen)))
							{
								delta *= -1;
								int sign = Math.Sign(delta);
								while (delta != 0)
								{
									if (delta < 0)
									{
										Terraria.Main.recStart -= mulX;
										if (Terraria.Main.recStart < 0) Terraria.Main.recStart = 0;
									}
									else
									{
										Terraria.Main.recStart += mulX;
										if (Terraria.Main.recStart > Terraria.Main.numAvailableRecipes - mulX) Terraria.Main.recStart = Terraria.Main.numAvailableRecipes - mulX;
									}

									delta -= sign;
								}
							}

							Terraria.GameInput.PlayerInput.SetZoom_World();
						}

						Terraria.Main.focusRecipe += delta;
						if (Terraria.Main.focusRecipe > Terraria.Main.numAvailableRecipes - 1) Terraria.Main.focusRecipe = Terraria.Main.numAvailableRecipes - 1;
						if (Terraria.Main.focusRecipe < 0) Terraria.Main.focusRecipe = 0;
					}
				});
			}
		}

		private static void Player_ToggleInv(Player.orig_ToggleInv orig, Terraria.Player self)
		{
			if (!Utility.Input.KeyboardHandler.Enabled)
			{
				orig(self);

				if (!Terraria.Main.playerInventory)
				{
					List<BaseUIPanel> panels = BaseLibrary.PanelGUI.Elements.Cast<BaseUIPanel>().ToList();
					foreach (BaseUIPanel ui in panels)
					{
						ClosedUICache.Add(ui.Container);
						BaseLibrary.PanelGUI.UI.CloseUI(ui.Container);
					}
				}
				else
				{
					foreach (IHasUI ui in ClosedUICache) BaseLibrary.PanelGUI.UI.OpenUI(ui);

					ClosedUICache.Clear();
				}
			}
		}

		private static void Main_DoUpdate_Enter_ToggleChat(Main.orig_DoUpdate_Enter_ToggleChat orig)
		{
			if (!Utility.Input.KeyboardHandler.Enabled) orig();
		}

		private static void PlayerInput_KeyboardInput(On.Terraria.GameInput.PlayerInput.orig_KeyboardInput orig)
		{
			if (!Utility.Input.KeyboardHandler.Enabled) orig();
			else
			{
				foreach (string key in Terraria.GameInput.PlayerInput.MouseKeys)
				{
					PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard].Processkey(PlayerInput.Triggers.Current, key);
				}
			}
		}

		private static void UserInterface_Update(UserInterface.orig_Update orig, Terraria.UI.UserInterface self, GameTime time)
		{
			Utility.Input.Update(time);

			orig(self, time);
		}

		private static void ItemSlot_LeftClick(ItemSlot.orig_LeftClick_ItemArray_int_int orig, Terraria.Item[] inv, int context, int slot)
		{
			if (Terraria.Main.mouseItem.modItem is IHasUI mouse) BaseLibrary.PanelGUI.UI.CloseUI(mouse);
			if (inv[slot].modItem is IHasUI hasUI) BaseLibrary.PanelGUI.UI.CloseUI(hasUI);

			orig(inv, context, slot);
		}

		private static void Player_DropSelectedItem(Player.orig_DropSelectedItem orig, Terraria.Player self)
		{
			if (self.HeldItem.modItem is IHasUI hasUI) BaseLibrary.PanelGUI.UI.CloseUI(hasUI);

			orig(self);
		}
	}
}