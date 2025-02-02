using BaseLibrary.Input;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Content.Sources;
using Terraria;
using Terraria.ModLoader;

// TODO: UI framework
// TODO: Make custom cursor lib part of BaseLibrary

namespace BaseLibrary;

public class Program
{
	public static void Main(string[] args)
	{
	}
}

public class BaseLibrary : Mod
{
	public const string PlaceholderTexture = "BaseLibrary/Assets/Textures/Placeholder";
	public static Asset<Texture2D> MissingTexture = null!;

	public override void Load()
	{
		Hooking.Load();

		if (!Main.dedServ)
		{
			MissingTexture = ModContent.Request<Texture2D>(PlaceholderTexture);

			InputSystem.Load();
		}
	}

	public override IContentSource CreateDefaultContentSource()
	{
		if (!Main.dedServ)
		{
			AddContent(new OgvReader()); // This manual ILoadable adds readers to AssetReaderCollections.
		}

		return base.CreateDefaultContentSource();
	}
}