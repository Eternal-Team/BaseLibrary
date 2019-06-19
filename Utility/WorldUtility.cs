using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ObjectData;

namespace BaseLibrary
{
	public static partial class Utility
	{
		public static Vector2 MouseToWorldVector => new Vector2((int)(Main.MouseWorld.X / 16), (int)(Main.MouseWorld.Y / 16));

		public static Point16 MouseToWorldPoint => new Point16((int)(Main.MouseWorld.X / 16), (int)(Main.MouseWorld.Y / 16));

		public static bool AnyTowerLiving => NPC.TowerActiveNebula || NPC.TowerActiveSolar || NPC.TowerActiveStardust || NPC.TowerActiveVortex;

		public static Rectangle GetRectangle(this Item item) => Main.itemAnimations[item.type] != null ? Main.itemAnimations[item.type].GetFrame(Main.itemTexture[item.type]) : Main.itemTexture[item.type].Frame(1, 1, 0, 0);

		public static bool IsTopLeft(this Tile tile)
		{
			int style = 0;
			int alt = 0;
			TileObjectData.GetTileInfo(tile, ref style, ref alt);
			TileObjectData data = TileObjectData.GetTileData(tile.type, style, alt);

			if (data != null) return tile.frameX % (data.Width * 18) == 0 && tile.frameY % (data.Height * 18) == 0;

			return true;
		}

		public static Entity GetEntityAtPos(this Vector2 position)
		{
			Point point = position.ToPoint();
			return Main.item.LastOrDefault(e => e.Hitbox.Contains(point)) ?? ((Entity)Main.npc.LastOrDefault(e => e.Hitbox.Contains(point)) ?? Main.projectile.LastOrDefault(e => e.Hitbox.Contains(point)));
		}

		#region Weather
		public static void StartRain() => typeof(Main).InvokeMethod<object>("StartRain");

		public static void StopRain() => typeof(Main).InvokeMethod<object>("StopRain");

		public static void StartSandstorm() => typeof(Sandstorm).InvokeMethod<object>("StartSandstorm");

		public static void StopSandstorm() => typeof(Sandstorm).InvokeMethod<object>("StopSandstorm");
		#endregion
	}
}