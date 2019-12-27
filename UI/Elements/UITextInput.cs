using BaseLibrary.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.OS;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameInput;
using Terraria.UI;

namespace BaseLibrary.UI.Elements
{
	public enum HorizontalAlignment
	{
		Left,
		Center,
		Right
	}

	public enum VerticalAlignment
	{
		Top,
		Center,
		Bottom
	}

	public class UITextInput : BaseElement
	{
		private static readonly Color CaretColor = new Color(160, 160, 160);
		private static readonly Color SelectionColor = new Color(51, 144, 255) * 0.25f;

		public string Text
		{
			get => _text?.Value;
			set
			{
				if (_text == null) return;

				_text.Value = value;

				CalculateTextMetrics();

				OnTextChange?.Invoke();
			}
		}

		public string SelectedText
		{
			get => Text.Substring(Math.Min(selectionStart, selectionEnd), Math.Abs(selectionStart - selectionEnd));
			set
			{
				Text = Text.Remove(Math.Min(selectionStart, selectionEnd), Math.Abs(selectionStart - selectionEnd));
				Text = Text.Insert(Math.Min(selectionStart, selectionEnd), value);
			}
		}

		private bool Focused
		{
			get => _focused;
			set
			{
				if (value)
				{
					_focused = true;
					Main.clrInput();
					Main.blockInput = true;
					Main.editSign = true;
					Main.chatRelease = true;
					PlayerInput.WritingText = true;
				}
				else
				{
					_focused = false;
					Main.blockInput = false;
					Main.editSign = false;
					Main.chatRelease = false;
					PlayerInput.WritingText = false;
				}
			}
		}

		public bool RenderPanel
		{
			get => _renderPanel;
			set
			{
				_renderPanel = value;
				SetPadding(value ? 8 : 0);
			}
		}

		private bool _focused;
		private bool _renderPanel;

		private Ref<string> _text;
		public bool AllowMultiline;

		private int caretTimer;
		private bool caretVisible;
		private bool doubleClicked;
		public string HintText;

		public HorizontalAlignment HorizontalAlignment = HorizontalAlignment.Left;

		public int MaxLength;

		private bool selecting;
		private int selectionEnd;
		private int selectionStart;

		public bool SelectOnFirstClick;
		public bool SizeToText;

		private Vector2 textPosition;
		private Vector2 textSize;
		public VerticalAlignment VerticalAlignment = VerticalAlignment.Top;

		public UITextInput(ref Ref<string> text)
		{
			_text = text;

			Utility.Input.InterceptKeyboard += ShouldIntercept;
			KeyboardEvents.KeyTyped += KeyTyped;
		}

		private void CalculateTextMetrics()
		{
			string text = Text ?? HintText;
			if (text == null) return;

			textSize = text.Measure(Utility.Font);
			if (SizeToText) Size = textSize;

			if (HorizontalAlignment == HorizontalAlignment.Left) textPosition.X = InnerDimensions.X;
			else if (HorizontalAlignment == HorizontalAlignment.Center) textPosition.X = InnerDimensions.X + InnerDimensions.Width * 0.5f - textSize.X * 0.5f;
			else if (HorizontalAlignment == HorizontalAlignment.Right) textPosition.X = InnerDimensions.X + InnerDimensions.Width - textSize.X;

			if (VerticalAlignment == VerticalAlignment.Top) textPosition.Y = InnerDimensions.Y;
			else if (VerticalAlignment == VerticalAlignment.Center) textPosition.Y = InnerDimensions.Y + InnerDimensions.Height * 0.5f - textSize.Y * 0.5f;
			else if (VerticalAlignment == VerticalAlignment.Bottom) textPosition.Y = InnerDimensions.Y + InnerDimensions.Height - textSize.Y;
		}

		public override void Click(UIMouseEvent evt)
		{
			if (doubleClicked)
			{
				doubleClicked = false;
				return;
			}

			if (!Focused && SelectOnFirstClick)
			{
				selectionEnd = 0;
				selectionStart = Text.Length;
				selecting = true;
			}

			selecting = false;

			Focused = true;

			float clickedX = evt.MousePosition.X - textPosition.X;

			for (int i = 0; i <= Text.Length; i++)
			{
				if (Text.Substring(0, i).Measure(Utility.Font).X < clickedX) selectionStart = i;
			}

			caretVisible = true;
			caretTimer = 0;
		}

