using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace BaseLibrary.ModBook
{
	public class Category
	{
		public Mod Mod { get; internal set; }
		public string Name { get; set; }
		public virtual string Texture { get; set; } = "Terraria/Item_149";

		public List<Page> Pages { get; set; } = new List<Page>();

		public Texture2D GetTexture() => ModBookLoader.textureCache[Texture];

		public override string ToString() => $"Name: {Name}; Texture: {Texture}; Mod: {Mod.DisplayName}";
	}

	public class Page
	{
	}
}