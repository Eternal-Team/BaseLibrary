using BaseLibrary.Input;
using BaseLibrary.UI.New;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent.UI.Chat;
using Terraria.ID;
using Terraria.UI.Chat;

namespace BaseLibrary
{
	public class ChatUI : BaseElement
	{
		private UITextInput input;
		private Ref<string> chat = new Ref<string>("");

		public ChatUI()
		{
			Width.Percent = 100;
			Height.Percent = 100;

			input = new UITextInput(ref chat)
			{
				Width = { Percent = 100, Pixels = -300 },
				MinWidth = 500,
				Height = { Pixels = 32 },
				X = { Pixels = 78 },
				Y = { Percent = 100, Pixels = -4 },
				RenderPanel = true,
				PanelColor = new Color(63, 45, 151) * (100f / 255),
				VerticalAlignment = VerticalAlignment.Center,
				Padding = new Padding(8, 4, 8, 4)
			};
			input.OnKeyReleased += args =>
			{
				// note: closing
				if (args.Key == Keys.Escape)
				{
					Main.drawingPlayerChat = false;
					input.Display = Display.None;

					Main.PlaySound(SoundID.MenuOpen);
				}
				// note: submitting message
				else if (args.Key == Keys.Enter)
				{
					string text = input.Text;
					bool handled = text.Length > 0 && text[0] == '/' /*&& CommandManager.HandleCommand(text, new ChatCommandCaller())*/;

					if (!string.IsNullOrWhiteSpace(text) && !handled)
					{
						ChatMessage chatMessage = new ChatMessage(text);
						ChatManager.Commands.ProcessOutgoingMessage(chatMessage);
						NetMessage.SendChatMessageFromClient(chatMessage);
						if (Main.netMode == 0)
						{
							Color color = Main.LocalPlayer.ChatColor();

							Main.LocalPlayer.chatOverhead.NewMessage(text, Main.chatLength / 2);

							string chatText = $"{NameTagHandler.GenerateTag(Main.LocalPlayer.name)} {text}";
							Main.NewTextMultiline(chatText, false, color, Main.TextMaxLengthForScreen);
						}
					}

					input.Text = "";
					Main.drawingPlayerChat = false;
					input.Display = Display.None;

					Main.PlaySound(SoundID.MenuClose);
				}
			};
			Add(input);

			Main.drawingPlayerChat = true;
		}

		protected override void Draw(SpriteBatch spriteBatch)
		{
			int num3 = Main.startChatLine;
			int num4 = Main.startChatLine + Main.showCount;
			if (num4 >= Main.numChatLines)
			{
				num4 = --Main.numChatLines;
				num3 = num4 - Main.showCount;
			}

			int num5 = 0;
			int num6 = -1;
			int num7 = -1;
			for (int j = num3; j < num4; j++)
			{
				if (Main.drawingPlayerChat || Main.chatLine[j].showTime > 0 && Main.chatLine[j].text.Length > 0)
				{
					ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, Main.chatLine[j].parsedText, new Vector2(88f, Main.screenHeight - 30 - 28 - num5 * 21), 0f, Vector2.Zero, Vector2.One, out var num8);
					if (num8 >= 0 && Main.chatLine[j].parsedText[num8].CheckForHover)
					{
						num6 = j;
						num7 = num8;
					}
				}

				num5++;
			}

			if (num6 > -1)
			{
				Main.chatLine[num6].parsedText[num7].OnHover();
				if (Main.mouseLeft && Main.mouseLeftRelease)
				{
					Main.chatLine[num6].parsedText[num7].OnClick();
				}
			}
		}

		// note: opening
		protected override void KeyPressed(KeyboardEventArgs args)
		{
			if (args.Key == Keys.Enter && !Main.drawingPlayerChat)
			{
				Main.drawingPlayerChat = true;
				input.Display = Display.Visible;

				Main.PlaySound(SoundID.MenuOpen);

				args.Handled = true;
			}
		}
	}
}