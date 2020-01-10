using BaseLibrary.UI.Intro;
using Microsoft.Xna.Framework;
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

		private static void UserInterface_Draw(On.Terraria.UI.UserInterface.orig_Draw orig, UserInterface self, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, GameTime time)
		{
			orig(self, spriteBatch, time);

			if (Main.gameMenu && BaseLibrary.Layers != null)
			{
				foreach (Layer layer in BaseLibrary.Layers)
				{
					if (layer.Enabled) layer.OnDraw(Main.spriteBatch);
				}
			}
		}

		private static void UserInterface_Update(On.Terraria.UI.UserInterface.orig_Update orig, UserInterface self, GameTime time)
		{
			orig(self, time);

			if (Main.gameMenu && BaseLibrary.Layers != null)
			{
				foreach (Layer layer in BaseLibrary.Layers)
					if (layer.Enabled)
						layer.OnUpdate(time);
			}
		}

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

						// todo: setup server-side
						return true;

//#if DEBUG
//						return false;
//#elif RELEASE
//						return previousVersions.ContainsKey(mod.Name) && previousVersions[mod.Name] != mod.Version || !previousVersions.ContainsKey(mod.Name);
//#endif
					}).ToList();
					if (newOrUpdated.Count > 0 && Utility.PingHost("localhost", 59035))
					{
						Dispatcher.Dispatch(() =>
						{
							//Main.menuMode = 888;

							//uiIntroMessage = new UIIntroMessage();
							//uiIntroMessage.Recalculate();
							//(BaseLibrary.Layers.First(x => x is UILayer) as UILayer)?.Add(uiIntroMessage);
							//UserInterface.ActiveInstance.SetState(uiIntroMessage);
						});
					}

					File.WriteAllText(LastVersionsPath, JsonConvert.SerializeObject(ModLoader.Mods.Select(mod => new { Key = mod.Name, Value = mod.Version.ToString() }).ToDictionary(x => x.Key, x => x.Value)));
				});
			}
		}
	}
}