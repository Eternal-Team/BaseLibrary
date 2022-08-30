using System;
using System.IO;
using Terraria.DataStructures;

namespace BaseLibrary.Utility;

public static class NetUtility
{
	public static void Write(this BinaryWriter writer, Guid id)
	{
		byte[] byteArray = id.ToByteArray();
		writer.Write(byteArray.Length);
		writer.Write(byteArray);
	}

	public static Guid ReadGuid(this BinaryReader reader)
	{
		return new Guid(reader.ReadBytes(reader.ReadInt32()));
	}
	
	public static void Write(this BinaryWriter writer, Point16 id)
	{
		writer.Write(id.X);
		writer.Write(id.Y);
	}

	public static Point16 ReadPoint16(this BinaryReader reader)
	{
		return new Point16(reader.ReadInt16(), reader.ReadInt16());
	}
}