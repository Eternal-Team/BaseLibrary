using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace BaseLibrary.Tiles.TileEntites
{
	public abstract class BaseTE : ModTileEntity
	{
		public abstract Type TileType { get; }

		public override bool ValidTile(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			return tile.active() && tile.type == mod.TileType(TileType.Name) && tile.IsTopLeft();
		}

		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
		{
			TileObjectData data = TileObjectData.GetTileData(type, style);

			if (Main.netMode != NetmodeID.MultiplayerClient) return Place(i - data.Origin.X, j - data.Origin.Y);

			NetMessage.SendTileSquare(Main.myPlayer, i - data.Origin.X, j - data.Origin.Y, Math.Max(data.Width, data.Height));
			NetMessage.SendData(MessageID.TileEntityPlacement, number: i - data.Origin.X, number2: j - data.Origin.Y, number3: Type);

			return -1;
		}

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