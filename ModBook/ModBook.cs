//using System;
//using System.Collections.Generic;
//using Terraria.ModLoader;

//namespace BaseLibrary.ModBook
//{
//	public abstract class ModBook
//	{
//		public ModTranslation DisplayName { get; internal set; }
//		public string Name { get; internal set; }
//		public Mod Mod { get; internal set; }

//		public List<Category> Categories { get; set; } = new List<Category>();

//		// note: possibly move to ModBookLoader, add support for further images (in-text, header's, etc.)

//		public virtual void Initialize()
//		{
//		}

//		public void AddCategory(Category category)
//		{
//			if (Categories.Contains(category)) throw new Exception($"Book already contains category \"{category.Name}\"");

//			Categories.Add(category);
//			if (!ModBookLoader.textureCache.ContainsKey(category.Texture)) ModBookLoader.textureCache.Add(category.Texture, ModLoader.GetTexture(category.Texture));
//		}
//	}
//}