using BaseLibrary.Tiles;
using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace BaseLibrary
{
	public class BLPlayer : ModPlayer
	{
		public Dictionary<Guid, Vector2> UIPositions;

		public override void Initialize()
		{
			UIPositions = new Dictionary<Guid, Vector2>();
		}

		public override void PostUpdate()
		{
			if (player.mouseInterface) return;

			if (player.GetHeldItem().IsAir && Main.mouseLeft && Main.mouseLeftRelease)
			{
				int type = Main.tile[Player.tileTargetX, Player.tileTargetY].type;
				ModTile modTile = TileLoader.GetTile(type);
				if (modTile != null && modTile is BaseTile baseTile) baseTile.LeftClick(Player.tileTargetX, Player.tileTargetY);
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
			Hooking.ClosedUICache.Clear();
			BaseLibrary.PanelGUI.UI.CloseAllUIs();
		}
	}
}