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
		if (args.Key == Keys.F10 && !Main.drawingPlayerChat && !Main.editSign && !Main.editChest)
		{
			SoundEngine.PlaySound(SoundID.MenuTick);
			Main.showFrameRate = !Main.showFrameRate;
			return;
		}

		if (args.Key == Keys.Enter && !KeyboardUtil.AltDown(args.Modifiers) && Main.hasFocus)
		{
			if (!Main.drawingPlayerChat && !Main.editSign && !Main.editChest && !Main.gameMenu && !KeyboardInput.IsKeyDown(Keys.Escape))
			{
				SoundEngine.PlaySound(SoundID.MenuOpen);
				Main.OpenPlayerChat();
				Main.chatText = "";
			}

			return;
		}

		if (args.Key == Keys.Enter && KeyboardUtil.AltDown(args.Modifiers) && Main.hasFocus)
		{
			Main.ToggleFullScreen();
			Main.chatRelease = false;

			return;
		}

		if (args.Key == Keys.F11)
		{
			Main.hideUI = !Main.hideUI;

			SoundEngine.PlaySound(SoundID.MenuTick);
				
			return;
		}

		if (args.Key == Keys.F7 && !Main.drawingPlayerChat && !Main.editSign && !Main.editChest)
		{
			SoundEngine.PlaySound(SoundID.MenuTick);
			if (KeyboardUtil.AltDown(args.Modifiers))
				TimeLogger.Start();
			else if (Main.drawDiag)
				Main.drawDiag = false;
			else
				Main.drawDiag = true;
				
			return;
		}

		if (args.Key == Keys.F8 && !Main.drawingPlayerChat && !Main.editSign && !Main.editChest)
		{
			SoundEngine.PlaySound(SoundID.MenuTick);
			Main.shouldDrawNetDiagnosticsUI = !Main.shouldDrawNetDiagnosticsUI;
				
			return;
		}
			
		if (args.Key == Keys.F9 && KeyboardUtil.ShiftDown(args.Modifiers) && !Main.drawingPlayerChat && !Main.editSign && !Main.editChest)
		{
			SoundEngine.PlaySound(SoundID.MenuTick);
			Lighting.NextLightMode();
				
			return;
		}
			
		if (args.Key == Keys.F10 && !Main.drawingPlayerChat && !Main.editSign && !Main.editChest)
		{
			SoundEngine.PlaySound(SoundID.MenuTick);
			Main.showFrameRate = !Main.showFrameRate;
				
			return;
		}

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