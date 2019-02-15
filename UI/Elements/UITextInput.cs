using System;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.OS;
using Terraria;
using Terraria.GameInput;
using Terraria.UI;
using Terraria.UI.Chat;

namespace BaseLibrary.UI.Elements
{
	public class UITextInput : BaseElement
	{
		public Ref<string> Text;
		public string HintText;

		private bool focused;

		private int blinkerCount;
		private bool blinkerState;
		private int blinkerPos;

		public static int BackSpaceCount
		{
			get => typeof(Main).GetValue<int>("backSpaceCount");
			set => typeof(Main).SetValue("backSpaceCount", value);
		}

		public event Action OnTextChange;

		public override void Click(UIMouseEvent evt)
		{
			if (focused) Unfocus();
			else Focus();

			base.Click(evt);
		}

		public override void RightClick(UIMouseEvent evt) => Text.Value = "";

		public void Focus()
		{
			focused = true;
			Main.clrInput();

			blinkerPos = Text.Value.Length;

			Main.LocalPlayer.showItemIcon = false;
			Main.ItemIconCacheUpdate(0);

			Main.blockInput = true;
			Main.editSign = true;
			Main.chatRelease = false;
		}

		public void Unfocus()
		{
			focused = false;
			Main.blockInput = false;
			Main.editSign = false;
			Main.chatRelease = true;
		}

		public static string GetInputText(string oldString)
		{
			if (!Main.hasFocus) return oldString;

			Main.inputTextEnter = false;
			Main.inputTextEscape = false;
			string finalText = oldString;
			string addedText = "";
			if (finalText == null) finalText = "";

			bool backPressed = false;
			if (Main.inputText.IsKeyDown(Keys.LeftControl) || Main.inputText.IsKeyDown(Keys.RightControl))
			{
				if (Keys.X.IsKeyPressed())
				{
					Platform.Current.Clipboard = oldString;
					finalText = "";
				}
				else if (Keys.C.IsKeyPressed() || Keys.Insert.IsKeyPressed()) Platform.Current.Clipboard = oldString;
				else if (Keys.V.IsKeyPressed()) addedText += Platform.Current.Clipboard;
			}
			else
			{
				if (Main.inputText.PressingShift())
				{
					if (Keys.Delete.IsKeyPressed())
					{
						Platform.Current.Clipboard = oldString;
						finalText = "";
					}

					if (Keys.Insert.IsKeyPressed())
					{
						string clipboard = Platform.Current.Clipboard;
						for (int i = 0; i < clipboard.Length; i++)
						{
							if (clipboard[i] < ' ' || clipboard[i] == '\u007f') clipboard = clipboard.Replace(string.Concat(clipboard[i--]), "");
						}

						addedText += clipboard;
					}
				}

				for (int j = 0; j < Main.keyCount; j++)
				{
					Keys key = (Keys)Main.keyInt[j];
					string str = Main.keyString[j];
					if (key == Keys.Enter) Main.inputTextEnter = true;
					else if (key == Keys.Escape) Main.inputTextEscape = true;
					else if (key >= Keys.Space && key != Keys.F16) addedText += str;
				}
			}

			Main.keyCount = 0;
			finalText += addedText;
			Main.oldInputText = Main.inputText;
			Main.inputText = Keyboard.GetState();
			if (Main.inputText.IsKeyDown(Keys.Back) && Main.oldInputText.IsKeyDown(Keys.Back))
			{
				if (BackSpaceCount == 0)
				{
					BackSpaceCount = 7;
					backPressed = true;
				}

				BackSpaceCount--;
			}
			else BackSpaceCount = 15;

			Keys[] pressedKeys = Main.inputText.GetPressedKeys();
			for (int k = 0; k < pressedKeys.Length; k++)
			{
				bool keysChanged = true;
				Keys[] pressedKeysOld = Main.oldInputText.GetPressedKeys();
				for (int l = 0; l < pressedKeysOld.Length; l++)
				{
					if (pressedKeys[k] == pressedKeysOld[l]) keysChanged = false;
				}

				if (pressedKeys[k] == Keys.Back && (keysChanged || backPressed) && finalText.Length > 0)
				{
					TextSnippet[] array = ChatManager.ParseMessage(finalText, Color.White).ToArray();
					finalText = array[array.Length - 1].DeleteWhole ? finalText.Substring(0, finalText.Length - array[array.Length - 1].TextOriginal.Length) : finalText.Substring(0, finalText.Length - 1);
				}
			}

			return finalText;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = GetDimensions();
			string displayString = Text.Value;

			if (focused)
			{
				PlayerInput.WritingText = true;
				Main.instance.HandleIME();

				string newString = GetInputText(Text.Value);
				if (!newString.Equals(Text))
				{
					Text.Value = newString;
					OnTextChange?.Invoke();
					Recalculate();
				}
				else Text.Value = newString;

				if (++blinkerCount >= 30)
				{
					blinkerState = !blinkerState;
					blinkerCount = 0;
				}
			}

			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontMouseText, displayString, dimensions.Center(), Color.White, 0f, displayString.Measure() * 0.5f, Vector2.One);
			//if (blinkerState) spriteBatch.Draw(Main.magicPixel, new Rectangle((int)(dimensions.X + dimensions.Width * 0.5f - displayString.Measure().X * 0.5f + blinkerPos * 8), (int)dimensions.Y, 1, (int)dimensions.Height), Color.LightGray);

			if (string.IsNullOrWhiteSpace(displayString) && !focused) ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontMouseText, HintText, dimensions.Center(), Color.Gray, 0f, displayString.Measure() * 0.5f, Vector2.One);
		}

		public override void Update(GameTime gameTime)
		{
			if (Keys.Enter.IsKeyDown() || Keys.Escape.IsKeyDown()) Unfocus();
		}
	}
}