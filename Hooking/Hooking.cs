using Terraria;
using Terraria.GameInput;
using OnPlayerInput = On.Terraria.GameInput.PlayerInput;
using OnMain = On.Terraria.Main;
using OnPlayer = On.Terraria.Player;

namespace BaseLibrary
{
	public static class Hooking
	{
		public static bool InUI;

		public static void Load()
		{
			OnPlayer.HandleHotbar += (orig, player) =>
			{
				// todo: do just for scrollable elements

				if (!InUI) orig(player);
				InUI = false;
			};

			OnPlayerInput.KeyboardInput += orig =>
			{
				if (!Utility.Input.KeyboardHandler.Enabled) orig();
				else
				{
					foreach (string key in PlayerInput.MouseKeys)
					{
						PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard].Processkey(PlayerInput.Triggers.Current, key);
					}
				}
			};

			OnPlayerInput.MouseInput += orig =>
			{
				if (!Utility.Input.MouseHandler.Enabled) orig();
			};

			OnMain.DoUpdate_HandleInput += (orig, self) =>
			{
				Utility.Input.Update();

				orig(self);

				if (Utility.Input.KeyboardHandler.Enabled) Main.keyState = Main.oldKeyState;
			};
		}
	}
}