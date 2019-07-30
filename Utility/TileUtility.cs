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
		public static Point16 TileTopLeft(Point16 position)
		{
			if (position.X >= 0 && position.X <= Main.maxTilesX && position.Y >= 0 && position.Y <= Main.maxTilesY)
			{
				Tile tile = Main.tile[position.X, position.Y];

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

				return new Point16(position.X - fX, position.Y - fY);
			}

			return Point16.NegativeOne;
		}

		public static T GetTileEntity<T>(int i, int j) where T : ModTileEntity => GetTileEntity<T>(new Point16(i, j));

		public static T GetTileEntity<T>(Point16 position) where T : ModTileEntity
		{
			if (TileEntity.ByPosition.TryGetValue(TileTopLeft(position), out TileEntity te)) return (T)te;
			return null;
		}

		public static TileObjectDirection GetDirection(this Tile tile)
		{
			if (tile == null) return TileObjectDirection.None;

			int style = 0;
			int alt = 0;
			TileObjectData.GetTileInfo(tile, ref style, ref alt);
			return TileObjectData.GetTileData(tile.type, style, alt).Direction;
		}

		public static TileObjectDirection GetDirection(int i, int j) => Main.tile[i, j].GetDirection();

		public static TileObjectDirection GetDirection(Point16 position) => Main.tile[position.X, position.Y].GetDirection();

		public static bool IsChest(this Tile tile) => tile.type == TileID.Containers || TileLoader.ModChestName(tile.type).Length > 0;
	}
}