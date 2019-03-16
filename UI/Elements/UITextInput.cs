using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.OS;
using Starbound.Input;
using Terraria;
using Terraria.UI;
using Terraria.UI.Chat;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace BaseLibrary.UI.Elements
{
	public class UITextInput : UIPanel
	{
		private Ref<string> _text;
		private bool focused;

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
					// copy selection - Platform.Current.Clipboard
                    break;
                case Keys.V when KeyboardUtil.ControlDown(args.Modifiers):
					// paste at index/override selection - Platform.Current.Clipboard
	                break;
                case Keys.X when KeyboardUtil.ControlDown(args.Modifiers):
	                // copy and delete selection - Platform.Current.Clipboard
                    break;
                case Keys.Delete:
                    // delete at index
                    // delete selection
					// ctrl - delete to next word
                    break;
                case Keys.Back:
	                // delete at index
	                // delete selection
	                // ctrl - delete to previous word
                    break;
                case Keys.Escape:
					// unfocus
	                break;
                case Keys.Enter:
					// unfocus, new line if UITextInputMultiline
	                break;
                case Keys.Left:
                    // move index previous
                    // ctrl - move index to previous word
                    // shift - (un)select previous
                    // ctrl + shift - (un)select previous word
                    break;
                case Keys.Right:
                    // move index next
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
                default:
	                if (args.Character != null) Text += args.Character.Value;
	                break;
            }
		}

		public override void Click(UIMouseEvent evt) => focused = !focused;

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

			CalculatedStyle innerDimensions = GetInnerDimensions();

			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontMouseText, Text, innerDimensions.Position(), Color.White, 0f, Vector2.Zero, Vector2.One);
			//if (blinkerState) spriteBatch.Draw(Main.magicPixel, new Rectangle((int)(dimensions.X + dimensions.Width * 0.5f - displayString.Measure().X * 0.5f + blinkerPos * 8), (int)dimensions.Y, 1, (int)dimensions.Height), Color.LightGray);

			//if (string.IsNullOrWhiteSpace(displayString) && !focused) ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontMouseText, HintText, dimensions.Center(), Color.Gray, 0f, displayString.Measure() * 0.5f, Vector2.One);
		}

		public override void Update(GameTime gameTime)
		{
			//if (Keys.Enter.IsKeyDown() || Keys.Escape.IsKeyDown()) Unfocus();
		}
	}
}