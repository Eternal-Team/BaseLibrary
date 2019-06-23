using BaseLibrary.UI.Elements;
using IL.Terraria;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.ModLoader;
using Terraria.UI;
using ItemSlot = IL.Terraria.UI.ItemSlot;

namespace BaseLibrary
{
	public static partial class Hooking
	{
		public static bool InUI;

		public static void Load()
		{
			On.Terraria.UI.UIElement.GetElementAt += UIElement_GetElementAt;

			ItemSlot.OverrideHover += ItemSlot_OverrideHover;
			Main.DrawInterface_36_Cursor += Main_DrawInterface_36_Cursor;

			Type uiLoadMods = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.UI.UILoadMods");
			HookEndpointManager.Modify(uiLoadMods.GetMethod("OnDeactivate", Utility.defaultFlags), new Action<ILContext>(OnLoadedMods));

			//OnPlayer.HandleHotbar += (orig, player) =>
			//{
			//    // todo: do just for scrollable elements

			//    if (!InUI) orig(player);
			//    InUI = false;
			//};

			Scheduler.EnqueueMessage(() =>
			{
				Terraria.Main.OnPostDraw += gameTime =>
				{
					if (Terraria.Main.menuMode == 4040 && newOrUpdated.Count > 0)
					{
						Terraria.Main.menuMode = 888;

						uiIntroMessage = new UIIntroMessage();
						UserInterface.ActiveInstance.SetState(uiIntroMessage);
					}
				};
			});

			#region thonk

			//Initialize();

			//OnUserInterface.Update += (orig, self, time) =>
			//{
			//    if (Main.gameMenu)
			//    {
			//        orig(self, time);
			//        return;
			//    }

			//    UserInterface.ActiveInstance.CurrentState?.Update(time);
			//};

			//OnPlayerInput.KeyboardInput += orig =>
			//{
			//    if (Utility.Input.KeyboardHandler.Enabled) orig();
			//    else
			//    {
			//        foreach (string key in PlayerInput.MouseKeys)
			//        {
			//            PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard].Processkey(PlayerInput.Triggers.Current, key);
			//        }
			//    }
			//};

			//MouseEvents.ButtonPressed += (sender, args) => { };

			//OnPlayerInput.MouseInput += orig =>
			//{
			//    bool changed = false;

			//    PlayerInput.MouseInfoOld = PlayerInput.MouseInfo;
			//    PlayerInput.MouseInfo = Mouse.GetState();

			//    if (PlayerInput.MouseInfoOld.X - PlayerInput.MouseInfo.X != 0 || PlayerInput.MouseY - PlayerInput.MouseInfo.Y != 0)
			//    {
			//        PlayerInput.MouseX = PlayerInput.MouseInfo.X;
			//        PlayerInput.MouseY = PlayerInput.MouseInfo.Y;
			//        changed = true;
			//    }

			//    PlayerInput.MouseKeys.Clear();
			//    if (!Utility.Input.InterceptMouseButton() && Main.instance.IsActive)
			//    {
			//        if (PlayerInput.MouseInfo.LeftButton == ButtonState.Pressed)
			//        {
			//            PlayerInput.MouseKeys.Add("Mouse1");
			//            changed = true;
			//        }

			//        if (PlayerInput.MouseInfo.RightButton == ButtonState.Pressed)
			//        {
			//            PlayerInput.MouseKeys.Add("Mouse2");
			//            changed = true;
			//        }

			//        if (PlayerInput.MouseInfo.MiddleButton == ButtonState.Pressed)
			//        {
			//            PlayerInput.MouseKeys.Add("Mouse3");
			//            changed = true;
			//        }

			//        if (PlayerInput.MouseInfo.XButton1 == ButtonState.Pressed)
			//        {
			//            PlayerInput.MouseKeys.Add("Mouse4");
			//            changed = true;
			//        }

			//        if (PlayerInput.MouseInfo.XButton2 == ButtonState.Pressed)
			//        {
			//            PlayerInput.MouseKeys.Add("Mouse5");
			//            changed = true;
			//        }
			//    }

			//    if (changed)
			//    {
			//        PlayerInput.CurrentInputMode = InputMode.Mouse;
			//        PlayerInput.Triggers.Current.UsedMovementKey = false;
			//    }
			//};

			//OnMain.DoUpdate_HandleInput += (orig, self) =>
			//{
			//    Utility.Input.Update();

			//    orig(self);

			//    if (!Utility.Input.KeyboardHandler.Enabled) Main.keyState = Main.oldKeyState;
			//}; 

			#endregion
		}

		private static bool openUI;

		private static UIIntroMessage uiIntroMessage;

		private static List<Mod> newOrUpdated = new List<Mod>();

		private static string LastVersionsPath = ModLoader.ModPath + "/LastVersionCache.json";

		private static void OnLoadedMods(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);

			cursor.EmitDelegate<Action>(() =>
			{
				Dictionary<string, Version> previousVersions = new Dictionary<string, Version>();
				if (File.Exists(LastVersionsPath)) previousVersions = JsonConvert.DeserializeObject<Dictionary<string, Version>>(File.ReadAllText(LastVersionsPath));

				newOrUpdated = ModLoader.Mods.Where(mod => previousVersions.ContainsKey(mod.Name) && previousVersions[mod.Name] != mod.Version || !previousVersions.ContainsKey(mod.Name)).ToList();
				Terraria.Main.menuMode = 4040;
				//File.WriteAllText(LastVersionsPath, JsonConvert.SerializeObject(ModLoader.Mods.Select(mod => new {Key = mod.Name, Value = mod.Version.ToString()}).ToDictionary(x => x.Key, x => x.Value)));
			});
		}

		public class UIIntroMessage : UIState
		{
			public override void OnInitialize()
			{
				UIPanel panel = new UIPanel
				{
					Width = (0, 0.25f),
					Height = (0, 0.5f)
				};
				panel.Center();
				Append(panel);

				UITexture texture = new UITexture(ModContent.GetTexture("BaseLibrary/Textures/UI/PatreonLogo"))
				{
					Width = (120, 0),
					Height = (120, 0),
					HAlign = 0.5f,
					VAlign = 0.25f
				};
				panel.Append(texture);

				UIText text = new UIText("Give us money!", 2f)
				{
					HAlign = 0.5f,
					VAlign = 0.65f
				};
				panel.Append(text);

				UITextButton button = new UITextButton("Back")
				{
					Width = (0, 1),
					Height = (60, 0),
					VAlign = 1
				};
				button.OnClick += (s, b) => Terraria.Main.menuMode = 0;
				panel.Append(button);
			}
		}
	}
}