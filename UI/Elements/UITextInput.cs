using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.OS;
using Starbound.Input;
using Terraria;
using Terraria.UI;
using Terraria.UI.Chat;

namespace BaseLibrary.UI.Elements
{
	public class UITextInput : UIPanel
	{
		private Ref<string> _text;
		private bool focused;

		private int selectionStart;
		private int selectionEnd;

		public string Text
		{
			get => _text.Value;
			set => _text.Value = value;
		}

		public string HintText;
		public event Action OnTextChange;

		public UITextInput(Ref<string> text, string hintText = "")
		{
			SetPadding(8);

			_text = text;
			HintText = hintText;

			Utility.Input.InterceptKeyboard += ShouldIntercept;
			KeyboardEvents.KeyTyped += KeyTyped;
		}

		~UITextInput()
		{
			Utility.Input.InterceptKeyboard -= ShouldIntercept;
			KeyboardEvents.KeyTyped -= KeyTyped;
		}

		private bool ShouldIntercept() => focused;

		// mouse click (position, select word, select whole, drag)

		private void KeyTyped(object sender, KeyboardEventArgs args)
		{
			switch (args.Key)
			{
				case Keys.C when KeyboardUtil.ControlDown(args.Modifiers):
					// copy selection
					break;
				case Keys.V when KeyboardUtil.ControlDown(args.Modifiers):
					Text = Text.Insert(selectionStart, Platform.Current.Clipboard);
					selectionStart += Platform.Current.Clipboard.Length;
					// override selection
					break;
				case Keys.X when KeyboardUtil.ControlDown(args.Modifiers):
					// copy and delete selection
					break;
				case Keys.Delete:
					if (selectionStart < Text.Length) Text = Text.Remove(selectionStart, 1);

					// delete selection
					// ctrl - delete to next word
					break;
				case Keys.Back:
					if (selectionStart > 0) Text = Text.Remove(--selectionStart, 1);

					// delete selection
					// ctrl - delete to previous word
					break;
				case Keys.Escape:
				case Keys.Enter:
					focused = false;
					break;
				case Keys.Left:
					if (selectionStart - 1 >= 0) selectionStart--;
					caretVisible = true;
					caretTimer = 0;

					// ctrl - move index to previous word
					// shift - (un)select previous
					// ctrl + shift - (un)select previous word
					break;
				case Keys.Right:
					if (selectionStart + 1 <= Text.Length) selectionStart++;
					caretVisible = true;
					caretTimer = 0;

					// ctrl - move index to next word
					// shift - (un)select next
					// ctrl + shift - (un)select next word
					break;
				case Keys.Up:
					// ctrl - move index to previous line
					// shift - (un)select to same column on previous line
					break;
				case Keys.Down:
					// ctrl - move index to next line
					// shift - (un)select to same column on next line
					break;
				case Keys.Space:
					Text = Text.Insert(selectionStart++, " ");
					break;
				default:
					if (args.Character != null)
					{
						Text = Text.Insert(selectionStart, args.Character.ToString());
						selectionStart++;
					}

					break;
			}
		}

		// first time click - set index/select all
		public override void Click(UIMouseEvent evt) => focused = !focused;

		// todo: add special cursor for text

		private int caretTimer;
		private bool caretVisible;
		private static readonly Color caretColor = new Color(160, 160, 160);

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

			CalculatedStyle innerDimensions = GetInnerDimensions();

			ChatManager.DrawColorCodedString(spriteBatch, Utility.Font, Text, innerDimensions.Position(), Color.White, 0f, Vector2.Zero, Vector2.One);

			if (++caretTimer > 30)
			{
				caretVisible = !caretVisible;
				caretTimer = 0;
			}

			if (caretVisible)
			{
				spriteBatch.Draw(Main.magicPixel, new Rectangle((int)(innerDimensions.X + Utility.Font.MeasureString(Text.Substring(0, selectionStart)).X) + 1, (int)innerDimensions.Y, 1, 20), caretColor);
				spriteBatch.Draw(Main.magicPixel, new Rectangle((int)(innerDimensions.X + Utility.Font.MeasureString(Text.Substring(0, selectionStart)).X) + 2, (int)innerDimensions.Y, 1, 20), caretColor * 0.25f);
			}

			//if (string.IsNullOrWhiteSpace(displayString) && !focused) ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontMouDseText, HintText, dimensions.Center(), Color.Gray, 0f, displayString.Measure() * 0.5f, Vector2.One);
		}
	}
}