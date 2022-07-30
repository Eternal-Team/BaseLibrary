using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace BaseLibrary.Utility;

public static class TileUtility
{
	public static Vector2 ToScreenCoordinates(this Point point, bool addOffscreenRange = true) => point.ToVector2() * 16 - Main.screenPosition + (Main.drawToScreen || !addOffscreenRange ? Vector2.Zero : new Vector2(Main.offScreenRange));

	public static Vector2 ToScreenCoordinates(this Point16 point, bool addOffscreenRange = true) => point.ToVector2() * 16 - Main.screenPosition + (Main.drawToScreen || !addOffscreenRange ? Vector2.Zero : new Vector2(Main.offScreenRange));

	public static bool IsWithinWorldBounds(int i, int j) => i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY;

	public static TileObjectData? GetTileData(int i, int j)
	{
		if (!IsWithinWorldBounds(i, j) || !Main.tile[i, j].HasTile)
			return null;

		int style = 0;
		int alt = 0;
		Tile tile = Main.tile[i, j];

		TileObjectData.GetTileInfo(tile, ref style, ref alt);
		return TileObjectData.GetTileData(tile.TileType, style, alt);
	}

	public static bool IsTopLeft(this Tile tile)
	{
		int style = 0;
		int alt = 0;
		TileObjectData.GetTileInfo(tile, ref style, ref alt);
		TileObjectData data = TileObjectData.GetTileData(tile.TileType, style, alt);

		if (data != null) return tile.TileFrameX % (data.Width * 18) == 0 && tile.TileFrameY % (data.Height * 18) == 0;

		return true;
	}

	public static Point16? TileTopLeft(int i, int j)
	{
		if (!IsWithinWorldBounds(i, j) || !Main.tile[i, j].HasTile)
			return null;
		
		Tile tile = Main.tile[i, j];

		int fX = 0;
		int fY = 0;

		if (tile.HasTile)
		{
			TileObjectData data = TileObjectData.GetTileData(tile.TileType, 0);

			if (data != null)
			{
				int size = 16 + data.CoordinatePadding;

				fX = tile.TileFrameX % (size * data.Width) / size;
				fY = tile.TileFrameY % (size * data.Height) / size;
			}
		}

		return new Point16(i - fX, j - fY);
	}

	public static Point16? TileTopLeft(Point16 position) => TileTopLeft(position.X, position.Y);
}