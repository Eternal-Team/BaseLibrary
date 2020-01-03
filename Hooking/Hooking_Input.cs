using Terraria;
using Terraria.GameInput;

namespace BaseLibrary
{
	public static partial class Hooking
	{
		private static void PlayerInput_KeyboardInput(On.Terraria.GameInput.PlayerInput.orig_KeyboardInput orig)
		{
		}

		private static void PlayerInput_MouseInput(On.Terraria.GameInput.PlayerInput.orig_MouseInput orig)
		{
		}

		private static void ReplaceInput(On.Terraria.GameInput.PlayerInput.orig_UpdateInput orig)
		{
			if (typeof(PlayerInput).GetValue<bool>("reinitialize")) typeof(PlayerInput).InvokeMethod("ReInitialize");

			PlayerInput.Triggers.Old = PlayerInput.Triggers.Current.Clone();

			Input.Input.Update(time);

			PlayerInput.Triggers.Update();

			PlayerInput.CacheZoomableValues();

			TriggersSet current = PlayerInput.Triggers.Current;
			Main.mouseLeft = current.MouseLeft;
			Main.mouseRight = current.MouseRight;
			Main.mouseMiddle = current.MouseMiddle;
			Main.mouseXButton1 = current.MouseXButton1;
			Main.mouseXButton2 = current.MouseXButton2;
		}

		private static void Main_DoUpdate_HandleInput(On.Terraria.Main.orig_DoUpdate_HandleInput orig, Main self)
		{
			orig(self);
		}
	}
}