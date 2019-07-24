using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.OS;
using Starbound.Input;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameInput;
using Terraria.UI;
using Terraria.UI.Chat;

namespace BaseLibrary.UI.Elements
{
	public class UITextInput : BaseElement
	{
		// todo: cleanup
		// note: support for Insert?

		private Ref<string> _text;
		private bool focused;

		private int selectionStart;
		private int selectionEnd;
		private bool selecting;

		private int caretTimer;
		private bool caretVisible;

		private static readonly Color CaretColor = new Color(160, 160, 160);
		private static readonly Color SelectionColor = new Color(51, 144, 255) * 0.25f;

		public string Text
		{
			get => _text.Value;
			set => _text.Value = value;
		}

		public bool SelectOnFirstClick;
		public string HintText;
		public event Action OnTextChange;
		private bool RenderPanel;
		public bool AllowMultiline;

		// todo: max lenght
		// todo: center text

		/*
		 while(s[i]!=wordboundary&&s[i-1]!=APLHANUM)
		 {
		 select
		 }
		 */

		public UITextInput(Ref<string> text, string hintText = "", bool renderPanel = true)
		{
			RenderPanel = renderPanel;
			SetPadding(RenderPanel ? 8 : 0);

			_text = text;
			HintText = hintText;

			Utility.Input.InterceptKeyboard += ShouldIntercept;

			KeyboardEvents.KeyTyped += KeyTyped;
		}

		public override void OnDeactivate()
		{
			Utility.Input.InterceptKeyboard -= ShouldIntercept;
			KeyboardEvents.KeyTyped -= KeyTyped;
		}

		private bool ShouldIntercept() => focused;

		private void KeyTyped(object sender, KeyboardEventArgs args)
		{
			switch (args.Key)
			{
				case Keys.A when KeyboardUtil.ControlDown(args.Modifiers):
				{
					selectionEnd = 0;
					selectionStart = Text.Length;
					selecting = true;
					break;
				}

				case Keys.C when KeyboardUtil.ControlDown(args.Modifiers):
				{
					Platform.Current.Clipboard = selecting ? Text.Substring(Math.Min(selectionStart, selectionEnd), Math.Abs(selectionStart - selectionEnd)) : Text;

					break;
				}

				case Keys.V when KeyboardUtil.ControlDown(args.Modifiers):
				{
					if (selecting)
					{
						selecting = false;

						Text = Text.Remove(Math.Min(selectionStart, selectionEnd), Math.Abs(selectionStart - selectionEnd));
						Text = Text.Insert(Math.Min(selectionStart, selectionEnd), Platform.Current.Clipboard);
						selectionStart = Math.Min(selectionStart, selectionEnd) + Platform.Current.Clipboard.Length;
					}
					else
					{
						Text = Text.Insert(selectionStart, Platform.Current.Clipboard);

						selectionStart += Platform.Current.Clipboard.Length;
					}

					break;
				}

				case Keys.X when KeyboardUtil.ControlDown(args.Modifiers):
				{
					if (selecting)
					{
						selecting = false;

						Platform.Current.Clipboard = Text.Substring(Math.Min(selectionStart, selectionEnd), Math.Abs(selectionStart - selectionEnd));
						Text = Text.Remove(Math.Min(selectionStart, selectionEnd), Math.Abs(selectionStart - selectionEnd));

						selectionStart = Math.Min(selectionStart, selectionEnd);
					}
					else
					{
						Platform.Current.Clipboard = Text;
						Text = string.Empty;
						selectionStart = 0;
					}

					break;
				}

				case Keys.Delete:
				{
					if (selecting)
					{
						selecting = false;

						Text = Text.Remove(Math.Min(selectionStart, selectionEnd), Math.Abs(selectionStart - selectionEnd));
						selectionStart = Math.Min(selectionStart, selectionEnd);
					}
					else if (selectionStart < Text.Length) Text = Text.Remove(selectionStart, 1);

					// ctrl - delete to next word
					break;
				}

				case Keys.Back:
				{
					if (selecting)
					{
						selecting = false;

						Text = Text.Remove(Math.Min(selectionStart, selectionEnd), Math.Abs(selectionStart - selectionEnd));
						selectionStart = Math.Min(selectionStart, selectionEnd);
					}
					else if (selectionStart > 0) Text = Text.Remove(--selectionStart, 1);

					// ctrl - delete to previous word
					break;
				}

				case Keys.Escape:
				{
					Task.Run(() =>
					{
						Thread.Sleep(20);

						focused = false;
						Main.blockInput = false;
						Main.editSign = false;
						Main.chatRelease = false;
						PlayerInput.WritingText = false;
					});

					Main.keyState = Main.oldKeyState;
					break;
				}

				case Keys.Left:
				{
					if (selectionStart - 1 >= 0) selectionStart--;

					if (KeyboardUtil.ShiftDown(args.Modifiers) && !selecting)
					{
						selecting = true;
						selectionEnd = selectionStart + 1;
					}
					else if (!KeyboardUtil.ShiftDown(args.Modifiers) && selecting)
					{
						selecting = false;

						if (selectionEnd < selectionStart) selectionStart = selectionEnd;
						else if (selectionStart > 0) selectionStart++;
					}

					caretVisible = true;
					caretTimer = 0;

					// ctrl - move index to previous word
					// ctrl + shift - (un)select previous word
					break;
				}

				case Keys.Right:
				{
					if (selectionStart + 1 <= Text.Length) selectionStart++;

					if (KeyboardUtil.ShiftDown(args.Modifiers) && !selecting)
					{
						selecting = true;
						selectionEnd = selectionStart - 1;
					}
					else if (!KeyboardUtil.ShiftDown(args.Modifiers) && selecting)
					{
						selecting = false;

						if (selectionEnd > selectionStart) selectionStart = selectionEnd;
						else if (selectionStart < Text.Length) selectionStart--;
					}

					caretVisible = true;
					caretTimer = 0;

					// ctrl - move index to next word
					// ctrl + shift - (un)select next word
					break;
				}

				default:
				{
					if (args.Character != null)
					{
						string charValue;
						if (args.Key == Keys.Enter)
						{
							if (AllowMultiline) charValue = "\n";
							else
							{
								Task.Run(() =>
								{
									Thread.Sleep(20);

									focused = false;
									Main.blockInput = false;
									Main.editSign = false;
									Main.chatRelease = false;
									PlayerInput.WritingText = false;
								});

								Main.keyState = Main.oldKeyState;

								return;
							}
						}
						else charValue = args.Character.Value.ToString();

						if (selecting)
						{
							selecting = false;

							Text = Text.Remove(Math.Min(selectionStart, selectionEnd), Math.Abs(selectionStart - selectionEnd));
							Text = Text.Insert(Math.Min(selectionStart, selectionEnd), charValue);

							selectionStart = Math.Min(selectionStart, selectionEnd) + 1;
						}
						else Text = Text.Insert(selectionStart++, charValue);
					}

					break;
				}
			}
		}

