using Microsoft.Xna.Framework;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using On.Terraria;
using On.Terraria.GameInput;
using On.Terraria.UI;
using System;
using Terraria.ModLoader;

namespace BaseLibrary
{
	public static partial class Hooking
	{
		public static GameTime time;

		internal static void Load()
		{
			Main.DoUpdate += (orig, self, gameTime) =>
			{
				time = gameTime;
				orig(self, gameTime);
			};
			
			ItemSlot.LeftClick_ItemArray_int_int += CloseUI_ItemSlot;
			Player.DropSelectedItem += CloseUI_Drop;

			IL.Terraria.Main.DrawInterface_36_Cursor += DrawCursor;

			PlayerInput.UpdateInput += ReplaceInput;
			PlayerInput.KeyboardInput += PlayerInput_KeyboardInput;
			PlayerInput.MouseInput += PlayerInput_MouseInput;
			Main.DoUpdate_HandleInput += Main_DoUpdate_HandleInput;

			Main.DoUpdate_Enter_ToggleChat += Main_DoUpdate_Enter_ToggleChat;
			Main.DoUpdate_HandleChat += Main_DoUpdate_HandleChat;
			Main.DrawInterface_34_PlayerChat += Main_DrawInterface_34_PlayerChat;

			PlayerInput.UpdateMainMouse += PlayerInput_UpdateMainMouse;

			UserInterface.Update += UserInterface_Update;

			UserInterface.Draw += UserInterface_Draw;

		

			HookEndpointManager.Modify(typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.UI.UILoadMods").GetMethod("OnDeactivate", Utility.defaultFlags), new Action<ILContext>(ShowIntroMessage));
		}

		private static void PlayerInput_UpdateMainMouse(PlayerInput.orig_UpdateMainMouse orig)
		{
		}
	}
}