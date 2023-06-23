using System.Reflection;
using BaseLibrary.Utility;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace BaseLibrary;

public interface TileEntityOf<T>
{
}

public abstract class BaseTileEntity : ModTileEntity
{
	private static MethodInfo? TileTypeMethod;

	public override bool IsTileValidForEntity(int x, int y)
	{
		TileTypeMethod ??= typeof(ModContent).GetMethod("TileType", ReflectionUtility.DefaultFlags_Static);

		Tile tile = Main.tile[x, y];

		int tiletype = (int)TileTypeMethod.MakeGenericMethod(GetType().GetInterface("TileEntityOf`1").GenericTypeArguments[0]).Invoke(null, null);

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
		int ID = base.Place(i, j);
		OnPlace();
		return ID;
	}
}