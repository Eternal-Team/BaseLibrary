using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BaseLibrary.Utility;

public static class TileEntityUtility
{
	public static bool TryGetTileEntity<T>(Point16 position, out T? tileEntity) where T : ModTileEntity
	{
		tileEntity = null;
		Point16? topLeft = TileUtility.TileTopLeft(position);

		if (!topLeft.HasValue || !TileEntity.ByPosition.TryGetValue(topLeft.Value, out TileEntity? te))
			return false;

		tileEntity = (T)te;
		return true;
	}

	public static bool TryGetTileEntity<T>(int i, int j, out T? tileEntity) where T : ModTileEntity => TryGetTileEntity(new Point16(i, j), out tileEntity);
}