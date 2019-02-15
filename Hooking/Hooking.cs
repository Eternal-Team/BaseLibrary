using On.Terraria;

namespace BaseLibrary.Hooking
{
	public static class Hooking
	{
		public static void Initialize()
		{
			//Main.do_Draw += DoDraw;
			//Main.DrawInterface += DrawInterface;
			//Main.EnsureRenderTargetContent += EnsureRenderTargetContent;

			Player.HandleHotbar += (orig, player) =>
			{
				if (!BaseLibrary.InUI) orig(player);
				BaseLibrary.InUI = false;
			};
		}
	}
}