using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace BaseLibrary
{
	public static partial class Utility
	{
		public static Point16 TileTopLeft(int i, int j)
		{
			if (i >= 0 && i <= Main.maxTilesX && j >= 0 && j <= Main.maxTilesY)
			{
				Tile tile = Main.tile[i, j];

				int fX = 0;
				int fY = 0;

				if (tile != null)
				{
					TileObjectData data = TileObjectData.GetTileData(tile.type, 0);
					if (data != null)
					{
						fX = tile.frameX % (18 * data.Width) / 18;
						fY = tile.frameY % (18 * data.Height) / 18;
					}
				}

				return new Point16(i - fX, j - fY);
			}

			return Point16.NegativeOne;
		}

		public static Point16 TileTopLeft(Point16 position) => TileTopLeft(position.X, position.Y);

		public static int Find(this ModTileEntity tileEntity, Point16 position) => tileEntity.Find(position.X, position.Y);

		public static int GetID<T>(this Mod mod, int i, int j) where T : ModTileEntity => mod.GetTileEntity<T>().Find(TileTopLeft(i, j));

		public static int GetID<T>(this Mod mod, Point16 position) where T : ModTileEntity => GetID<T>(mod, position.X, position.Y);

		public static T GetTileEntity<T>(this Mod mod, int i, int j) where T : ModTileEntity
		{
			// todo: could just use position instead, with trygetvalue
			int ID = mod.GetID<T>(i, j);
			if (ID >= 0) return (T)TileEntity.ByID[ID];
			return null;
		}

		public static T GetTileEntity<T>(this Mod mod, Point16 position) where T : ModTileEntity => mod.GetTileEntity<T>(position.X, position.Y);

		public static TileObjectDirection GetDirection(this Tile tile)
		{
			int style = 0;
			int alt = 0;
			TileObjectData.GetTileInfo(tile, ref style, ref alt);
			return TileObjectData.GetTileData(tile.type, style, alt).Direction;
		}

		public static TileObjectDirection GetDirection(int i, int j, int type) => Main.tile[i, j].GetDirection();

		public static TileObjectDirection GetDirection(Point16 position, int type) => Main.tile[position.X, position.Y].GetDirection();

		public static bool IsChest(this Tile tile) => tile.type == TileID.Containers || TileLoader.ModChestName(tile.type).Length > 0;
	}
}