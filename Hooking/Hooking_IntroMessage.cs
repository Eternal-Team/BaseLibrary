using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.UI;

namespace BaseLibrary
{
	public static partial class Hooking
	{
		private static UIIntroMessage uiIntroMessage;

		private static List<Mod> newOrUpdated = new List<Mod>();

		private static string LastVersionsPath => ModLoader.ModPath + "/LastVersionCache.json";

		private static void OnLoadedMods(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);

			cursor.EmitDelegate<Action>(() =>
			{
				Dictionary<string, Version> previousVersions = new Dictionary<string, Version>();
				if (File.Exists(LastVersionsPath)) previousVersions = JsonConvert.DeserializeObject<Dictionary<string, Version>>(File.ReadAllText(LastVersionsPath));

				newOrUpdated = ModLoader.Mods.Where(mod =>
				{
					Type type = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.Core.BuildProperties");
					TmodFile file = mod.GetValue<TmodFile>("File");
					if (file != null)
					{
						object properties = type.InvokeMethod<object>("ReadModFile", null, file);
						string author = properties.GetValue<string>("author");
						if (!author.Contains("Itorius")) return false;
					}
					
					return previousVersions.ContainsKey(mod.Name) && previousVersions[mod.Name] != mod.Version || !previousVersions.ContainsKey(mod.Name);
				}).ToList();
				if (newOrUpdated.Count > 0) Main.menuMode = 4040;

				File.WriteAllText(LastVersionsPath, JsonConvert.SerializeObject(ModLoader.Mods.Select(mod => new {Key = mod.Name, Value = mod.Version.ToString()}).ToDictionary(x => x.Key, x => x.Value)));
			});
		}

		public static void OnPostDraw(GameTime gameTime)
		{
			if (Main.menuMode == 4040)
			{
				Main.menuMode = 888;

				uiIntroMessage = new UIIntroMessage();
				UserInterface.ActiveInstance.SetState(uiIntroMessage);
			}
		}
	}
}