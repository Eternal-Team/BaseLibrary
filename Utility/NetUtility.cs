using System;
using System.IO;

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
}