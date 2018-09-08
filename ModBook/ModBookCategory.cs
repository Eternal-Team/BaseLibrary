using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace BaseLibrary.ModBook
{
	public class Category
	{
		public Mod Mod { get; internal set; }
		public string Name { get; internal set; }
		public virtual string Texture { get; set; } = "Terraria/Item_149";

		public Texture2D GetTexture() => ModBookLoader.textureCache[Texture];
	}
}