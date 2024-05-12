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
	public override void Load()
	{
		Input.Input.Load();
		Hooking.Load();
	}
}