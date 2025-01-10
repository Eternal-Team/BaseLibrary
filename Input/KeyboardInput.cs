using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;

namespace BaseLibrary.Input;

public static class KeyboardInput
{
	public static event Action<KeyboardEventArgs>? KeyPressed;
	public static event Action<KeyboardEventArgs>? KeyReleased;
	public static event Action<KeyboardEventArgs>? KeyTyped;

	private static bool isInitial;
	private static Keys lastKey;
	private static TimeSpan lastPress;
	private static KeyboardState currentKeyboardState;
	private static KeyboardState previousKeyboardState;

	private static int InitialDelay { get; set; }

	private static int RepeatDelay { get; set; }

	internal static void Load()
	{
		InitialDelay = 800;
		RepeatDelay = 31;
		currentKeyboardState = Keyboard.GetState();
	}

	public static bool IsKeyDown(Keys key) => currentKeyboardState.IsKeyDown(key);

	internal static void Update(GameTime gameTime)
	{
		if (!Main.instance.IsActive || !Main.hasFocus) return;

		previousKeyboardState = currentKeyboardState;
		currentKeyboardState = Keyboard.GetState();

		Modifiers modifiers = KeyboardUtil.GetModifiers(currentKeyboardState);

		foreach (Keys key in Enum.GetValues(typeof(Keys)))
		{
			if (!currentKeyboardState.IsKeyDown(key) || !previousKeyboardState.IsKeyUp(key)) continue;

			OnKeyPressed(new KeyboardEventArgs(modifiers, key, KeyboardUtil.ToChar(key, modifiers)));
			OnKeyTyped(new KeyboardEventArgs(modifiers, key, KeyboardUtil.ToChar(key, modifiers)));

			lastKey = key;
			lastPress = gameTime.TotalGameTime;
			isInitial = true;
		}

		foreach (Keys key in Enum.GetValues(typeof(Keys)))
		{
			if (currentKeyboardState.IsKeyUp(key) && previousKeyboardState.IsKeyDown(key))
			{
				OnKeyReleased(new KeyboardEventArgs(modifiers, key, KeyboardUtil.ToChar(key, modifiers)));
			}
		}

		double elapsedTime = (gameTime.TotalGameTime - lastPress).TotalMilliseconds;

		if (currentKeyboardState.IsKeyDown(lastKey) && ((isInitial && elapsedTime > InitialDelay) || (!isInitial && elapsedTime > RepeatDelay)))
		{
			OnKeyTyped(new KeyboardEventArgs(modifiers, lastKey, KeyboardUtil.ToChar(lastKey, modifiers)));
			lastPress = gameTime.TotalGameTime;
			isInitial = false;
		}
	}

	private static void OnKeyPressed(KeyboardEventArgs args) => KeyPressed?.Invoke(args);

	private static void OnKeyReleased(KeyboardEventArgs args) => KeyReleased?.Invoke(args);

	private static void OnKeyTyped(KeyboardEventArgs args) => KeyTyped?.Invoke(args);
}