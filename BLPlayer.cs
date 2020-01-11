using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BaseLibrary
{
	public class BLPlayer : ModPlayer
	{
		public Dictionary<Guid, Vector2> UIPositions;

		public override void Initialize()
		{
			UIPositions = new Dictionary<Guid, Vector2>();
		}

		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			if (BaseLibrary.hotkey.JustPressed)
			{
				Main.NewText("Just pressed");
			}

			if (BaseLibrary.hotkey.JustReleased)
			{
				Main.NewText("Just released");
			}

			if (BaseLibrary.hotkey.Current)
			{
				Main.NewText("Held down");
			}
		}

		public override TagCompound Save()
		{
			return new TagCompound
			{
				["UIPositions"] = UIPositions.Select(position => new TagCompound
				{
					["UUID"] = position.Key,
					["Position"] = position.Value
				}).ToList()
			};
		}

		public override void Load(TagCompound tag)
		{
			UIPositions = new Dictionary<Guid, Vector2>();
			UIPositions.AddRange(tag.GetList<TagCompound>("UIPositions").ToDictionary(c => c.Get<Guid>("UUID"), c => c.Get<Vector2>("Position")));
		}

		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			BaseLibrary.ClosedUICache.Clear();

			PanelUI.Instance.CloseAllUIs();
		}
	}
}