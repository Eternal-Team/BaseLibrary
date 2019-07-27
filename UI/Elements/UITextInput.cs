using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.OS;
using Starbound.Input;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameInput;
using Terraria.UI;
using Terraria.UI.Chat;

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

		private Ref<string> _text;
		private bool _renderPanel;
		private bool _focused;

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

		private bool selecting;
		private int selectionStart;
		private int selectionEnd;
		private bool doubleClicked;

		private int caretTimer;
		private bool caretVisible;

		public bool SelectOnFirstClick;
		public bool AllowMultiline;

		public int MaxLength;
		public string HintText;
		public event Action OnTextChange;

		public HorizontalAlignment HorizontalAlignment = HorizontalAlignment.Left;
		public VerticalAlignment VerticalAlignment = VerticalAlignment.Top;

		public string Text
		{
			get => _text.Value;
			set => _text.Value = value;
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

		public bool RenderPanel
		{
			get => _renderPanel;
			set
			{
				_renderPanel = value;
				SetPadding(value ? 8 : 0);
			}
		}

		public UITextInput(ref Ref<string> text)
		{
			_text = text;

			Utility.Input.InterceptKeyboard += ShouldIntercept;
			KeyboardEvents.KeyTyped += KeyTyped;
		}

		public override void OnDeactivate()
		{
			Focused = false;

			Utility.Input.InterceptKeyboard -= ShouldIntercept;
			KeyboardEvents.KeyTyped -= KeyTyped;
		}

		private bool ShouldIntercept() => Focused;

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

					OnTextChange?.Invoke();

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

					OnTextChange?.Invoke();

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

					OnTextChange?.Invoke();

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

					OnTextChange?.Invoke();

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

						OnTextChange?.Invoke();
					}

					break;
				}
			}
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

			float clickedX = evt.MousePosition.X - GetTextPosition().X;

			for (int i = 0; i <= Text.Length; i++)
			{
				if (MeasureString(Text.Substring(0, i)).X < clickedX) selectionStart = i;
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

		public override void TripleClick(UIMouseEvent evt)
		{
			selectionEnd = 0;
			selectionStart = Text.Length;
			selecting = true;
		}

		private Vector2 MeasureString(string s) => Utility.Font.MeasureString(s) - new Vector2(s.Count(x => x == ' ') * 2, 0);

		private Vector2 GetTextPosition()
		{
			Vector2 textSize = MeasureString(Text);
			Vector2 vec = Vector2.Zero;
			if (HorizontalAlignment == HorizontalAlignment.Left) vec.X = InnerDimensions.X;
			else if (HorizontalAlignment == HorizontalAlignment.Center) vec.X = InnerDimensions.X + InnerDimensions.Width * 0.5f - textSize.X * 0.5f;
			else if (HorizontalAlignment == HorizontalAlignment.Right) vec.X = InnerDimensions.X + InnerDimensions.Width - textSize.X;

			if (VerticalAlignment == VerticalAlignment.Top) vec.Y = InnerDimensions.Y;
			else if (VerticalAlignment == VerticalAlignment.Center) vec.Y = InnerDimensions.Y + InnerDimensions.Height * 0.5f - textSize.Y * 0.5f;
			else if (VerticalAlignment == VerticalAlignment.Bottom) vec.Y = InnerDimensions.Y + InnerDimensions.Height - textSize.Y;

			return vec;
		}

		public override void Update(GameTime gameTime)
		{
			if ((Main.mouseLeft || Main.mouseRight || Main.mouseMiddle) && !IsMouseHovering) Focused = false;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (RenderPanel) spriteBatch.DrawPanel(Dimensions, Utility.ColorPanel, Color.Black);

			Vector2 textPosition = GetTextPosition();

			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Utility.Font, Text, textPosition, Color.White, 0f, Vector2.Zero, Vector2.One);

			float size = MeasureString(Text.Substring(0, selectionStart)).X;

			if (selecting)
			{
				spriteBatch.Draw(Main.magicPixel, new Rectangle(
					(int)(textPosition.X + MeasureString(Text.Substring(0, Math.Min(selectionStart, selectionEnd))).X),
					(int)textPosition.Y,
					(int)MeasureString(Text.Substring(Math.Min(selectionStart, selectionEnd), Math.Abs(selectionStart - selectionEnd))).X,
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
				spriteBatch.Draw(Main.magicPixel, new Rectangle((int)(textPosition.X + size) + 1, (int)textPosition.Y, 1, 20), CaretColor);
				spriteBatch.Draw(Main.magicPixel, new Rectangle((int)(textPosition.X + size) + 2, (int)textPosition.Y, 1, 20), CaretColor * 0.25f);
			}

			if (string.IsNullOrWhiteSpace(Text) && !Focused) ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Utility.Font, HintText, textPosition, Color.Gray, 0f, Vector2.Zero, Vector2.One);

			if (IsMouseHovering)
			{
				Hooking.SetCursor("BaseLibrary/Textures/UI/TextCursor", new Vector2(3.5f, 8.5f));
			}
		}
	}
}