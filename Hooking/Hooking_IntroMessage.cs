using BaseLibrary.UI;
using MonoMod.Cil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace BaseLibrary
{
	public static partial class Hooking
	{
		private static UIIntroMessage uiIntroMessage;

		private static List<Mod> newOrUpdated = new List<Mod>();

		private static string LastVersionsPath => ModLoader.ModPath + "/LastVersionCache.json";

		private static void ShowIntroMessage(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);
			if (cursor.TryGotoNext(i => i.MatchRet()))
			{
				cursor.EmitDelegate<Action>(() =>
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
					if (newOrUpdated.Count > 0)
					{
						Dispatcher.Dispatch(() =>
						{
							Main.menuMode = 888;

							uiIntroMessage = new UIIntroMessage();
							UserInterface.ActiveInstance.SetState(uiIntroMessage);
						});
					}

					File.WriteAllText(LastVersionsPath, JsonConvert.SerializeObject(ModLoader.Mods.Select(mod => new { Key = mod.Name, Value = mod.Version.ToString() }).ToDictionary(x => x.Key, x => x.Value)));
				});
			}
		}
	}
}