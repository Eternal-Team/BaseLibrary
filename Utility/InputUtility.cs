using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ModLoader;

namespace BaseLibrary.Utility
{
	public static partial class Utility
	{
		public static ModHotKey Register(this Mod mod, string name, Keys key) => mod.RegisterHotKey(name, key.ToString());

		public static bool IsKeyDown(this Keys key) => Main.keyState.IsKeyDown(key);

		public static bool IsKeyDown(this int key) => IsKeyDown((Keys)key);

		public static bool IsKeyPressed(this Keys key) => Main.inputText.IsKeyDown(key) && !Main.oldInputText.IsKeyDown(key);

		public static string GetHotkeyValue(string hotkey)
		{
			Dictionary<string, ModHotKey> hotkeys = typeof(ModLoader).GetValue<Dictionary<string, ModHotKey>>("modHotKeys");
			return hotkeys != null && hotkeys.ContainsKey(hotkey) ? hotkeys[hotkey].GetAssignedKeys().Any() ? hotkeys[hotkey].GetAssignedKeys().First() : "Unassigned" : string.Empty;
		}
	}
}