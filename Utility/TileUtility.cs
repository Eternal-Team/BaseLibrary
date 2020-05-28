using Microsoft.Xna.Framework;
using System.Collections.Generic;
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
		public static IEnumerable<Point> GetCircle(int centerX, int centerY, int radius)
		{
			int x = radius, y = 0;

			yield return new Point(x + centerX, centerY);

			if (radius > 0)
			{
				yield return new Point(-x + centerX, centerY);
				yield return new Point(centerX, x + centerY);
				yield return new Point(centerX, -x + centerY);
			}

			int P = 1 - radius;
			while (x > y)
			{
				y++;

				if (P <= 0) P = P + 2 * y + 1;
				else
				{
					x--;
					P = P + 2 * y - 2 * x + 1;
				}

				if (x < y) break;

				yield return new Point(x + centerX, y + centerY);
				yield return new Point(-x + centerX, y + centerY);
				yield return new Point(x + centerX, -y + centerY);
				yield return new Point(-x + centerX, -y + centerY);

				if (x != y)
				{
					yield return new Point(y + centerX, x + centerY);
					yield return new Point(-y + centerX, x + centerY);
					yield return new Point(y + centerX, -x + centerY);
					yield return new Point(-y + centerX, -x + centerY);
				}
			}
		}
		
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
						int size = 16 + data.CoordinatePadding;
						
						fX = tile.frameX % (size * data.Width) / size;
						fY = tile.frameY % (size * data.Height) / size;
					}
				}

				return new Point16(position.X - fX, position.Y - fY);
			}

			return Point16.NegativeOne;
		}

		public static bool InWorldBounds(int i, int j) => i >= 0 && i < Main.maxTilesX && j >= 0 && j < Main.maxTilesY;

		public static T GetTileEntity<T>(int i, int j) where T : ModTileEntity => GetTileEntity<T>(new Point16(i, j));

		public static T GetTileEntity<T>(Point16 position) where T : ModTileEntity
		{
			if (TileEntity.ByPosition.TryGetValue(TileTopLeft(position), out TileEntity te)) return (T)te;
			return null;
		}

		public static bool TryGetTileEntity<T>(Point16 position, out T tileEntity) where T : ModTileEntity
		{
			if (TileEntity.ByPosition.TryGetValue(TileTopLeft(position), out TileEntity te))
			{
				tileEntity = (T)te;
				return true;
			}

			tileEntity = null;
			return false;
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