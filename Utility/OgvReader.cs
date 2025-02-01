// Copyright (c) 2020-2024 Mirsario & Contributors.
// Released under the GNU General Public License 3.0.
// See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Media;
using ReLogic.Content;
using ReLogic.Content.Readers;
using ReLogic.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace BaseLibrary;

[Autoload(false)]
public sealed class OgvReader : IAssetReader, ILoadable
{
	private const BindingFlags ReflectionFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

	public static readonly string Extension = ".ogv";

	private static readonly Dictionary<IntPtr, UnmanagedMemoryStream> memoryStreams = new();

	// Stores delegates in heap so that they don't get eaten by the GC.
	private static readonly Theorafile.tf_callbacks callbacks = new() {
		read_func = ReadFunction,
		seek_func = SeekFunction,
		close_func = CloseFunction,
	};

	// Reflection
	private static readonly Type videoType = typeof(Video);
	private static readonly FieldInfo videoTheora = videoType.GetField("theora", ReflectionFlags)!;
	private static readonly FieldInfo videoYWidth = videoType.GetField("yWidth", ReflectionFlags)!;
	private static readonly FieldInfo videoYHeight = videoType.GetField("yHeight", ReflectionFlags)!;
	private static readonly FieldInfo videoUvWidth = videoType.GetField("uvWidth", ReflectionFlags)!;
	private static readonly FieldInfo videoUvHeight = videoType.GetField("uvHeight", ReflectionFlags)!;
	private static readonly FieldInfo videoFps = videoType.GetField("fps", ReflectionFlags)!;
	private static readonly FieldInfo videoNeedsDurationHack = videoType.GetField("needsDurationHack", ReflectionFlags)!;
	private static readonly PropertyInfo videoDuration = videoType.GetProperty(nameof(Video.Duration), ReflectionFlags)!;
	private static readonly PropertyInfo videoGraphicsDevice = videoType.GetProperty("GraphicsDevice", ReflectionFlags)!;

	public async ValueTask<T> FromStream<T>(Stream stream, MainThreadCreationContext mainThreadCtx) where T : class
	{
		if (typeof(T) != videoType)
		{
			throw AssetLoadException.FromInvalidReader<OgvReader, T>();
		}

		await mainThreadCtx;

		Video result = CreateVideo(stream);

		return (result as T)!;
	}

	private unsafe Video CreateVideo(Stream stream)
	{
		// This is created only to get a length without accessing stream.Length,
		// because 'stream' may be 'DeflateStream', and that doesn't implement it.
		// Could be avoided.
		using MemoryStream memoryStream = new MemoryStream();

		stream.CopyTo(memoryStream);

		int numBytes = (int)memoryStream.Position;
		nint dataPtr = Marshal.AllocHGlobal(numBytes);
		UnmanagedMemoryStream unmanagedStream = new UnmanagedMemoryStream((byte*)dataPtr, numBytes, numBytes, FileAccess.ReadWrite);

		memoryStream.Seek(0, SeekOrigin.Begin);
		memoryStream.CopyTo(unmanagedStream, numBytes);
		unmanagedStream.Seek(0L, SeekOrigin.Begin);

		// Keep track of streams and the data pointers.
		memoryStreams[dataPtr] = unmanagedStream;

		// Video's constructors are useless - they're internal, and take an OS file path rather than a data pointer.
		// Here we assemble the Video instance completely by ourselves.
		// Good thing that we no longer have to account for different frameworks being used.
		// - Mirsario.
		Video result = (Video)FormatterServices.GetUninitializedObject(videoType);

		int openResult = Theorafile.tf_open_callbacks(dataPtr, out nint theoraPtr, callbacks);

		if (openResult != 0)
		{
			throw new InvalidOperationException($"Theorafile returned code '{openResult}' when trying to load data.");
		}

		Theorafile.tf_videoinfo(theoraPtr, out int yWidth, out int yHeight, out double fps, out Theorafile.th_pixel_fmt fmt);

		int uvWidth;
		int uvHeight;

		switch (fmt)
		{
			case Theorafile.th_pixel_fmt.TH_PF_420:
				uvWidth = yWidth / 2;
				uvHeight = yHeight / 2;
				break;
			case Theorafile.th_pixel_fmt.TH_PF_422:
				uvWidth = yWidth / 2;
				uvHeight = yHeight;
				break;
			case Theorafile.th_pixel_fmt.TH_PF_444:
				uvWidth = yWidth;
				uvHeight = yHeight;
				break;
			default:
				throw new NotSupportedException("Unrecognized YUV format!");
		}

		videoGraphicsDevice.SetValue(result, Main.graphics.GraphicsDevice);
		videoTheora.SetValue(result, theoraPtr);
		videoYWidth.SetValue(result, yWidth);
		videoYHeight.SetValue(result, yHeight);
		videoUvWidth.SetValue(result, uvWidth);
		videoUvHeight.SetValue(result, uvHeight);
		videoFps.SetValue(result, fps);

		videoDuration.SetValue(result, TimeSpan.MaxValue);
		videoNeedsDurationHack.SetValue(result, true);

		return result;
	}

