using BaseLibrary.Input;
using BaseLibrary.Input.Mouse;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;

namespace BaseLibrary
{
	public class TerrariaLayer : Layer
	{
		public override void OnMouseMove(MouseMoveEventArgs args)
		{
			PlayerInput.MouseX = args.X;
			PlayerInput.MouseY = args.Y;

			Main.lastMouseX = Main.mouseX;
			Main.lastMouseY = Main.mouseY;
			Main.mouseX = PlayerInput.MouseX;
			Main.mouseY = PlayerInput.MouseY;
		}

		public override void OnUpdate(GameTime gameTime)
		{
			PlayerInput.ScrollWheelDelta = 0;
			PlayerInput.ScrollWheelDeltaForUI = 0;
		}

		public override void OnMouseScroll(MouseScrollEventArgs args)
		{
			PlayerInput.ScrollWheelValueOld = PlayerInput.ScrollWheelValue;
			PlayerInput.ScrollWheelValue -= args.OffsetY;

			PlayerInput.ScrollWheelDelta = args.OffsetY;
			PlayerInput.ScrollWheelDeltaForUI = args.OffsetY;
		}

		public override void OnMouseDown(MouseButtonEventArgs args)
		{
			switch (args.Button)
			{
				case MouseButton.Left:
					PlayerInput.MouseKeys.Add("Mouse1");
					break;
				case MouseButton.Right:
					PlayerInput.MouseKeys.Add("Mouse2");
					break;
				case MouseButton.Middle:
					PlayerInput.MouseKeys.Add("Mouse3");
					break;
				case MouseButton.XButton1:
					PlayerInput.MouseKeys.Add("Mouse4");
					break;
				case MouseButton.XButton2:
					PlayerInput.MouseKeys.Add("Mouse5");
					break;
			}

			foreach (string key in PlayerInput.MouseKeys)
			{
				foreach (KeyValuePair<string, List<string>> current in PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard].KeyStatus)
				{
					if (current.Value.Contains(key))
					{
						PlayerInput.Triggers.Old.KeyStatus[current.Key] = false;
						PlayerInput.Triggers.Current.KeyStatus[current.Key] = true;
					}
				}
			}

			args.Handled = true;
		}

		public override void OnMouseUp(MouseButtonEventArgs args)
		{
			foreach (string key in PlayerInput.MouseKeys)
			{
				foreach (KeyValuePair<string, List<string>> current in PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard].KeyStatus)
				{
					if (current.Value.Contains(key))
					{
						PlayerInput.Triggers.Old.KeyStatus[current.Key] = true;
						PlayerInput.Triggers.Current.KeyStatus[current.Key] = false;
					}
				}
			}

			switch (args.Button)
			{
				case MouseButton.Left:
					PlayerInput.MouseKeys.Remove("Mouse1");
					break;
				case MouseButton.Right:
					PlayerInput.MouseKeys.Remove("Mouse2");
					break;
				case MouseButton.Middle:
					PlayerInput.MouseKeys.Remove("Mouse3");
					break;
				case MouseButton.XButton1:
					PlayerInput.MouseKeys.Remove("Mouse4");
					break;
				case MouseButton.XButton2:
					PlayerInput.MouseKeys.Remove("Mouse5");
					break;
			}

			args.Handled = true;
		}

		public override void OnKeyPressed(KeyboardEventArgs args)
		{
			string newKey = args.Key.ToString();
			InputMode listeningInputMode = typeof(PlayerInput).GetValue<InputMode>("_listeningInputMode");

			if (PlayerInput.CurrentlyRebinding && listeningInputMode == InputMode.Keyboard)
			{
				PlayerInput.NavigatorRebindingLock = 3;
				Main.PlaySound(12);
				if (PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard].KeyStatus[PlayerInput.ListeningTrigger].Contains(newKey))
				{
					PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard].KeyStatus[PlayerInput.ListeningTrigger].Remove(newKey);
				}
				else
				{
					PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard].KeyStatus[PlayerInput.ListeningTrigger] = new List<string>
					{
						newKey
					};
				}

				PlayerInput.ListenFor(null, InputMode.Keyboard);

				return;
			}

			if (PlayerInput.CurrentlyRebinding && listeningInputMode == InputMode.KeyboardUI)
			{
				PlayerInput.NavigatorRebindingLock = 3;
				Main.PlaySound(12);
				if (PlayerInput.CurrentProfile.InputModes[InputMode.KeyboardUI].KeyStatus[PlayerInput.ListeningTrigger].Contains(newKey))
				{
					PlayerInput.CurrentProfile.InputModes[InputMode.KeyboardUI].KeyStatus[PlayerInput.ListeningTrigger].Remove(newKey);
				}
				else
				{
					PlayerInput.CurrentProfile.InputModes[InputMode.KeyboardUI].KeyStatus[PlayerInput.ListeningTrigger] = new List<string>
					{
						newKey
					};
				}

				PlayerInput.ListenFor(null, InputMode.KeyboardUI);

				return;
			}

			KeyConfiguration keyConfiguration = PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard];
			if (Main.gameMenu && !PlayerInput.WritingText) keyConfiguration = PlayerInput.CurrentProfile.InputModes[InputMode.KeyboardUI];
			foreach (KeyValuePair<string, List<string>> current in keyConfiguration.KeyStatus)
			{
				if (current.Value.Contains(args.Key.ToString()))
				{
					PlayerInput.Triggers.Current.KeyStatus[current.Key] = true;
				}
			}
		}

		public override void OnKeyReleased(KeyboardEventArgs args)
		{
			KeyConfiguration keyConfiguration = PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard];
			if (Main.gameMenu && !PlayerInput.WritingText) keyConfiguration = PlayerInput.CurrentProfile.InputModes[InputMode.KeyboardUI];

			foreach (KeyValuePair<string, List<string>> current in keyConfiguration.KeyStatus)
			{
				if (current.Value.Contains(args.Key.ToString()))
				{
					PlayerInput.Triggers.Current.KeyStatus[current.Key] = false;
				}
			}
		}
	}
}