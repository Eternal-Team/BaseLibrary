using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using Newtonsoft.Json;
using On.Terraria;
using On.Terraria.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Terraria.GameInput;
using Terraria.ModLoader;
using Item = Terraria.Item;
using PlayerInput = On.Terraria.GameInput.PlayerInput;
using Utils = Terraria.Utils;

namespace BaseLibrary
{
	public static partial class Hooking
	{
		public static bool BlockScrolling;

		internal static void Load()
		{
			ItemSlot.LeftClick_ItemArray_int_int += ItemSlot_LeftClick;
			Player.DropSelectedItem += Player_DropSelectedItem;
			UIElement.GetElementAt += UIElement_GetElementAt;

			UserInterface.Update += UserInterface_Update;

			IL.Terraria.Main.DrawInterface_36_Cursor += Main_DrawInterface_36_Cursor;
			PlayerInput.KeyboardInput += PlayerInput_KeyboardInput;

			Main.DoUpdate_Enter_ToggleChat += Main_DoUpdate_Enter_ToggleChat;

			Player.ToggleInv += Player_ToggleInv;

			IL.Terraria.Player.Update += Player_Update;
			SetupRecipes += ModContent_SetupRecipes;

			Scheduler.EnqueueMessage(() => Terraria.Main.OnPostDraw += OnPostDraw);

			Initialize();
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
							if (new Rectangle(x, y, mulX * height, mulY * height).Contains(Utils.ToPoint(Terraria.Main.MouseScreen)))
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
			if (!Utility.Input.KeyboardHandler.Enabled) orig(self);
		}

		private static void Main_DoUpdate_Enter_ToggleChat(Main.orig_DoUpdate_Enter_ToggleChat orig)
		{
			if (!Utility.Input.KeyboardHandler.Enabled) orig();
		}

		private static void PlayerInput_KeyboardInput(PlayerInput.orig_KeyboardInput orig)
		{
			if (!Utility.Input.KeyboardHandler.Enabled) orig();
			else
			{
				foreach (string key in Terraria.GameInput.PlayerInput.MouseKeys)
				{
					Terraria.GameInput.PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard].Processkey(Terraria.GameInput.PlayerInput.Triggers.Current, key);
				}
			}
		}

		private static void UserInterface_Update(UserInterface.orig_Update orig, Terraria.UI.UserInterface self, GameTime time)
		{
			Utility.Input.Update(time);

			orig(self, time);
		}

		private static void ModContent_SetupRecipes(orig_SetupRecipes orig, CancellationToken token)
		{
			Dictionary<string, Version> previousVersions = new Dictionary<string, Version>();
			if (File.Exists(LastVersionsPath)) previousVersions = JsonConvert.DeserializeObject<Dictionary<string, Version>>(File.ReadAllText(LastVersionsPath));

			Type type = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.Core.ModOrganizer");
			object[] arr = type.InvokeMethod<object[]>("FindMods");

			newOrUpdated = ModLoader.Mods.Where(mod =>
			{
				object o = arr.FirstOrDefault(x => x.GetValue<string>("Name") == mod.Name);
				if (o != null && !o.GetValue<object>("properties").GetValue<string>("author").Contains("Itorius")) return false;

				return previousVersions.ContainsKey(mod.Name) && previousVersions[mod.Name] != mod.Version || !previousVersions.ContainsKey(mod.Name);
			}).ToList();
			if (newOrUpdated.Count > 0) Terraria.Main.menuMode = 4040;

			File.WriteAllText(LastVersionsPath, JsonConvert.SerializeObject(ModLoader.Mods.Select(mod => new {Key = mod.Name, Value = mod.Version.ToString()}).ToDictionary(x => x.Key, x => x.Value)));

			orig(token);
		}

		public delegate void orig_SetupRecipes(CancellationToken token);

		public delegate void hook_SetupRecipes(orig_SetupRecipes orig, CancellationToken token);

		public static MethodBase method => typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.ModContent").GetMethod("SetupRecipes", Utility.defaultFlags, null, new[] {typeof(CancellationToken)}, null);

		public static event hook_SetupRecipes SetupRecipes
		{
			add => HookEndpointManager.Add(method, value);
			remove => HookEndpointManager.Remove(method, value);
		}

		internal static void Unload()
		{
			Uninitialize();

			Scheduler.EnqueueMessage(() => Terraria.Main.OnPostDraw -= OnPostDraw);
		}

		private static void ItemSlot_LeftClick(ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
		{
			if(Terraria.Main.mouseItem.modItem is IHasUI mouse) BaseLibrary.PanelGUI.UI.CloseUI(mouse);
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