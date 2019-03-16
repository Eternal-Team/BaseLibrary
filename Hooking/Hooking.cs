using On.Terraria;
using Terraria.GameInput;
using OnPlayerInput = On.Terraria.GameInput.PlayerInput;

namespace BaseLibrary
{
	public static class Hooking
	{
		public static bool InUI;

		public static void Load()
		{
			Player.HandleHotbar += (orig, player) =>
			{
				// do just for scrollable elements?

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

			Main.DoUpdate_HandleInput += (orig, self) =>
			{
				Utility.Input.Update();

				orig(self);

				if (Utility.Input.KeyboardHandler.Enabled) Terraria.Main.keyState = Terraria.Main.oldKeyState;
			};
		}
	}
}