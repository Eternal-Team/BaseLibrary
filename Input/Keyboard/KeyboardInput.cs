using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;

namespace BaseLibrary;

public class KeyboardInput
{
	private static List<KeySequence> keySequences;

	private static bool isInitial;
	private static Keys lastKey;

	private static TimeSpan lastPress;

	private static KeyboardState current;
	private static KeyboardState previous;

	public static int InitialDelay { get; set; }

	public static int RepeatDelay { get; set; }

	public static void AddKeySequence(KeySequence keySequence)
	{
		keySequences.Add(keySequence);
	}

	public static void RemoveKeySequence(KeySequence keySequence)
	{
		keySequences.Remove(keySequence);
	}

	public static void OnKeyPressed(KeyboardEventArgs args)
	{
		KeyPressed?.Invoke(args);
	}

	public static void OnKeyReleased(KeyboardEventArgs args)
	{
		KeyReleased?.Invoke(args);
	}

	public static void OnKeyTyped(KeyboardEventArgs args)
	{
		KeyTyped?.Invoke(args);
	}

	public static bool IsKeyDown(Keys key)
	{
		return current.IsKeyDown(key);
	}

	public static event Action<KeyboardEventArgs> KeyPressed;

	public static event Action<KeyboardEventArgs> KeyReleased;

	public static event Action<KeyboardEventArgs> KeyTyped;

	internal static void Load()
	{
		InitialDelay = 800;
		RepeatDelay = 31;
		keySequences = new List<KeySequence>();
		KeyTyped += KeyTypedHandler;
		current = Keyboard.GetState();
	}

	internal static void Update(GameTime gameTime)
	{
		if (!Main.instance.IsActive || !Main.hasFocus) return;

		previous = current;
		current = Keyboard.GetState();

		Modifiers modifiers = KeyboardUtil.GetModifiers(current);

		foreach (Keys key in Enum.GetValues(typeof(Keys)))
		{
			if (current.IsKeyDown(key) && previous.IsKeyUp(key))
			{
				OnKeyPressed(new KeyboardEventArgs(modifiers, key, KeyboardUtil.ToChar(key, modifiers)));
				OnKeyTyped(new KeyboardEventArgs(modifiers, key, KeyboardUtil.ToChar(key, modifiers)));

				lastKey = key;
				lastPress = gameTime.TotalGameTime;
				isInitial = true;
			}
		}

		foreach (Keys key in Enum.GetValues(typeof(Keys)))
		{
			if (current.IsKeyUp(key) && previous.IsKeyDown(key))
			{
				OnKeyReleased(new KeyboardEventArgs(modifiers, key, KeyboardUtil.ToChar(key, modifiers)));
			}
		}

		double elapsedTime = (gameTime.TotalGameTime - lastPress).TotalMilliseconds;

		if (current.IsKeyDown(lastKey) && ((isInitial && elapsedTime > InitialDelay) || (!isInitial && elapsedTime > RepeatDelay)))
		{
			OnKeyTyped(new KeyboardEventArgs(modifiers, lastKey, KeyboardUtil.ToChar(lastKey, modifiers)));
			lastPress = gameTime.TotalGameTime;
			isInitial = false;
		}
	}

	private static void KeyTypedHandler(KeyboardEventArgs args)
	{
		foreach (KeySequence sequence in keySequences) sequence.HandleKey(args.Key);
	}
}