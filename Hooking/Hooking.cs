using BaseLibrary.UI;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using Newtonsoft.Json;
using On.Terraria.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Terraria;
using Terraria.ModLoader;
using Main = IL.Terraria.Main;
using Player = On.Terraria.Player;

namespace BaseLibrary
{
	public static partial class Hooking
	{
		public static bool InUI;

		internal static void Load()
		{
			ItemSlot.LeftClick_ItemArray_int_int += ItemSlot_LeftClick;
			Player.DropSelectedItem += Player_DropSelectedItem;
			UIElement.GetElementAt += UIElement_GetElementAt;

			Main.DrawInterface_36_Cursor += Main_DrawInterface_36_Cursor;

			do_worldGenCallBack_Hook += Hooking_do_worldGenCallBack_Hook;

			Type uiLoadMods = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.UI.DownloadManager.UILoadModsProgress");
			HookEndpointManager.Modify(uiLoadMods.GetMethod("OnDeactivate", Utility.defaultFlags), new Action<ILContext>(OnLoadedMods));

			Scheduler.EnqueueMessage(() => Terraria.Main.OnPostDraw += OnPostDraw);

			Initialize();
		}

		private static void Hooking_do_worldGenCallBack_Hook(orig_do_worldGenCallBack orig, CancellationToken token)
		{
			Dictionary<string, Version> previousVersions = new Dictionary<string, Version>();
			if (File.Exists(LastVersionsPath)) previousVersions = JsonConvert.DeserializeObject<Dictionary<string, Version>>(File.ReadAllText(LastVersionsPath));

			Type type = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.Core.ModOrganizer");
			object[] arr = type.InvokeMethod<object[]>("FindMods");

			newOrUpdated = ModLoader.Mods.Where(mod =>
			{
				object o = arr.FirstOrDefault(x => x.GetValue<string>("Name") == mod.Name);
				if (o != null && !o.GetValue<object>("properties").GetValue<string>("author").Contains("Itorius")) return false;

				return previousVersions.ContainsKey(mod.Name) && previousVersions[mod.Name] != mod.Version || !previousVersions.ContainsKey(mod.Name);
			}).ToList();
			if (newOrUpdated.Count > 0) Terraria.Main.menuMode = 4040;

			File.WriteAllText(LastVersionsPath, JsonConvert.SerializeObject(ModLoader.Mods.Select(mod => new {Key = mod.Name, Value = mod.Version.ToString()}).ToDictionary(x => x.Key, x => x.Value)));

			orig(token);
		}

		public delegate void orig_do_worldGenCallBack(CancellationToken token);

		public delegate void hook_do_worldGenCallBack(orig_do_worldGenCallBack orig, CancellationToken token);

		public static MethodBase method => typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.ModContent").GetMethod("SetupRecipes", Utility.defaultFlags, null, new[] {typeof(CancellationToken)}, null);

		public static event hook_do_worldGenCallBack do_worldGenCallBack_Hook
		{
			add => HookEndpointManager.Add(method, value);
			remove => HookEndpointManager.Remove(method, value);
		}

		internal static void Unload()
		{
			Uninitialize();

			Scheduler.EnqueueMessage(() => Terraria.Main.OnPostDraw -= OnPostDraw);
		}

		private static void ItemSlot_LeftClick(ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
		{
			if (inv[slot].modItem is IHasUI hasUI) BaseLibrary.PanelGUI.UI.CloseUI(hasUI);

			orig(inv, context, slot);
		}

		private static void Player_DropSelectedItem(Player.orig_DropSelectedItem orig, Terraria.Player self)
		{
			if (self.HeldItem.modItem is IHasUI hasUI) BaseLibrary.PanelGUI.UI.CloseUI(hasUI);

			orig(self);
		}
	}
}