using Terraria.ModLoader;

namespace BaseLibrary
{
	public static class Language
	{
		public static void Load()
		{
			ModTranslation translation = BaseLibrary.Instance.CreateTranslation("HotkeyUnassigned");
			translation.SetDefault("Unassigned");
			BaseLibrary.Instance.AddTranslation(translation);
		}
	}
}