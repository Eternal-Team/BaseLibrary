using BaseLibrary.Input;
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
	public static string PlaceholderTexture = "BaseLibrary/Assets/Textures/Placeholder";

	public override void Load()
	{
		Hooking.Load();

		if (!Main.dedServ)
		{
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