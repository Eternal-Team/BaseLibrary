using BaseLibrary.Input;
using BaseLibrary.UI.New;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent.UI.Chat;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace BaseLibrary
{
	public class ChatUI : BaseElement
	{
		private UITextInput input;
		private Ref<string> chat = new Ref<string>("[item:2] getting [item:666] tags [c/FF0000:This text is red.] working [a:NO_HOBO]. was a real pain in the ass [g:4]");
		private bool justOpened;

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
				Padding = new Padding(8, 4, 8, 4),
				Display = Display.None
			};
			input.OnKeyReleased += args =>
			{
				if (!input.Focused) return;

				if (justOpened)
				{
					justOpened = false;
					return;
				}

				if (args.Key == Keys.Enter)
				{
					string text = input.Text;
					bool handled = text.Length > 0 && text[0] == '/' && typeof(CommandManager).InvokeMethod<bool>("HandleCommand", text, Activator.CreateInstance(typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.ChatCommandCaller")));

					if (!string.IsNullOrWhiteSpace(text) && !handled)
					{
						ChatMessage chatMessage = new ChatMessage(text);
						ChatManager.Commands.ProcessOutgoingMessage(chatMessage);
						NetMessage.SendChatMessageFromClient(chatMessage);
						if (Main.netMode == NetmodeID.SinglePlayer)
						{
							Color color = Main.LocalPlayer.ChatColor();

							Main.LocalPlayer.chatOverhead.NewMessage(text, Main.chatLength / 2);

							string chatText = $"{NameTagHandler.GenerateTag(Main.LocalPlayer.name)} {text}";
							Main.NewTextMultiline(chatText, false, color, Main.TextMaxLengthForScreen);
						}
					}

					Main.drawingPlayerChat = false;
					input.Text = "";
					input.Display = Display.None;
					input.Focused = false;
				}
				else if (args.Key == Keys.Escape)
				{
					Main.drawingPlayerChat = false;
					input.Text = "";
					input.Display = Display.None;
					input.Focused = false;
				}
			};
			input.OnKeyPressed += args =>
			{
				if (args.Key == Keys.Enter && !Main.drawingPlayerChat)
				{
					justOpened = true;

					Main.drawingPlayerChat = true;
					input.Display = Display.Visible;
					input.Focused = true;

					Main.PlaySound(SoundID.MenuOpen);

					args.Handled = true;
				}
			};
			Add(input);
		}

		protected override void Draw(SpriteBatch spriteBatch)
		{
			int start = Main.startChatLine;
			int end = Main.startChatLine + Main.showCount;
			if (end >= Main.numChatLines)
			{
				end = --Main.numChatLines;
				start = end - Main.showCount;
			}

			int yOff = 0;
			int hoverable = -1;
			int hoveredSnippet = -1;
			for (int i = start; i < end; i++)
			{
				if (Main.drawingPlayerChat || Main.chatLine[i].showTime > 0 && Main.chatLine[i].text.Length > 0)
				{
					ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, Main.chatLine[i].parsedText, new Vector2(88f, Main.screenHeight - 30 - 28 - yOff * 21), 0f, Vector2.Zero, Vector2.One, out var snippet);
					if (snippet >= 0 && Main.chatLine[i].parsedText[snippet].CheckForHover)
					{
						hoverable = i;
						hoveredSnippet = snippet;
					}
				}

				yOff++;
			}

			if (hoverable > -1)
			{
				Main.chatLine[hoverable].parsedText[hoveredSnippet].OnHover();
				if (Main.mouseLeft && Main.mouseLeftRelease) Main.chatLine[hoverable].parsedText[hoveredSnippet].OnClick();
			}
		}
	}
}