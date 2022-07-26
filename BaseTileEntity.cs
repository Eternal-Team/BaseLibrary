using System;
using System.Reflection;
using BaseLibrary.Utility;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace BaseLibrary;

// [AttributeUsage(AttributeTargets.Class)]
// public class TileEntityOf<T> : Attribute
// {
// }

public abstract class BaseTileEntity : ModTileEntity
{
	protected abstract Type TileType { get; }

	private static MethodInfo TileTypeMethod;

	public override bool IsTileValidForEntity(int x, int y)
	{
		TileTypeMethod ??= typeof(ModContent).GetMethod("TileType", ReflectionUtility.DefaultFlags_Static);

		Tile tile = Main.tile[x, y];
		// var att = GetType().GetCustomAttribute(typeof(TileEntityOf<>));
		// if (att is null)
		// {
		// 	throw new Exception("TileEntity lacks TileEntityOf attribute");
		// }

		int tiletype = (int)TileTypeMethod.MakeGenericMethod(TileType).Invoke(null, null);

		return tile.HasTile && tile.TileType == tiletype && tile.IsTopLeft();
	}

	public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
	{
		TileObjectData data = TileObjectData.GetTileData(type, style);

		if (Main.netMode != NetmodeID.MultiplayerClient) return Place(i - data.Origin.X, j - data.Origin.Y);

		NetMessage.SendTileSquare(Main.myPlayer, i - data.Origin.X, j - data.Origin.Y, data.Width, data.Height);
		NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i - data.Origin.X, j - data.Origin.Y, Type);

		return -1;
	}

	public virtual void OnPlace()
	{
	}

	public override void OnNetPlace() => OnPlace();

	public new int Place(int i, int j)
	{
		ModTileEntity newEntity = ConstructFromBase(this);
		newEntity.Position = new Point16(i, j);
		newEntity.ID = AssignNewID();
		newEntity.type = (byte)Type;
		lock (EntityCreationLock)
		{
			ByID[newEntity.ID] = newEntity;
			ByPosition[newEntity.Position] = newEntity;
		}

		((BaseTileEntity)newEntity).OnPlace();

		return newEntity.ID;
	}
}