		public override void DoubleClick(UIMouseEvent evt)
		{
			string[] split = Regex.Split(Text, "\\b");

			int stringStart = 0;
			foreach (string s in split)
			{
				if (selectionStart >= stringStart && selectionStart <= stringStart + s.Length && !string.IsNullOrWhiteSpace(s))
				{
					selecting = true;
					selectionEnd = stringStart;
					selectionStart = stringStart + s.Length;

					doubleClicked = true;
				}

				stringStart += s.Length;
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (RenderPanel) spriteBatch.DrawPanel(Dimensions, Utility.ColorPanel, Color.Black);

			if (string.IsNullOrWhiteSpace(Text)) goto draw_hint;

			Utils.DrawBorderStringFourWay(spriteBatch, Utility.Font, Text, textPosition.X, textPosition.Y, Color.White, Color.Black, Vector2.Zero);

			if (selecting)
			{
				spriteBatch.Draw(Main.magicPixel, new Rectangle(
					(int)(textPosition.X + Text.Substring(0, Math.Min(selectionStart, selectionEnd)).Measure(Utility.Font).X),
					(int)textPosition.Y,
					(int)Text.Substring(Math.Min(selectionStart, selectionEnd), Math.Abs(selectionStart - selectionEnd)).Measure(Utility.Font).X,
					20
				), SelectionColor);
			}

			if (++caretTimer > 30)
			{
				caretVisible = !caretVisible;
				caretTimer = 0;
			}

			if (caretVisible && Focused)
			{
				float size = Text.Substring(0, selectionStart).Measure(Utility.Font).X;
				spriteBatch.Draw(Main.magicPixel, new Rectangle((int)(textPosition.X + size) + 1, (int)textPosition.Y, 1, 20), CaretColor);
				spriteBatch.Draw(Main.magicPixel, new Rectangle((int)(textPosition.X + size) + 2, (int)textPosition.Y, 1, 20), CaretColor * 0.25f);
			}

			draw_hint:
			if (string.IsNullOrWhiteSpace(Text) && !string.IsNullOrWhiteSpace(HintText) && !Focused) Utils.DrawBorderStringFourWay(spriteBatch, Utility.Font, HintText, textPosition.X, textPosition.Y, new Color(10, 10, 10), new Color(50, 50, 50), Vector2.Zero);

			if (IsMouseHovering) Hooking.SetCursor("BaseLibrary/Textures/UI/TextCursor", new Vector2(3.5f, 8.5f));
		}

		private void KeyTyped(object sender, KeyboardEventArgs args)
		{
			if (!Focused) return;

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
					Platform.Current.Clipboard = selecting ? SelectedText : Text;

					break;
				}

				case Keys.V when KeyboardUtil.ControlDown(args.Modifiers):
				{
					if (selecting)
					{
						selecting = false;

						if (Text.Length + Platform.Current.Clipboard.Length > MaxLength) return;

						SelectedText = Platform.Current.Clipboard;
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

						Platform.Current.Clipboard = SelectedText;
						SelectedText = "";

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

						SelectedText = "";

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

						SelectedText = "";

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

						Focused = false;
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

				case Keys.Enter when !AllowMultiline:
				{
					Task.Run(() =>
					{
						Thread.Sleep(20);

						Focused = false;
					});

					Main.keyState = Main.oldKeyState;

					break;
				}

				case Keys.LeftAlt:
				case Keys.Tab:
					break;
				default:
				{
					if (args.Character != null)
					{
						string charValue = args.Character.Value.ToString();

						if (selecting)
						{
							selecting = false;

							SelectedText = charValue;

							selectionStart = Math.Min(selectionStart, selectionEnd) + 1;
							selectionEnd = selectionStart;
						}
						else
						{
							if (Text.Length + 1 > MaxLength) return;
							Text = Text.Insert(selectionStart++, charValue);
						}
					}

					break;
				}
			}
		}

		public override void OnDeactivate()
		{
			Focused = false;

			Utility.Input.InterceptKeyboard -= ShouldIntercept;
			KeyboardEvents.KeyTyped -= KeyTyped;
		}

		public event Action OnTextChange;

		public override void Recalculate()
		{
			base.Recalculate();

			CalculateTextMetrics();
		}

		private bool ShouldIntercept() => Focused;

		public override void TripleClick(UIMouseEvent evt)
		{
			selectionEnd = 0;
			selectionStart = Text.Length;
			selecting = true;
		}

		public override void Update(GameTime gameTime)
		{
			if ((Main.mouseLeft || Main.mouseRight || Main.mouseMiddle || Main.mouseXButton1 || Main.mouseXButton2) && !IsMouseHovering) Focused = false;
		}
	}
}