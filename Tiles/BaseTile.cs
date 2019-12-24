using Terraria.ModLoader;

namespace BaseLibrary.Tiles
{
	public abstract class BaseTile : ModTile
	{
		public virtual string Texture => "BaseLibrary/Textures/Placeholder";

		public override bool Autoload(ref string name, ref string texture)
		{
			texture = Texture;
			return base.Autoload(ref name, ref texture);
		}

		public virtual void LeftClick(int i, int j)
		{
		}

		public virtual void RightClickCont(int i, int j)
		{
		}

		public virtual void LeftClickCont(int i, int j)
		{
		}
	}
}