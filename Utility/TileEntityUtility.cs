using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BaseLibrary.Utility;

public static class TileEntityUtility
{
	public static bool TryGetTileEntity<T>(Point16 position, out T tileEntity) where T : ModTileEntity
	{
		if (TileEntity.ByPosition.TryGetValue(TileUtility.TileTopLeft(position), out TileEntity te))
		{
			tileEntity = (T)te;
			return true;
		}

		tileEntity = null;
		return false;
	}

	public static bool TryGetTileEntity<T>(int i, int j, out T tileEntity) where T : ModTileEntity => TryGetTileEntity(new Point16(i, j), out tileEntity);
}