using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Starbound.Input;
using Terraria;
using Terraria.ModLoader;

namespace BaseLibrary
{
	public static partial class Utility
	{
		public static class Input
		{
			public static event Func<bool> InterceptMouse = () => false;
			public static event Func<bool> InterceptKeyboard = () => false;

			internal static MouseEvents MouseHandler;
			internal static KeyboardEvents KeyboardHandler;

			internal static void Load()
			{
				KeyboardEvents.RepeatDelay = 31;

				MouseHandler = new MouseEvents(Main.instance);
				Main.instance.Components.Add(MouseHandler);

				KeyboardHandler = new KeyboardEvents(Main.instance);
				Main.instance.Components.Add(KeyboardHandler);
			}

			internal static void Unload()
			{
				Main.instance.Components.Remove(MouseHandler);
				Main.instance.Components.Remove(KeyboardHandler);
			}

			internal static void Update()
			{
				MouseHandler.Enabled = InterceptMouse();
				KeyboardHandler.Enabled = InterceptKeyboard();
			}
		}

		private static Dictionary<string, ModHotKey> _hotkeys;

		public static Dictionary<string, ModHotKey> Hotkeys => _hotkeys ?? (_hotkeys = typeof(ModLoader).GetValue<Dictionary<string, ModHotKey>>("modHotKeys"));

		public static ModHotKey RegisterHotKey(this Mod mod, string name, Keys key) => mod.RegisterHotKey(name, key.ToString());

		public static string GetHotkeyValue(string hotkey)
		{
			if (string.IsNullOrWhiteSpace(hotkey) || Hotkeys == null) throw new ArgumentNullException();
			if (!Hotkeys.ContainsKey(hotkey)) throw new Exception("Hotkey doesn't exist");

			return Hotkeys[hotkey].GetAssignedKeys().Count > 0 ? Hotkeys[hotkey].GetAssignedKeys().First() : "Unassigned";
		}
	}
}