using System;
using System.Text.RegularExpressions;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using ReLogic.Graphics;
using ReLogic.OS;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace BaseLibrary.UI;

public struct UITextInputSettings
{
	public static readonly UITextInputSettings Default = new()
	{
		CaretColor = new Color(160, 160, 160),
		SelectionColor = new Color(51, 144, 255) * 0.4f,
		HintText = "",
		AllowMultiline = false,
		MaxLength = null,
		HorizontalAlignment = HorizontalAlignment.Left,
		VerticalAlignment = VerticalAlignment.Top,
		SelectOnFirstClick = false
	};

	public Color CaretColor;
	public Color SelectionColor;
	public string HintText;
	public bool AllowMultiline;
	public int? MaxLength;
	public HorizontalAlignment HorizontalAlignment;
	public VerticalAlignment VerticalAlignment;
	public bool SelectOnFirstClick;
}

// todo: selection of text when dragging
// todo: control Z/Y
public class UITextInput : BaseElement
{
	public UITextInputSettings Settings = UITextInputSettings.Default;
	public static Asset<DynamicSpriteFont> Font { get; internal set; }

	public Action OnTextChange;
	public bool Focused;

	public string Text
	{
		get => text.Value;
		set
		{
			text.Value = value;

			if (string.IsNullOrWhiteSpace(value))
			{
				selecting = false;
				selectionStart = 0;
				selectionEnd = 0;
			}

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

	private static string Clipboard
	{
		get => Platform.Get<IClipboard>().Value;
		set => Platform.Get<IClipboard>().Value = value;
	}

	private Ref<string> text;
	private int caretTimer;
	private bool caretVisible;
	private bool selecting;
	private int selectionEnd;
	private int selectionStart;
	private Vector2 textPosition;
	private Vector2 textSize;
	private TextSnippet selected;

	public UITextInput(ref Ref<string> text)
	{
		this.text = text;

		if (text?.Value == null) throw new ArgumentNullException(nameof(text));
	}

	#region Mouse events
	protected override void MouseClick(MouseButtonEventArgs args)
	{
		if (args.Button != MouseButton.Left) return;

		args.Handled = true;

		if (!Focused && Settings.SelectOnFirstClick)
		{
			selectionEnd = 0;
			selectionStart = Text.Length;
			selecting = true;
		}

		selecting = false;

		Focused = true;

		float clickedX = args.Position.X - textPosition.X;

		var list = ChatManager.ParseMessage(Text, Color.White);

		for (int i = 0; i <= Text.Length; i++)
		{
			if (ChatManager.GetStringSize(Font.Value, Text.Substring(0, i), Vector2.One).X < clickedX) selectionStart = i;
		}

		selected = null;

		int current = 0;
		foreach (TextSnippet snippet in list)
		{
			int prev = current;
			current += snippet.TextOriginal.Length;

			if (snippet.Text == snippet.TextOriginal && !snippet.DeleteWhole) continue;

			if (selectionStart >= prev && selectionStart <= current)
			{
				selectionStart = prev;
				selected = snippet;
			}
		}

		selectionEnd = selectionStart;

		caretVisible = true;
		caretTimer = 0;
	}

	protected override void DoubleClick(MouseButtonEventArgs args)
	{
		if (args.Button != MouseButton.Left) return;

		args.Handled = true;

		if (selected != null)
		{
			selectionEnd = selectionStart + selected.TextOriginal.Length;
			selecting = true;
			return;
		}

		selecting = true;

		Regex regex = new Regex("\\b");
		while (selectionEnd - 1 >= 0 && regex.IsMatch(Text[selectionEnd - 1].ToString())) selectionEnd--;

		if (!regex.IsMatch(Text[selectionStart].ToString())) return;

		while (selectionStart + 1 <= Text.Length && regex.IsMatch(Text[selectionStart + 1].ToString())) selectionStart++;

		selectionStart++;
	}

	protected override void TripleClick(MouseButtonEventArgs args)
	{
		if (args.Button != MouseButton.Left) return;

		args.Handled = true;

		selectionEnd = 0;
		selectionStart = Text.Length;
		selecting = true;
	}

	protected override void MouseDown(MouseButtonEventArgs args)
	{
		if (args.Button != MouseButton.Left) return;

		args.Handled = true;

		base.MouseDown(args);
	}

	protected override void MouseUp(MouseButtonEventArgs args)
	{
		if (args.Button != MouseButton.Left) return;

		args.Handled = true;

		base.MouseUp(args);
	}
	#endregion

	#region Key events
	protected override void KeyPressed(KeyboardEventArgs args)
	{
		args.Handled = Focused;

		base.KeyPressed(args);
	}

	protected override void KeyReleased(KeyboardEventArgs args)
	{
		base.KeyReleased(args);

		args.Handled = Focused;
	}

	protected override void KeyTyped(KeyboardEventArgs args)
	{
		if (!Focused) return;
		if (args.Key == Keys.Enter && !Settings.AllowMultiline) return;
		if (args.Key == Keys.LeftAlt || args.Key == Keys.Tab)
		{
			args.Handled = true;
			return;
		}

		args.Handled = true;

		if (KeyboardUtil.ControlDown(args.Modifiers))
		{
			if (args.Key == Keys.A) HandleSelectAll();
			else if (args.Key == Keys.C) HandleCopy();
			else if (args.Key == Keys.V) HandlePaste();
			else if (args.Key == Keys.X) HandleCut();
		}
		else if (args.Key == Keys.Delete) HandleDelete();
		else if (args.Key == Keys.Back) HandleBackspace();
		else if (args.Key == Keys.Left) HandleGoLeft(args);
		else if (args.Key == Keys.Right) HandleGoRight(args);
		// todo: up, down
		else
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
					if (Settings.MaxLength != null && Text.Length + 1 > Settings.MaxLength) return;
					Text = Text.Insert(selectionStart++, charValue);
				}
			}
		}
	}
	#endregion

	#region Action handling
	private void HandleGoRight(KeyboardEventArgs args)
	{
		string next = Text.Substring(selectionStart, Text.Length - selectionStart);
		var list = ChatManager.ParseMessage(next, Color.White);

		int width = 1;
		if (list.Count > 0 && (list[0].Text != list[0].TextOriginal || list[0].DeleteWhole)) width = list[0].TextOriginal.Length;
		selectionStart += width;
		selectionStart = MathUtility.Clamp(selectionStart, 0, Text.Length);
		if (KeyboardUtil.ShiftDown(args.Modifiers) && !selecting)
		{
			selecting = true;
			selectionEnd = selectionStart - width;
		}
		else if (!KeyboardUtil.ShiftDown(args.Modifiers) && selecting)
		{
			selecting = false;

			if (selectionEnd > selectionStart) selectionStart = selectionEnd;
			else if (selectionStart < Text.Length) selectionStart -= width;
		}

		caretVisible = true;
		caretTimer = 0;

		// ctrl - move index to next word
		// ctrl + shift - (un)select next word
	}

	private void HandleGoLeft(KeyboardEventArgs args)
	{
		string next = Text.Substring(0, selectionStart);
		var list = ChatManager.ParseMessage(next, Color.White);

		int width = 1;
		if (list.Count > 0 && (list[list.Count - 1].Text != list[list.Count - 1].TextOriginal || list[list.Count - 1].DeleteWhole)) width = list[list.Count - 1].TextOriginal.Length;
		selectionStart -= width;
		selectionStart = MathUtility.Clamp(selectionStart, 0, Text.Length);
		if (KeyboardUtil.ShiftDown(args.Modifiers) && !selecting)
		{
			selecting = true;
			selectionEnd = selectionStart + width;
		}
		else if (!KeyboardUtil.ShiftDown(args.Modifiers) && selecting)
		{
			selecting = false;

			if (selectionEnd < selectionStart) selectionStart = selectionEnd;
			else if (selectionStart > 0) selectionStart += width;
		}

		caretVisible = true;
		caretTimer = 0;

		// ctrl - move index to previous word
		// ctrl + shift - (un)select previous word
	}

	private void HandleBackspace()
	{
		if (selecting)
		{
			selecting = false;

			SelectedText = "";

			selectionStart = Math.Min(selectionStart, selectionEnd);
		}

		else if (selectionStart > 0)
		{
			string next = Text.Substring(0, selectionStart);
			var list = ChatManager.ParseMessage(next, Color.White);
			int width = 1;
			if (list.Count > 0 && (list[list.Count - 1].Text != list[list.Count - 1].TextOriginal || list[list.Count - 1].DeleteWhole)) width = list[list.Count - 1].TextOriginal.Length;

			selectionStart -= width;

			Text = Text.Remove(selectionStart, width);
		}

		// ctrl - delete to previous word
	}

	private void HandleDelete()
	{
		if (selecting)
		{
			selecting = false;

			SelectedText = "";

			selectionStart = Math.Min(selectionStart, selectionEnd);
		}

		else if (selectionStart < Text.Length)
		{
			string next = Text.Substring(selectionStart, Text.Length - selectionStart);
			var list = ChatManager.ParseMessage(next, Color.White);
			int width = 1;
			if (list.Count > 0 && (list[0].Text != list[0].TextOriginal || list[0].DeleteWhole)) width = list[0].TextOriginal.Length;

			Text = Text.Remove(selectionStart, width);
		}

		// ctrl - delete to next word
	}

	private void HandleCut()
	{
		if (selecting)
		{
			selecting = false;

			Clipboard = SelectedText;
			SelectedText = "";

			selectionStart = Math.Min(selectionStart, selectionEnd);
		}

		else
		{
			Clipboard = Text;
			Text = string.Empty;
			selectionStart = 0;
		}
	}

	private void HandlePaste()
	{
		if (selecting)
		{
			selecting = false;

			if (Settings.MaxLength != null && Text.Length + Clipboard.Length > Settings.MaxLength) return;

			SelectedText = Clipboard;
			selectionStart = Math.Min(selectionStart, selectionEnd) + Clipboard.Length;
		}

		else
		{
			Text = Text.Insert(selectionStart, Clipboard);

			selectionStart += Clipboard.Length;
		}
	}

	private void HandleCopy()
	{
		Clipboard = selecting ? SelectedText : Text;
	}

	private void HandleSelectAll()
	{
		selectionEnd = 0;
		selectionStart = Text.Length;
		selecting = true;
	}
	#endregion

	private void CalculateTextMetrics()
	{
		string text = Text ?? Settings.HintText;
		if (string.IsNullOrWhiteSpace(text)) text = "I";

		textSize = ChatManager.GetStringSize(Font.Value, text, Vector2.One);
		textSize.Y -= 8f;
		// if (SizeToText)
		// {
		// 	Width.Pixels = (int)textSize.X;
		// 	Height.Pixels = (int)textSize.Y;
		// }

		var hAlign = Settings.HorizontalAlignment;
		var vAlign = Settings.VerticalAlignment;

		if (hAlign == HorizontalAlignment.Left) textPosition.X = InnerDimensions.X;
		else if (hAlign == HorizontalAlignment.Center) textPosition.X = InnerDimensions.X + InnerDimensions.Width * 0.5f - textSize.X * 0.5f;
		else if (hAlign == HorizontalAlignment.Right) textPosition.X = InnerDimensions.X + InnerDimensions.Width - textSize.X;

		if (vAlign == VerticalAlignment.Top) textPosition.Y = InnerDimensions.Y;
		else if (vAlign == VerticalAlignment.Center) textPosition.Y = InnerDimensions.Y + InnerDimensions.Height * 0.5f - textSize.Y * 0.5f;
		else if (vAlign == VerticalAlignment.Bottom) textPosition.Y = InnerDimensions.Y + InnerDimensions.Height - textSize.Y;
	}

	public override void Recalculate()
	{
		base.Recalculate();
		CalculateTextMetrics();
	}

	protected override void Update(GameTime gameTime)
	{
		if (MouseInput.IsMouseDown(MouseButton.Left) && !IsMouseHovering)
		{
			selecting = false;
			Focused = false;
		}
	}

	protected override void Draw(SpriteBatch spriteBatch)
	{
		GraphicsDevice device = spriteBatch.GraphicsDevice;
		SamplerState sampler = SamplerState.LinearClamp;
		RasterizerState rasterizer = new RasterizerState
		{
			CullMode = CullMode.None,
			ScissorTestEnable = true
		};

		Rectangle original = device.ScissorRectangle;

		spriteBatch.End();

		Rectangle clippingRectangle = GetClippingRectangle(spriteBatch);
		Rectangle adjustedClippingRectangle = Rectangle.Intersect(clippingRectangle, device.ScissorRectangle);
		device.ScissorRectangle = adjustedClippingRectangle;

		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);

		if (string.IsNullOrWhiteSpace(Text) && !string.IsNullOrWhiteSpace(Settings.HintText) && !Focused)
			Utils.DrawBorderStringFourWay(spriteBatch, Font.Value, Settings.HintText, textPosition.X, textPosition.Y, new Color(150, 150, 150), Color.Black, Vector2.Zero);
		else
			DrawText(spriteBatch);

		spriteBatch.End();

		device.ScissorRectangle = original;

		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);

		if (IsMouseHovering) CustomCursor.CustomCursor.SetCursor(BaseLibrary.TexturePath + "UI/TextCursor", new Vector2(3.5f, 8.5f), false);
	}

	private void DrawText(SpriteBatch spriteBatch)
	{
		if (selecting)
		{
			float x = ChatManager.GetStringSize(Font.Value, Text.Substring(0, Math.Min(selectionStart, selectionEnd)), Vector2.One).X;
			float width = ChatManager.GetStringSize(Font.Value, Text.Substring(Math.Min(selectionStart, selectionEnd), Math.Abs(selectionStart - selectionEnd)), Vector2.One).X;

			spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(
				(int)(textPosition.X + x),
				(int)textPosition.Y,
				(int)width,
				20
			), Settings.SelectionColor);
		}

		//Utils.DrawBorderStringFourWay(spriteBatch, Utility.Font, Text, textPosition.X, textPosition.Y, Color.White, Color.Black, Vector2.Zero);
		ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Font.Value, Text, textPosition, Color.White, 0f, Vector2.Zero, Vector2.One);

		if (++caretTimer > 30)
		{
			caretVisible = !caretVisible;
			caretTimer = 0;
		}

		if (caretVisible && Focused)
		{
			float size = ChatManager.GetStringSize(Font.Value, Text.Substring(0, selectionStart), Vector2.One).X;
			spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)(textPosition.X + size) + 1, (int)textPosition.Y, 1, 20), Settings.CaretColor);
			spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)(textPosition.X + size) + 2, (int)textPosition.Y, 1, 20), Settings.CaretColor * 0.25f);
		}
	}
}