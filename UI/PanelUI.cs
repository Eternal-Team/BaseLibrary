using BaseLibrary.UI.Elements;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace BaseLibrary.UI
{
	public class PanelUI : BaseUI
	{
		private static Dictionary<Type, Type> UICache;
		public static PanelUI Instance;

		public PanelUI()
		{
			UICache = new Dictionary<Type, Type>();
			Instance = this;

			foreach (Type type in ModLoader.Mods.Where(mod => mod.Name != "ModLoader").SelectMany(mod => mod.Code?.GetTypes()))
			{
				if (type.IsSubclassOfRawGeneric(typeof(BaseUIPanel<>)) && type.BaseType != null && type.BaseType.GenericTypeArguments.Length > 0) UICache[type.BaseType.GenericTypeArguments[0]] = type;
			}
		}

		public override void OnInitialize()
		{
		}

		public void HandleUI(IHasUI entity)
		{
			if (Main.netMode == NetmodeID.Server || entity == null) return;

			if (entity.UI != null) CloseUI(entity);
			else
			{
				if (!BaseLibrary.ClosedUICache.Contains(entity)) OpenUI(entity);

				if (!Main.playerInventory) Main.playerInventory = true;
			}
		}

		public void CloseUI(IHasUI entity)
		{
			if (Main.netMode == NetmodeID.Server || entity == null) return;

			BaseElement element = entity.UI;
			if (element == null) return;

			Main.LocalPlayer.GetModPlayer<BLPlayer>().UIPositions[entity.UUID] = element.Position / Size;
			element.Deactivate();
			RemoveChild(element);
			entity.UI = null;

			Main.PlaySound(entity.CloseSound);
		}

		public void OpenUI(IHasUI entity)
		{
			if (Main.netMode == NetmodeID.Server || entity == null) return;

			Type entityType = UICache.ContainsKey(entity.GetType()) ? entity.GetType() : entity.GetType().BaseType;

			entity.UI = (BaseUIPanel)Activator.CreateInstance(UICache[entityType]);
			entity.UI.Container = entity;

			entity.UI.Activate();

			if (Main.LocalPlayer.GetModPlayer<BLPlayer>().UIPositions.TryGetValue(entity.UUID, out Vector2 position))
			{
				entity.UI.HAlign = entity.UI.VAlign = 0;
				entity.UI.Position = position * Dimensions.Size();
			}

			entity.UI.OnMouseDown += (evt, element) =>
			{
				if (entity.UI == null) return;

				RemoveChild(entity.UI);
				Append(entity.UI);
			};

			Append(entity.UI);

			Main.PlaySound(entity.OpenSound);
		}

		public void CloseAllUIs()
		{
			for (int i = 0; i < Elements.Count; i++)
			{
				UIElement element = Elements[i];
				if (element is BaseUIPanel panel)
				{
					Main.LocalPlayer.GetModPlayer<BLPlayer>().UIPositions[panel.Container.UUID] = panel.Position / Size;
					element.Deactivate();
					panel.Container.UI = null;
				}
			}

			BaseLibrary.PanelGUI.UI.RemoveAllChildren();
		}
	}
}