using Terraria.ModLoader;

namespace BaseLibrary.Items
{
	public abstract class BaseItem : ModItem
	{
		public override bool CloneNewInstances => true;

		public override ModItem Clone()
		{
			BaseItem clone = (BaseItem)MemberwiseClone();
			return clone;
		}

		public override string Texture => "BaseLibrary/Textures/Placeholder";
	}
}