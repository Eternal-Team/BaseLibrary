using BaseLibrary.Input;
using BaseLibrary.Input.Keyboard;
using BaseLibrary.Input.Mouse;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent.UI.Chat;
using Terraria.GameInput;
using Terraria.UI.Chat;

namespace BaseLibrary
{
	public class TerrariaLayer : Layer
	{
		public override void OnMouseMove(MouseMoveEventArgs args)
		{
			PlayerInput.MouseX = (int)args.X;
			PlayerInput.MouseY = (int)args.Y;

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

			//HandleChatOpen(args);
			//if (args.Handled) return;
			//HandleChat(args);
			//if (args.Handled) return;

			foreach (var pair in KeyConfiguration.KeyStatus)
			{
				if (pair.Value.Contains(args.Key.ToString()))
				{
					PlayerInput.Triggers.Current.KeyStatus[pair.Key] = true;
				}
			}
		}

		private KeyConfiguration KeyConfiguration
		{
			get
			{
				if (Main.gameMenu && !PlayerInput.WritingText) return PlayerInput.CurrentProfile.InputModes[InputMode.KeyboardUI];
				return PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard];
			}
		}

		//private static void HandleChatOpen(KeyboardEventArgs args)
		//{
		//	if (args.Key != Keys.Enter || KeyboardUtil.AltDown(args.Modifiers))
		//	{
		//		Main.chatRelease = true;
		//		return;
		//	}

		//	if (!Main.drawingPlayerChat && !Main.editSign && !Main.editChest && !Main.gameMenu /* && !Main.keyState.IsKeyDown(Keys.Escape)*/)
		//	{
		//		Main.PlaySound(10);
		//		Main.drawingPlayerChat = true;
		//		Main.clrInput();
		//		Main.chatText = "";

		//		args.Handled = true;
		//	}

		//	Main.chatRelease = false;
		//}

		//private static void HandleChat(KeyboardEventArgs args)
		//{
		//	if (Main.editSign) Main.drawingPlayerChat = false;

		//	if (!Main.drawingPlayerChat)
		//	{
		//		Main.startChatLine = 0;
		//		return;
		//	}

		//	Main.showCount = (int)(Main.screenHeight / 3f / Main.fontMouseText.MeasureString("1").Y) - 1;
		//	if (args.Key == Keys.Up)
		//	{
		//		Main.startChatLine++;
		//		if (Main.startChatLine + Main.showCount >= Main.numChatLines - 1)
		//		{
		//			Main.startChatLine = Main.numChatLines - Main.showCount - 1;
		//		}

		//		if (Main.chatLine[Main.startChatLine + Main.showCount].text == "")
		//		{
		//			Main.startChatLine--;
		//		}
		//		args.Handled = true;
		//	}
		//	else if (args.Key == Keys.Down)
		//	{
		//		Main.startChatLine--;
		//		if (Main.startChatLine < 0)
		//		{
		//			Main.startChatLine = 0;
		//		}
		//		args.Handled = true;
		//	}

		//	if (args.Key == Keys.Escape)
		//	{
		//		Main.drawingPlayerChat = false;
		//		args.Handled = true;
		//	}

		//	string a = Main.chatText;
		//	Main.chatText = Main.GetInputText(Main.chatText);
		//	int num = (int)(Main.screenWidth * (1f / Main.UIScale)) - 330;
		//	if (a != Main.chatText)
		//	{
		//		while (ChatManager.GetStringSize(Main.fontMouseText, Main.chatText, Vector2.One).X > num)
		//		{
		//			Main.chatText = Main.chatText.Substring(0, Main.chatText.Length - 1);
		//		}
		//	}

		//	if (a != Main.chatText)
		//	{
		//		Main.PlaySound(12);
		//	}

		//	if (args.Key == Keys.Enter)
		//	{
		//		args.Handled = true;
				
		//		bool handled = Main.chatText.Length > 0 && Main.chatText[0] == '/' /*&& CommandManager.HandleCommand(Main.chatText, new ChatCommandCaller())*/;
		//		if (Main.chatText != "" && !handled)
		//		{
		//			ChatMessage chatMessage = new ChatMessage(Main.chatText);
		//			ChatManager.Commands.ProcessOutgoingMessage(chatMessage);
		//			NetMessage.SendChatMessageFromClient(chatMessage);
		//			if (Main.netMode == 0)
		//			{
		//				Color c = Main.player[Main.myPlayer].ChatColor();
		//				string text = Main.chatText;
		//				text = NameTagHandler.GenerateTag(Main.player[Main.myPlayer].name) + " " + Main.chatText;
		//				Main.player[Main.myPlayer].chatOverhead.NewMessage(Main.chatText, Main.chatLength / 2);
		//				Main.NewTextMultiline(text, false, c, Main.TextMaxLengthForScreen);
		//			}
		//		}

		//		Main.chatText = "";
		//		Main.drawingPlayerChat = false;
		//		Main.chatRelease = false;
		//		PlayerInput.WritingText = true;
		//		Main.LocalPlayer.releaseHook = false;
		//		Main.LocalPlayer.releaseThrow = false;
		//		Main.PlaySound(11);
		//	}
		//}

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