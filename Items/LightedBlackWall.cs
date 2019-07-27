namespace BaseLibrary.Items
{
	public class LightedBlackWall : BaseItem
	{
		public override string Texture => "BaseLibrary/Textures/Item_2681";

		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Emits light.");
		}

		public override void SetDefaults()
		{
			item.width = 12;
			item.height = 12;
			item.maxStack = 999;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 7;
			item.useStyle = 1;
			item.consumable = true;
			item.createWall = mod.WallType<Walls.LightedBlackWall>();
		}
	}
}