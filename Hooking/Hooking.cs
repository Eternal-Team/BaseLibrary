using BaseLibrary.Input;
using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using On.Terraria.UI;
using System;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace BaseLibrary
{
	public static partial class Hooking
	{
		public static bool BlockScrolling;

		internal static void Load()
		{
			ItemSlot.LeftClick_ItemArray_int_int += ItemSlot_LeftClick;
			On.Terraria.Player.DropSelectedItem += Player_DropSelectedItem;
			UIElement.GetElementAt += UIElement_GetElementAt;

			IL.Terraria.Main.DrawInterface_36_Cursor += Main_DrawInterface_36_Cursor;
			On.Terraria.GameInput.PlayerInput.KeyboardInput += PlayerInput_KeyboardInput;
			On.Terraria.GameInput.PlayerInput.MouseInput += PlayerInput_MouseInput;

			On.Terraria.Main.DoUpdate_Enter_ToggleChat += Main_DoUpdate_Enter_ToggleChat;

			On.Terraria.Player.ToggleInv += Player_ToggleInv;

			IL.Terraria.Player.Update += Player_Update;

			On.Terraria.Main.DoUpdate += Main_DoUpdate;

			On.Terraria.GameInput.PlayerInput.UpdateInput += PlayerInput_UpdateInput;

			HookEndpointManager.Modify(typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.UI.UILoadMods").GetMethod("OnDeactivate", Utility.defaultFlags), new Action<ILContext>(ShowIntroMessage));
		}

		private static void PlayerInput_MouseInput(On.Terraria.GameInput.PlayerInput.orig_MouseInput orig)
		{
			bool anyInput = false;
			Terraria.GameInput.PlayerInput.MouseInfoOld = Terraria.GameInput.PlayerInput.MouseInfo;
			Terraria.GameInput.PlayerInput.MouseInfo = Mouse.GetState();
			Terraria.GameInput.PlayerInput.ScrollWheelValue += Terraria.GameInput.PlayerInput.MouseInfo.ScrollWheelValue;
			int deltaX = Terraria.GameInput.PlayerInput.MouseInfo.X - Terraria.GameInput.PlayerInput.MouseInfoOld.X;
			int deltaY = Terraria.GameInput.PlayerInput.MouseInfo.Y - Terraria.GameInput.PlayerInput.MouseInfoOld.Y;
			if (deltaX != 0 || deltaY != 0 || Terraria.GameInput.PlayerInput.MouseInfo.ScrollWheelValue != Terraria.GameInput.PlayerInput.MouseInfoOld.ScrollWheelValue)
			{
				Terraria.GameInput.PlayerInput.MouseX = Terraria.GameInput.PlayerInput.MouseInfo.X;
				Terraria.GameInput.PlayerInput.MouseY = Terraria.GameInput.PlayerInput.MouseInfo.Y;
				anyInput = true;
			}

			Terraria.GameInput.PlayerInput.MouseKeys.Clear();
			if (Main.instance.IsActive && !MouseEvents.ButtonPressCaptured)
			{
				if (Terraria.GameInput.PlayerInput.MouseInfo.LeftButton == ButtonState.Pressed)
				{
					Terraria.GameInput.PlayerInput.MouseKeys.Add("Mouse1");
					anyInput = true;
				}

				if (Terraria.GameInput.PlayerInput.MouseInfo.RightButton == ButtonState.Pressed)
				{
					Terraria.GameInput.PlayerInput.MouseKeys.Add("Mouse2");
					anyInput = true;
				}

				if (Terraria.GameInput.PlayerInput.MouseInfo.MiddleButton == ButtonState.Pressed)
				{
					Terraria.GameInput.PlayerInput.MouseKeys.Add("Mouse3");
					anyInput = true;
				}

				if (Terraria.GameInput.PlayerInput.MouseInfo.XButton1 == ButtonState.Pressed)
				{
					Terraria.GameInput.PlayerInput.MouseKeys.Add("Mouse4");
					anyInput = true;
				}

				if (Terraria.GameInput.PlayerInput.MouseInfo.XButton2 == ButtonState.Pressed)
				{
					Terraria.GameInput.PlayerInput.MouseKeys.Add("Mouse5");
					anyInput = true;
				}
			}

			if (anyInput)
			{
				Terraria.GameInput.PlayerInput.CurrentInputMode = InputMode.Mouse;
				PlayerInput.Triggers.Current.UsedMovementKey = false;
			}
		}

		private static GameTime time;

		private static void Main_DoUpdate(On.Terraria.Main.orig_DoUpdate orig, Main self, GameTime gameTime)
		{
			time = gameTime;
			orig(self, gameTime);
		}

		private static void PlayerInput_UpdateInput(On.Terraria.GameInput.PlayerInput.orig_UpdateInput orig)
		{
			Utility.Input.Update(time);

			orig();

			//if (Utility.Input.RightMouseDown) PlayerInput.LockTileUseButton = true;
		}

		private static void Player_Update(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);

			if (cursor.TryGotoNext(i => i.MatchLdloc(28), i => i.MatchBrtrue(out _)))
			{
				cursor.Index += 2;

				cursor.RemoveRange(113);

				cursor.Emit(OpCodes.Ldarg, 0);
				cursor.EmitDelegate<Action<Player>>(player =>
				{
					if (BlockScrolling)
					{
						BlockScrolling = false;
						return;
					}

					if (!Main.playerInventory) player.InvokeMethod<object>("HandleHotbar");
					else
					{
						int delta = PlayerInput.ScrollWheelDelta / 120;
						if (Main.recBigList)
						{
							const int height = 42;
							const int y = 340;
							const int x = 310;
							PlayerInput.SetZoom_UI();
							int mulX = (Main.screenWidth - x - 280) / height;
							int mulY = (Main.screenHeight - y - 20) / height;
							if (new Rectangle(x, y, mulX * height, mulY * height).Contains(Main.MouseScreen.ToPoint()))
							{
								delta *= -1;
								int sign = Math.Sign(delta);
								while (delta != 0)
								{
									if (delta < 0)
									{
										Main.recStart -= mulX;
										if (Main.recStart < 0) Main.recStart = 0;
									}
									else
									{
										Main.recStart += mulX;
										if (Main.recStart > Main.numAvailableRecipes - mulX) Main.recStart = Main.numAvailableRecipes - mulX;
									}

									delta -= sign;
								}
							}

							PlayerInput.SetZoom_World();
						}

						Main.focusRecipe += delta;
						if (Main.focusRecipe > Main.numAvailableRecipes - 1) Main.focusRecipe = Main.numAvailableRecipes - 1;
						if (Main.focusRecipe < 0) Main.focusRecipe = 0;
					}
				});
			}
		}

		private static void Player_ToggleInv(On.Terraria.Player.orig_ToggleInv orig, Player self)
		{
			if (!Utility.Input.KeyboardHandler.Enabled) orig(self);
		}

		private static void Main_DoUpdate_Enter_ToggleChat(On.Terraria.Main.orig_DoUpdate_Enter_ToggleChat orig)
		{
			if (!Utility.Input.KeyboardHandler.Enabled) orig();
		}

		private static void PlayerInput_KeyboardInput(On.Terraria.GameInput.PlayerInput.orig_KeyboardInput orig)
		{
			if (!Utility.Input.KeyboardHandler.Enabled) orig();
			else
			{
				foreach (string key in PlayerInput.MouseKeys)
				{
					PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard].Processkey(PlayerInput.Triggers.Current, key);
				}
			}
		}

		private static void ItemSlot_LeftClick(ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
		{
			if (Main.mouseItem.modItem is IHasUI mouse) BaseLibrary.PanelGUI.UI.CloseUI(mouse);

			if (inv[slot].modItem is IHasUI hasUI) BaseLibrary.PanelGUI.UI.CloseUI(hasUI);

			orig(inv, context, slot);
		}

		private static void Player_DropSelectedItem(On.Terraria.Player.orig_DropSelectedItem orig, Player self)
		{
			if (self.HeldItem.modItem is IHasUI hasUI) BaseLibrary.PanelGUI.UI.CloseUI(hasUI);

			orig(self);
		}
	}
}