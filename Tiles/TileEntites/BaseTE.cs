using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BaseLibrary.Tiles.TileEntites
{
	public class BaseTE : ModTileEntity
	{
		public override bool ValidTile(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			return tile.active() && ValidTile(tile);
		}

		public virtual bool ValidTile(Tile tile) => true;

		public virtual void OnPlace()
		{
		}

		public override void OnNetPlace() => OnPlace();

		public new int Place(int i, int j)
		{
			ModTileEntity modTileEntity = ConstructFromBase(this);
			modTileEntity.Position = new Point16(i, j);
			modTileEntity.ID = AssignNewID();
			modTileEntity.type = (byte)Type;
			ByID[modTileEntity.ID] = modTileEntity;
			ByPosition[modTileEntity.Position] = modTileEntity;
			((BaseTE)modTileEntity).OnPlace();
			return modTileEntity.ID;
		}
	}
}