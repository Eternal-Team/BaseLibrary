using BaseLibrary.Input;
using Terraria;
using Terraria.ModLoader;

// TODO: UI framework

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
}