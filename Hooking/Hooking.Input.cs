using System.Collections.Generic;
using System.Reflection;
using BaseLibrary.Utility;
using On.Terraria.GameContent.UI.Elements;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI;

namespace BaseLibrary;

internal static partial class Hooking
{
	private static MethodInfo ReinitializeMethod;
	private static FieldInfo ReinitializeField;

	private static KeybindingLayer _rebindingLayer;
	private static FieldInfo InputModeField = typeof(Terraria.GameContent.UI.Elements.UIKeybindingListItem).GetField("_inputmode", ReflectionUtility.DefaultFlags);
	private static FieldInfo KeybindField = typeof(Terraria.GameContent.UI.Elements.UIKeybindingListItem).GetField("_keybind", ReflectionUtility.DefaultFlags);

	private static void PlayerInputOnUpdateInput(On.Terraria.GameInput.PlayerInput.orig_UpdateInput orig)
	{
		ReinitializeMethod ??= typeof(PlayerInput).GetMethod("ReInitialize", ReflectionUtility.DefaultFlags_Static);
		ReinitializeField ??= typeof(PlayerInput).GetField("reinitialize", ReflectionUtility.DefaultFlags_Static);

		if (ReinitializeField.GetValue<bool>(null))
			ReinitializeMethod!.Invoke(null, null);

		PlayerInput.Triggers.Old = PlayerInput.Triggers.Current.Clone();

		PlayerInput.VerifyBuildingMode();

		Input.Update(Main.gameTimeCache);

		Main.mouseLeft = PlayerInput.Triggers.Current.MouseLeft;
		Main.mouseRight = PlayerInput.Triggers.Current.MouseRight;
		Main.mouseMiddle = PlayerInput.Triggers.Current.MouseMiddle;
		Main.mouseXButton1 = PlayerInput.Triggers.Current.MouseXButton1;
		Main.mouseXButton2 = PlayerInput.Triggers.Current.MouseXButton2;

		PlayerInput.Triggers.Update();

		PlayerInput.WritingText = false;

		PlayerInput.CacheZoomableValues();
	}

	private class KeybindingLayer : Layer
	{
		private List<string> KeyStatus
		{
			get => PlayerInput.CurrentProfile.InputModes[inputMode].KeyStatus[keybind];
			set => PlayerInput.CurrentProfile.InputModes[inputMode].KeyStatus[keybind] = value;
		}

		private Terraria.GameContent.UI.Elements.UIKeybindingListItem item;
		
		public InputMode inputMode => InputModeField.GetValue<InputMode>(item);
		public string keybind => KeybindField.GetValue<string>(item);
		
		public KeybindingLayer(Terraria.GameContent.UI.Elements.UIKeybindingListItem item)
		{
			this.item = item;
		}

		public override void OnKeyPressed(KeyboardEventArgs args)
		{
			args.Handled = true;

			string newKey = args.Key.ToString();

			SoundEngine.PlaySound(SoundID.MenuTick);
			if (KeyStatus.Contains(newKey))
			{
				KeyStatus.Remove(newKey);
			}
			else
			{
				KeyStatus = new List<string>
				{
					newKey
				};
			}

			Input.Layers.PopOverlay(this);
			_rebindingLayer = null;
			PlayerInput.ListenFor(null, inputMode);
		}

		public override void OnMouseDown(MouseButtonEventArgs args)
		{
			args.Handled = true;

			string newKey = TerrariaLayer.NamedMouseToNumber(args.Button);

			SoundEngine.PlaySound(SoundID.MenuTick);
			if (KeyStatus.Contains(newKey))
			{
				KeyStatus.Remove(newKey);
			}
			else
			{
				KeyStatus = new List<string>
				{
					newKey
				};
			}

			Input.Layers.PopOverlay(this);
			_rebindingLayer = null;
			PlayerInput.ListenFor(null, inputMode);
		}
	}

	private static void UIKeybindingListItemOnOnClickMethod(UIKeybindingListItem.orig_OnClickMethod orig, Terraria.GameContent.UI.Elements.UIKeybindingListItem self, UIMouseEvent evt, UIElement listeningelement)
	{
		if (_rebindingLayer == null)
		{
			_rebindingLayer = new KeybindingLayer(self);
			Input.Layers.PushOverlay(_rebindingLayer);

			PlayerInput.ListenFor(_rebindingLayer.keybind, _rebindingLayer.inputMode);
		}
	}
}