using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoMod.Utils;
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

		public override TagCompound Save()
		{
			return new TagCompound
			{
				["UIPositions"] = UIPositions.Select(position => new TagCompound
				{
					["ID"] = position.Key.ToString(),
					["Position"] = position.Value
				}).ToList()
			};
		}

		public override void Load(TagCompound tag)
		{
			UIPositions = new Dictionary<Guid, Vector2>();
			UIPositions.AddRange(tag.GetList<TagCompound>("UIPositions").ToDictionary(c => Guid.Parse(c.Get<string>("ID")), c => c.Get<Vector2>("Position")));
		}
	}
}