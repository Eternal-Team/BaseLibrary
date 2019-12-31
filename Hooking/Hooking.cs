using Microsoft.Xna.Framework;
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

		public static GameTime time;

		internal static void Load()
		{
			ItemSlot.LeftClick_ItemArray_int_int += CloseUI_ItemSlot;
			On.Terraria.Player.DropSelectedItem += CloseUI_Drop;
			UIElement.GetElementAt += UIElement_GetElementAt;

			IL.Terraria.Main.DrawInterface_36_Cursor += DrawCursor;

			On.Terraria.GameInput.PlayerInput.UpdateInput += ReplaceInput;
			On.Terraria.GameInput.PlayerInput.KeyboardInput += PlayerInput_KeyboardInput;
			On.Terraria.GameInput.PlayerInput.MouseInput += PlayerInput_MouseInput;

			On.Terraria.Main.DoUpdate += (orig, self, gameTime) =>
			{
				time = gameTime;
				orig(self, gameTime);
			};

			HookEndpointManager.Modify(typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.UI.UILoadMods").GetMethod("OnDeactivate", Utility.defaultFlags), new Action<ILContext>(ShowIntroMessage));
		}
	}
}