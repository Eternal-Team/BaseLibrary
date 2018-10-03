//using BaseLibrary.Utility;
//using ContainerLibrary.Content;
//using System.Collections.Generic;
//using System.Linq;
//using Terraria;
//using Terraria.ID;
//using Terraria.ModLoader;

//namespace BaseLibrary.Items
//{
//    public class TestItem : BaseItem
//    {
//        public ItemStackHandler handler = new ItemStackHandler(5);

//        public override ModItem Clone()
//        {
//            TestItem clone = (TestItem)base.Clone();
//            clone.handler = new ItemStackHandler(handler.stacks.Select(x => x.Clone()).ToList());
//            return clone;
//        }

//        public override void ModifyTooltips(List<TooltipLine> tooltips)
//        {
//            tooltips.Add(new TooltipLine(mod, "test", handler.stacks.Select(x => x.ToString()).Aggregate()));
//        }

//        public override void OnCraft(Recipe recipe)
//        {
//            foreach (Item stack in handler.stacks)
//            {
//                stack.SetDefaults(Main.rand.Next(0, ItemLoader.ItemCount));
//            }
//        }

//        private int timer;
//        public override void UpdateInventory(Player player)
//        {
//            if (++timer > 60)
//            {
//                Item a = handler.ExtractItem(0, 1);
//                Item b = handler.ExtractItem(4, 1);
//                if (a != null || b != null)
//                {
//                    handler.InsertItem(0, b);
//                    handler.InsertItem(4, a);
//                }

//                timer = 0;
//            }
//        }

//        public override void AddRecipes()
//        {
//            ModRecipe recipe = new ModRecipe(mod);
//            recipe.AddIngredient(ItemID.DirtBlock);
//            recipe.SetResult(this);
//            recipe.AddRecipe();
//        }
//    }
//}

