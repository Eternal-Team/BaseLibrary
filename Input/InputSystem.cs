using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;

namespace BaseLibrary.Input;

internal static class InputSystem
{
	public static LayerStack LayerStack;

	internal static void Load()
	{
		LayerStack = new LayerStack();
		LayerStack.PushLayer(new TerrariaLayer());

		MouseInput.Load();
		KeyboardInput.Load();
		On_PlayerInput.UpdateInput += On_PlayerInputOnUpdateInput;

		MouseInput.MouseMoved += args =>
		{
			foreach (Layer layer in LayerStack)
			{
				layer.OnMouseMove(args);
				if (args.Handled) break;
			}
		};

		MouseInput.ButtonPressed += args =>
		{
			foreach (Layer layer in LayerStack)
			{
				layer.OnMouseDown(args);
				if (args.Handled) break;
			}
		};

		MouseInput.ButtonReleased += args =>
		{
			foreach (Layer layer in LayerStack)
			{
				layer.OnMouseUp(args);
				if (args.Handled) break;
			}
		};

		MouseInput.ButtonClicked += args =>
		{
			foreach (Layer layer in LayerStack)
			{
				layer.OnClick(args);
				if (args.Handled) break;
			}
		};

		MouseInput.ButtonDoubleClicked += args =>
		{
			foreach (Layer layer in LayerStack)
			{
				layer.OnDoubleClick(args);
				if (args.Handled) break;
			}
		};

		MouseInput.ButtonTripleClicked += args =>
		{
			foreach (Layer layer in LayerStack)
			{
				layer.OnTripleClick(args);
				if (args.Handled) break;
			}
		};

		MouseInput.MouseScroll += args =>
		{
			foreach (Layer layer in LayerStack)
			{
				layer.OnMouseScroll(args);
				if (args.Handled) break;
			}
		};

		KeyboardInput.KeyPressed += args =>
		{
			foreach (Layer layer in LayerStack)
			{
				layer.OnKeyPressed(args);
				if (args.Handled) break;
			}
		};

		KeyboardInput.KeyReleased += args =>
		{
			foreach (Layer layer in LayerStack)
			{
				layer.OnKeyReleased(args);
				if (args.Handled) break;
			}
		};

		KeyboardInput.KeyTyped += args =>
		{
			foreach (Layer layer in LayerStack)
			{
				layer.OnKeyTyped(args);
				if (args.Handled) break;
			}
		};
	}

	private static readonly StaticField<bool> reinitialize = ReflectionUtility.GetField<PlayerInput, bool>("reinitialize");

	private static readonly StaticField<Action> OnActionableInput = ReflectionUtility.GetField<PlayerInput, Action>("OnActionableInput");
	private static readonly StaticMethod ReInitialize = ReflectionUtility.GetMethod<PlayerInput>("ReInitialize");
	private static readonly StaticMethod<bool> GamePadInput = ReflectionUtility.GetMethod<PlayerInput, bool>("GamePadInput");
	private static readonly StaticMethod PostInput = ReflectionUtility.GetMethod<PlayerInput>("PostInput");

	private static int oldScreenWidth;
	private static int oldScreenHeight;

	private static void On_PlayerInputOnUpdateInput(On_PlayerInput.orig_UpdateInput orig)
	{
		// Added by TML.
		if (reinitialize.GetValue())
			ReInitialize.Invoke();

		PlayerInput.SettingsForUI.UpdateCounters();

		PlayerInput.Triggers.Old.CloneFrom(PlayerInput.Triggers.Current);

		PlayerInput.ScrollWheelValueOld = 0;
		PlayerInput.ScrollWheelValue = 0;
		PlayerInput.ScrollWheelDeltaForUI = 0;
		PlayerInput.ScrollWheelDelta = 0;

		PlayerInput.GamepadThumbstickLeft = Vector2.Zero;
		PlayerInput.GamepadThumbstickRight = Vector2.Zero;
		PlayerInput.GrappleAndInteractAreShared = (PlayerInput.UsingGamepad || PlayerInput.SteamDeckIsUsed) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].DoGrappleAndInteractShareTheSameKey;
		if (PlayerInput.InBuildingMode && !PlayerInput.UsingGamepad)
			PlayerInput.ExitBuildingMode();

		// TODO: rebinding
		// if (_canReleaseRebindingLock.GetValue<bool>() && PlayerInput.NavigatorRebindingLock > 0)
		// {
		// 	PlayerInput.NavigatorRebindingLock--;
		// 	PlayerInput.Triggers.Current.UsedMovementKey = false;
		// 	if (PlayerInput.NavigatorRebindingLock == 0 && _memoOfLastPoint.GetValue<int>() != -1)
		// 	{
		// 		UIManageControls.ForceMoveTo = _memoOfLastPoint.GetValue<int>();
		// 		_memoOfLastPoint.SetValue(-1);
		// 	}
		// }
		//
		// _canReleaseRebindingLock.SetValue(true);

		PlayerInput.VerifyBuildingMode();

		MouseInput.Update(Main.gameTimeCache);
		KeyboardInput.Update(Main.gameTimeCache);

		if (oldScreenWidth != Main.screenWidth || oldScreenHeight != Main.screenHeight)
		{
			WindowResizedEventArgs args = new(new Vector2(Main.screenWidth, Main.screenHeight));

			foreach (Layer layer in LayerStack) layer.OnWindowResize(args);

			oldScreenWidth = Main.screenWidth;
			oldScreenHeight = Main.screenHeight;
		}

		bool keypadInput = GamePadInput.Invoke();
		PlayerInput.Triggers.Update();
		PostInput.Invoke();
		PlayerInput.WritingText = false;

		Main.mouseLeft = PlayerInput.Triggers.Current.MouseLeft;
		Main.mouseRight = PlayerInput.Triggers.Current.MouseRight;
		Main.mouseMiddle = PlayerInput.Triggers.Current.MouseMiddle;
		Main.mouseXButton1 = PlayerInput.Triggers.Current.MouseXButton1;
		Main.mouseXButton2 = PlayerInput.Triggers.Current.MouseXButton2;

		PlayerInput.CacheZoomableValues();
		if (keypadInput) OnActionableInput.GetValue()?.Invoke();
	}
}