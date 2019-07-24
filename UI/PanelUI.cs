using BaseLibrary.UI.Elements;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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

		public void HandleUI(IHasUI entity)
		{
			if (entity.UI != null) CloseUI(entity);
			else
			{
				Main.playerInventory = true;

				if (BaseLibrary.Instance.ClosedUICache.Contains(entity)) BaseLibrary.Instance.ClosedUICache.Remove(entity);
				OpenUI(entity);
			}
		}

		public void CloseUI(IHasUI entity)
		{
			BaseElement element = entity.UI;
			if (element == null) return;

			Main.LocalPlayer.GetModPlayer<BLPlayer>().UIPositions[entity.ID] = element.Position / Dimensions.Size();
			element.Deactivate();
			RemoveChild(element);
			entity.UI = null;

			Main.PlaySound(entity.CloseSound);
		}

		public void OpenUI(IHasUI entity)
		{
			Type entityType = UICache.ContainsKey(entity.GetType()) ? entity.GetType() : entity.GetType().BaseType;

			entity.UI = (BaseUIPanel)Activator.CreateInstance(UICache[entityType]);
			entity.UI.Container = entity;

			entity.UI.Activate();

			if (Main.LocalPlayer.GetModPlayer<BLPlayer>().UIPositions.TryGetValue(entity.ID, out Vector2 position))
			{
				entity.UI.HAlign = entity.UI.VAlign = 0;
				entity.UI.Position = position * Dimensions.Size();
			}

			entity.UI.OnMouseDown += (evt, element) =>
			{
				RemoveChild(entity.UI);
				Append(entity.UI);
			};

			Append(entity.UI);

			Main.PlaySound(entity.OpenSound);
		}
	}
}