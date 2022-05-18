using System.Reflection;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;

namespace BaseLibrary;

internal static partial class Hooking
{
	private static MethodInfo ReinitializeMethod;
	private static FieldInfo ReinitializeField;

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
}