	void ILoadable.Load(Mod mod)
	{
		AssetReaderCollection? assetReaderCollection = Main.instance.Services.Get<AssetReaderCollection>();

		if (!assetReaderCollection.TryGetReader(Extension, out IAssetReader? reader) || reader != this)
		{
			assetReaderCollection.RegisterReader(this, Extension);
		}
	}

	void ILoadable.Unload()
	{
		AssetReaderCollection? assetReaderCollection = Main.instance.Services.Get<AssetReaderCollection>();

		if (assetReaderCollection.TryGetReader(Extension, out IAssetReader? reader) && reader == this)
		{
			RemoveExtension(assetReaderCollection, Extension);
		}
	}

	private static unsafe IntPtr ReadFunction(IntPtr ptr, IntPtr size, IntPtr nmemb, IntPtr dataSource)
	{
		if (!memoryStreams.TryGetValue(dataSource, out UnmanagedMemoryStream? stream))
		{
			return IntPtr.Zero;
		}

		int numBytes = (int)(nmemb * size);
		Span<byte> span = new Span<byte>((void*)ptr, numBytes);
		int numRead = stream.Read(span);

		return numRead;
	}

	private static int SeekFunction(IntPtr dataSource, long offset, Theorafile.SeekWhence whence)
	{
		if (!memoryStreams.TryGetValue(dataSource, out UnmanagedMemoryStream? stream))
		{
			return 0;
		}

		SeekOrigin seekOrigin = whence switch {
			Theorafile.SeekWhence.TF_SEEK_SET => SeekOrigin.Begin,
			Theorafile.SeekWhence.TF_SEEK_CUR => SeekOrigin.Current,
			Theorafile.SeekWhence.TF_SEEK_END => SeekOrigin.End,
			_ => throw new InvalidDataException($"{nameof(Theorafile.SeekWhence)} value made no sense"),
		};
		long newPosition = stream.Seek(offset, seekOrigin);

		return (int)newPosition;
	}

	private static int CloseFunction(IntPtr dataSource)
	{
		if (!memoryStreams.Remove(dataSource, out UnmanagedMemoryStream? stream))
		{
			return 0;
		}

		stream.Dispose();
		Marshal.FreeHGlobal(dataSource);

		return 1;
	}

	private static readonly FieldInfo? readersByExtensionField = typeof(AssetReaderCollection).GetField("_readersByExtension", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

	private static readonly FieldInfo? extensionsField = typeof(AssetReaderCollection).GetField("_extensions", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

	public static void RemoveExtension(AssetReaderCollection collection, string extension)
	{
		if (readersByExtensionField?.GetValue(collection) is not Dictionary<string, IAssetReader> dictionary || extensionsField?.GetValue(collection) is not string[])
		{
			// DebugSystem.Logger.Warn("Unable to remove a reader from the asset reader collection.");
			return;
		}

		// And then we hope that nothing explodes.
		dictionary.Remove(extension);
		extensionsField.SetValue(collection, dictionary.Keys.ToArray());
	}
}