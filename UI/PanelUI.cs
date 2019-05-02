using System;
using System.Collections.Generic;
using System.Linq;
using BaseLibrary.UI.Elements;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BaseLibrary.UI
{
	public class PanelUI : BaseUI
	{
		private static Dictionary<Type, Type> UICache;

		public PanelUI()
		{
			UICache = new Dictionary<Type, Type>();

			foreach (Type type in ModLoader.Mods.Where(mod => mod.Name != "ModLoader").SelectMany(mod => mod.Code?.GetTypes()))
			{
				if (type.IsSubclassOfRawGeneric(typeof(BaseUIPanel<>)) && type.BaseType != null && type.BaseType.GenericTypeArguments.Length > 0) UICache[type.BaseType.GenericTypeArguments[0]] = type;
			}
		}

		public override void OnInitialize()
		{
		}

		public void HandleUI(IHasUI bag)
		{
			if (bag.UI != null) CloseUI(bag);
			else
			{
				Main.playerInventory = true;

				if (BaseLibrary.Instance.ClosedUICache.Contains(bag)) BaseLibrary.Instance.ClosedUICache.Remove(bag);
				OpenUI(bag);
			}
		}

		public void CloseUI(IHasUI bag)
		{
			BaseElement element = bag.UI;
			if (element == null) return;
			//if (ModLoader.GetMod("ContainerLibrary") != null&&bag is IItemHandler)
			//{
			//	ModLoader.GetMod("ContainerLibrary")
			//}

			//ContainerLibrary.ContainerLibrary.ItemHandlerUI.Remove((IItemHandlerUI)element);

			Main.LocalPlayer.GetModPlayer<BLPlayer>().UIPositions[bag.ID] = element.Position;
			Elements.Remove(element);
			bag.UI = null;

			Main.PlaySound(bag.CloseSound);
		}

		public void OpenUI(IHasUI bag)
		{
			Type bagType = UICache.ContainsKey(bag.GetType()) ? bag.GetType() : bag.GetType().BaseType;

			bag.UI = (BaseUIPanel)Activator.CreateInstance(UICache[bagType]);
			bag.UI.Container = bag;

			bag.UI.Activate();

			if (Main.LocalPlayer.GetModPlayer<BLPlayer>().UIPositions.TryGetValue(bag.ID, out Vector2 position))
			{
				bag.UI.HAlign = bag.UI.VAlign = 0;
				bag.UI.Position = position;
			}

			Append(bag.UI);
			//ContainerLibrary.ContainerLibrary.ItemHandlerUI.Add((IItemHandlerUI)element);

			Main.PlaySound(bag.OpenSound);
		}
	}
}