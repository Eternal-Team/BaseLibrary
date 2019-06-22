using IL.Terraria.UI;

namespace BaseLibrary
{
	public static partial class Hooking
	{
		public static bool InUI;

		public static void Load()
		{
			On.Terraria.UI.UIElement.GetElementAt += UIElement_GetElementAt;

			ItemSlot.OverrideHover += ItemSlot_OverrideHover;
			IL.Terraria.Main.DrawInterface_36_Cursor += Main_DrawInterface_36_Cursor;

			//OnPlayer.HandleHotbar += (orig, player) =>
			//{
			//    // todo: do just for scrollable elements

			//    if (!InUI) orig(player);
			//    InUI = false;
			//};

			#region thonk

			//Initialize();

			//OnUserInterface.Update += (orig, self, time) =>
			//{
			//    if (Main.gameMenu)
			//    {
			//        orig(self, time);
			//        return;
			//    }

			//    UserInterface.ActiveInstance.CurrentState?.Update(time);
			//};

			//OnPlayerInput.KeyboardInput += orig =>
			//{
			//    if (Utility.Input.KeyboardHandler.Enabled) orig();
			//    else
			//    {
			//        foreach (string key in PlayerInput.MouseKeys)
			//        {
			//            PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard].Processkey(PlayerInput.Triggers.Current, key);
			//        }
			//    }
			//};

			//MouseEvents.ButtonPressed += (sender, args) => { };

			//OnPlayerInput.MouseInput += orig =>
			//{
			//    bool changed = false;

			//    PlayerInput.MouseInfoOld = PlayerInput.MouseInfo;
			//    PlayerInput.MouseInfo = Mouse.GetState();

			//    if (PlayerInput.MouseInfoOld.X - PlayerInput.MouseInfo.X != 0 || PlayerInput.MouseY - PlayerInput.MouseInfo.Y != 0)
			//    {
			//        PlayerInput.MouseX = PlayerInput.MouseInfo.X;
			//        PlayerInput.MouseY = PlayerInput.MouseInfo.Y;
			//        changed = true;
			//    }

			//    PlayerInput.MouseKeys.Clear();
			//    if (!Utility.Input.InterceptMouseButton() && Main.instance.IsActive)
			//    {
			//        if (PlayerInput.MouseInfo.LeftButton == ButtonState.Pressed)
			//        {
			//            PlayerInput.MouseKeys.Add("Mouse1");
			//            changed = true;
			//        }

			//        if (PlayerInput.MouseInfo.RightButton == ButtonState.Pressed)
			//        {
			//            PlayerInput.MouseKeys.Add("Mouse2");
			//            changed = true;
			//        }

			//        if (PlayerInput.MouseInfo.MiddleButton == ButtonState.Pressed)
			//        {
			//            PlayerInput.MouseKeys.Add("Mouse3");
			//            changed = true;
			//        }

			//        if (PlayerInput.MouseInfo.XButton1 == ButtonState.Pressed)
			//        {
			//            PlayerInput.MouseKeys.Add("Mouse4");
			//            changed = true;
			//        }

			//        if (PlayerInput.MouseInfo.XButton2 == ButtonState.Pressed)
			//        {
			//            PlayerInput.MouseKeys.Add("Mouse5");
			//            changed = true;
			//        }
			//    }

			//    if (changed)
			//    {
			//        PlayerInput.CurrentInputMode = InputMode.Mouse;
			//        PlayerInput.Triggers.Current.UsedMovementKey = false;
			//    }
			//};

			//OnMain.DoUpdate_HandleInput += (orig, self) =>
			//{
			//    Utility.Input.Update();

			//    orig(self);

			//    if (!Utility.Input.KeyboardHandler.Enabled) Main.keyState = Main.oldKeyState;
			//}; 

			#endregion
		}
	}
}