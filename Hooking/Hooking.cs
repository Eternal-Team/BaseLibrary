using Microsoft.Xna.Framework.Input;
using Starbound.Input;
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
				orig();

				//bool changed = false;

				//PlayerInput.MouseInfoOld = PlayerInput.MouseInfo;
				//PlayerInput.MouseInfo = Mouse.GetState();

				//if (!MouseEvents.InterceptMouseWheel)
				//{
				//	PlayerInput.ScrollWheelValue += PlayerInput.MouseInfo.ScrollWheelValue;
				//	if (PlayerInput.MouseInfo.ScrollWheelValue != PlayerInput.MouseInfoOld.ScrollWheelValue) changed = true;
				//}

				//if (!MouseEvents.InterceptMouseMove)
				//{
				//	if (PlayerInput.MouseInfo.X - PlayerInput.MouseInfoOld.X != 0 || PlayerInput.MouseInfo.Y - PlayerInput.MouseInfoOld.Y != 0)
				//	{
				//		PlayerInput.MouseX = PlayerInput.MouseInfo.X;
				//		PlayerInput.MouseY = PlayerInput.MouseInfo.Y;
				//		changed = true;
				//	}
				//}

				//PlayerInput.MouseKeys.Clear();
				//if (MouseEvents.InterceptMouseButton && Main.instance.IsActive)
				//{
				//	if (PlayerInput.MouseInfo.LeftButton == ButtonState.Pressed)
				//	{
				//		PlayerInput.MouseKeys.Add("Mouse1");
				//		changed = true;
				//	}

				//	if (PlayerInput.MouseInfo.RightButton == ButtonState.Pressed)
				//	{
				//		PlayerInput.MouseKeys.Add("Mouse2");
				//		changed = true;
				//	}

				//	if (PlayerInput.MouseInfo.MiddleButton == ButtonState.Pressed)
				//	{
				//		PlayerInput.MouseKeys.Add("Mouse3");
				//		changed = true;
				//	}

				//	if (PlayerInput.MouseInfo.XButton1 == ButtonState.Pressed)
				//	{
				//		PlayerInput.MouseKeys.Add("Mouse4");
				//		changed = true;
				//	}

				//	if (PlayerInput.MouseInfo.XButton2 == ButtonState.Pressed)
				//	{
				//		PlayerInput.MouseKeys.Add("Mouse5");
				//		changed = true;
				//	}
				//}

				//if (changed)
				//{
				//	PlayerInput.CurrentInputMode = InputMode.Mouse;
				//	PlayerInput.Triggers.Current.UsedMovementKey = false;
				//}

				//MouseEvents.InterceptMouseWheel = MouseEvents.InterceptMouseButton = MouseEvents.InterceptMouseMove = false;
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