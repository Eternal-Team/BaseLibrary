using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Starbound.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace BaseLibrary
{
	public static partial class Utility
	{
		public static class Input
		{
			internal static MouseEvents MouseHandler;
			internal static KeyboardEvents KeyboardHandler;

			public static Func<bool> InterceptKeyboard = () => false;
			public static Func<bool> InterceptMouse = () => false;

			internal static void Load()
			{
				if (Main.dedServ) return;

				KeyboardEvents.RepeatDelay = 31;

				MouseHandler = new MouseEvents(Main.instance);
				Main.instance.Components.Add(MouseHandler);

				KeyboardHandler = new KeyboardEvents(Main.instance);
				Main.instance.Components.Add(KeyboardHandler);
			}

			internal static void Unload()
			{
				if (Main.dedServ) return;

				Main.instance.Components.Remove(MouseHandler);
				Main.instance.Components.Remove(KeyboardHandler);
			}

			internal static void Update(GameTime time)
			{
				if (Main.dedServ) return;

				if (InterceptKeyboard.GetInvocationList().Any(del => (bool)del.DynamicInvoke()))
				{
					KeyboardHandler.Enabled = true;
					KeyboardHandler.Update(time);
				}
				else KeyboardHandler.Enabled = false;

				if (InterceptMouse.GetInvocationList().Any(del => (bool)del.DynamicInvoke()))
				{
					MouseHandler.Enabled = true;
					MouseHandler.Update(time);
				}
				else MouseHandler.Enabled = false;
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