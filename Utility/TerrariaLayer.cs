using BaseLibrary.Input;
using BaseLibrary.Input.Mouse;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace BaseLibrary
{
	public class TerrariaLayer : Layer
	{
		public override bool Enabled => true;

		private KeyConfiguration KeyConfiguration
		{
			get
			{
				if (Main.gameMenu && !PlayerInput.WritingText) return PlayerInput.CurrentProfile.InputModes[InputMode.KeyboardUI];
				return PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard];
			}
		}

		private static FieldInfo originalMouseX = typeof(PlayerInput).GetField("_originalMouseX", Utility.defaultFlags);
		private static FieldInfo originalMouseY = typeof(PlayerInput).GetField("_originalMouseY", Utility.defaultFlags);
		private static FieldInfo originalLastMouseX = typeof(PlayerInput).GetField("_originalLastMouseX", Utility.defaultFlags);
		private static FieldInfo originalLastMouseY = typeof(PlayerInput).GetField("_originalLastMouseY", Utility.defaultFlags);

		public override void OnMouseMove(MouseMoveEventArgs args)
		{
			PlayerInput.MouseX = (int)args.X;
			PlayerInput.MouseY = (int)args.Y;

			Main.lastMouseX = Main.mouseX;
			Main.lastMouseY = Main.mouseY;
			Main.mouseX = PlayerInput.MouseX;
			Main.mouseY = PlayerInput.MouseY;

			// originalMouseX.SetValue(Main.mouseX);
			// originalMouseY.SetValue(Main.mouseY);
			// originalLastMouseX.SetValue(Main.lastMouseX);
			// originalLastMouseY.SetValue(Main.lastMouseY);
		}

		public override void OnUpdate(GameTime gameTime)
		{
			PlayerInput.ScrollWheelDelta = 0;
			// PlayerInput.ScrollWheelDeltaForUI = 0;

			ModContent.GetInstance<BaseLibrary>().Logger.Debug($"w:{originalScreenWidth.GetValue(null)} h:{originalScreenHeight.GetValue(null)}");
		}

		public override void OnMouseScroll(MouseScrollEventArgs args)
		{
			PlayerInput.ScrollWheelValueOld = PlayerInput.ScrollWheelValue;
			PlayerInput.ScrollWheelValue -= (int)args.OffsetY;

			PlayerInput.ScrollWheelDelta = (int)args.OffsetY;
			PlayerInput.ScrollWheelDeltaForUI = (int)args.OffsetY;
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
				foreach (var pair in PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard].KeyStatus)
				{
					if (pair.Value.Contains(key))
					{
						PlayerInput.Triggers.Current.KeyStatus[pair.Key] = false;
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

			foreach (var pair in KeyConfiguration.KeyStatus)
			{
				if (pair.Value.Contains(args.Key.ToString()))
				{
					PlayerInput.Triggers.Current.KeyStatus[pair.Key] = true;
				}
			}
		}

		private static FieldInfo originalScreenWidth = typeof(PlayerInput).GetField("_originalScreenWidth", Utility.defaultFlags);
		private static FieldInfo originalScreenHeight = typeof(PlayerInput).GetField("_originalScreenHeight", Utility.defaultFlags);

		// public override void OnWindowResize(WindowResizedEventArgs args)
		// {
		// 	originalScreenWidth.SetValue((int)args.Width);
		// 	originalScreenHeight.SetValue((int)args.Height);
		// }

		public override void OnKeyReleased(KeyboardEventArgs args)
		{
			foreach (var pair in KeyConfiguration.KeyStatus)
			{
				if (pair.Value.Contains(args.Key.ToString()))
				{
					PlayerInput.Triggers.Current.KeyStatus[pair.Key] = false;
				}
			}
		}
	}
}