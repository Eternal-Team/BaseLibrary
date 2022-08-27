using System;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;

namespace BaseLibrary;

public class TerrariaLayer : Layer
{
	private static KeyConfiguration KeyConfiguration
	{
		get
		{
			if (Main.gameMenu && !PlayerInput.WritingText) return PlayerInput.CurrentProfile.InputModes[InputMode.KeyboardUI];
			return PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard];
		}
	}

	public override void OnMouseMove(MouseMoveEventArgs args)
	{
		PlayerInput.MouseX = (int)(args.X * PlayerInput.RawMouseScale.X);
		PlayerInput.MouseY = (int)(args.Y * PlayerInput.RawMouseScale.Y);

		Main.lastMouseX = Main.mouseX;
		Main.lastMouseY = Main.mouseY;
		Main.mouseX = PlayerInput.MouseX;
		Main.mouseY = PlayerInput.MouseY;

		PlayerInput.CurrentInputMode = InputMode.Mouse;
		PlayerInput.Triggers.Current.UsedMovementKey = false;
	}

	internal static string NamedMouseToNumber(MouseButton button)
	{
		switch (button)
		{
			case MouseButton.Left:
				return "Mouse1";
			case MouseButton.Right:
				return "Mouse2";
			case MouseButton.Middle:
				return "Mouse3";
			case MouseButton.XButton1:
				return "Mouse4";
			case MouseButton.XButton2:
				return "Mouse5";
			default:
				throw new Exception("Unsupported mouse button " + button);
		}
	}

	public override void OnMouseDown(MouseButtonEventArgs args)
	{
		PlayerInput.CurrentInputMode = InputMode.Mouse;
		PlayerInput.Triggers.Current.UsedMovementKey = false;

		foreach (var item in KeyConfiguration.KeyStatus)
		{
			if (item.Value.Contains(NamedMouseToNumber(args.Button)))
				PlayerInput.Triggers.Current.KeyStatus[item.Key] = true;
		}
	}

	public override void OnMouseUp(MouseButtonEventArgs args)
	{
		foreach (var pair in KeyConfiguration.KeyStatus)
		{
			if (pair.Value.Contains(NamedMouseToNumber(args.Button)))
				PlayerInput.Triggers.Current.KeyStatus[pair.Key] = false;
		}
	}

	public override void OnMouseScroll(MouseScrollEventArgs args)
	{
		PlayerInput.ScrollWheelValue = (int)args.Y;
		PlayerInput.ScrollWheelDelta = (int)args.OffsetY;
		PlayerInput.ScrollWheelDeltaForUI = PlayerInput.ScrollWheelDelta;

		PlayerInput.CurrentInputMode = InputMode.Mouse;
		PlayerInput.Triggers.Current.UsedMovementKey = false;
	}

	public override void OnKeyPressed(KeyboardEventArgs args)
	{
		if (args.Key.ToString() == Main.blockKey)
		{
			Main.blockKey = Keys.None.ToString();
			return;
		}

		if (PlayerInput.WritingText)
			return;

		KeyConfiguration.Processkey(PlayerInput.Triggers.Current, args.Key.ToString(), InputMode.Keyboard);
	}

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