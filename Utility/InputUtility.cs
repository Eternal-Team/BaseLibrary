using BaseLibrary.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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
			//internal static MouseEvents MouseHandler;
			internal static KeyboardEvents KeyboardHandler;

			public static Func<bool> InterceptKeyboard = () => false;
			public static Func<bool> InterceptMouse = () => false;

			public static bool LeftMouseDown;
			public static bool RightMouseDown;

			internal static void Load()
			{
				if (Main.dedServ) return;

				MouseEvents.Load();

				KeyboardEvents.RepeatDelay = 31;

				MouseEvents.ButtonPressed += args =>
				{
					switch (args.Button)
					{
						case MouseButton.Left:
							LeftMouseDown = true;
							break;
						case MouseButton.Right:
							RightMouseDown = true;
							break;
					}

					return false;
				};
				MouseEvents.ButtonReleased += args =>
				{
					switch (args.Button)
					{
						case MouseButton.Left:
							LeftMouseDown = false;
							break;
						case MouseButton.Right:
							RightMouseDown = false;
							break;
					}

					return false;
				};

				KeyboardHandler = new KeyboardEvents(Main.instance);
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

				MouseEvents.Update(time);
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