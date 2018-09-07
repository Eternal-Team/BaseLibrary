using System.IO;
using System.IO.Compression;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace BaseLibrary.Utility
{
	public static partial class Utility
	{
		public static bool Contains(this Rectangle rectangle, Point16 point) => rectangle.Contains(point.X, point.Y);

		public static bool Contains(this Rectangle rectangle, Vector2 vector) => rectangle.Contains((int)vector.X, (int)vector.Y);

		public static byte[] CompressBytes(this byte[] data)
		{
			MemoryStream output = new MemoryStream();
			using (var deflateStream = new DeflateStream(output, CompressionLevel.Optimal, false)) deflateStream.Write(data, 0, data.Length);
			return output.ToArray();
		}

		public static byte[] DecompressBytes(this byte[] data)
		{
			MemoryStream input = new MemoryStream(data);
			MemoryStream output = new MemoryStream();
			using (var deflateStream = new DeflateStream(input, CompressionMode.Decompress)) deflateStream.CopyTo(output);
			return output.ToArray();
		}
	}
}