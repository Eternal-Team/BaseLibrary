using IL.Terraria;
using IL.Terraria.UI;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using System;
using Terraria.ModLoader;

namespace BaseLibrary
{
	public static partial class Hooking
	{
		public static bool InUI;

		internal static void Load()
		{
			On.Terraria.UI.UIElement.GetElementAt += UIElement_GetElementAt;

			ItemSlot.OverrideHover += ItemSlot_OverrideHover;
			Main.DrawInterface_36_Cursor += Main_DrawInterface_36_Cursor;

			Type uiLoadMods = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.UI.UILoadMods");
			HookEndpointManager.Modify(uiLoadMods.GetMethod("OnDeactivate", Utility.defaultFlags), new Action<ILContext>(OnLoadedMods));

			Scheduler.EnqueueMessage(() => Terraria.Main.OnPostDraw += OnPostDraw);

			Initialize();
		}
	}
}