		public override void Click(UIMouseEvent evt)
		{
			if (!focused && SelectOnFirstClick)
			{
				selectionEnd = 0;
				selectionStart = Text.Length;
				selecting = true;
			}

			selecting = false;

			focused = true;
			Main.clrInput();
			Main.blockInput = true;
			Main.editSign = true;
			Main.chatRelease = true;
			PlayerInput.WritingText = true;

			float lenght = 0f;
			float x = evt.MousePosition.X - InnerDimensions.Position().X;
			int index = 0;
			while (lenght < x && index < Text.Length)
			{
				index++;
				lenght = Utility.Font.MeasureString(Text.Substring(0, index - 1)).X;
			}
			// bad selection calculation

			selectionStart = index.Clamp(0, Text.Length);

			caretVisible = true;
			caretTimer = 0;
		}

		public override void DoubleClick(UIMouseEvent evt)
		{
			// select current word
		}

		public override void TripleClick(UIMouseEvent evt)
		{
			selectionEnd = 0;
			selectionStart = Text.Length;
			selecting = true;
		}

		// todo: add special cursor for text

		private Vector2 MeasureString(string s) => Utility.Font.MeasureString(s) - new Vector2(s.Count(x => x == ' ') * 2, 0);

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (RenderPanel) spriteBatch.DrawPanel(Dimensions, Utility.ColorPanel, Color.Black);

			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Utility.Font, Text, InnerDimensions.Position(), Color.White, 0f, Vector2.Zero, Vector2.One);

			float size = MeasureString(Text.Substring(0, selectionStart)).X;

			if (selecting)
			{
				spriteBatch.Draw(Main.magicPixel, new Rectangle(
					(int)(InnerDimensions.X + MeasureString(Text.Substring(0, Math.Min(selectionStart, selectionEnd))).X),
					(int)InnerDimensions.Y,
					(int)MeasureString(Text.Substring(Math.Min(selectionStart, selectionEnd), Math.Abs(selectionStart - selectionEnd))).X,
					20
				), SelectionColor);
			}

			if (++caretTimer > 30)
			{
				caretVisible = !caretVisible;
				caretTimer = 0;
			}

			if (caretVisible && focused)
			{
				spriteBatch.Draw(Main.magicPixel, new Rectangle((int)(InnerDimensions.X + size) + 1, (int)InnerDimensions.Y, 1, 20), CaretColor);
				spriteBatch.Draw(Main.magicPixel, new Rectangle((int)(InnerDimensions.X + size) + 2, (int)InnerDimensions.Y, 1, 20), CaretColor * 0.25f);
			}

			if (string.IsNullOrWhiteSpace(Text) && !focused) ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Utility.Font, HintText, InnerDimensions.Position(), Color.Gray, 0f, Vector2.Zero, Vector2.One);
		}
	}
}