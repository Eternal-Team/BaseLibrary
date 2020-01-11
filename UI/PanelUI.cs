using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace BaseLibrary.UI
{
	public interface IHasUI
	{
		Guid UUID { get; set; }

		BaseUIPanel UI { get; set; }

		LegacySoundStyle CloseSound { get; }

		LegacySoundStyle OpenSound { get; }
	}

	public class BaseUIPanel : UIDraggablePanel
	{
		public IHasUI Container;

		public BaseUIPanel(IHasUI container) => Container = container;
	}

	public class BaseUIPanel<T> : BaseUIPanel where T : IHasUI
	{
		public new T Container => (T)base.Container;

		public BaseUIPanel(T container) : base(container)
		{
		}
	}

	public class PanelUI : BaseState
	{
		public static PanelUI Instance;

		private static Dictionary<Type, Type> UICache;

		public override bool Enabled => !Main.ingameOptionsWindow && !Main.gameMenu;

		public PanelUI()
		{
			if (Instance != null) throw new Exception("PanelUI already created");

			Instance = this;

			Width.Percent = 100;
			Height.Percent = 100;

			UICache = new Dictionary<Type, Type>();

			foreach (Type type in ModLoader.Mods.Where(mod => mod.Name != "ModLoader").SelectMany(mod => mod.Code?.GetTypes()))
			{
				if (type.IsSubclassOfRawGeneric(typeof(BaseUIPanel<>)) && type.BaseType != null && type.BaseType.GenericTypeArguments.Length > 0) UICache[type.BaseType.GenericTypeArguments[0]] = type;
			}
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

			Main.LocalPlayer.GetModPlayer<BLPlayer>().UIPositions[entity.UUID] = new Vector2(element.Dimensions.X / (float)Dimensions.Width, element.Dimensions.Y / (float)Dimensions.Height);
			element.InternalDeactivate();

			Remove(element);
			entity.UI = null;

			Main.PlaySound(entity.CloseSound);
		}

		public void OpenUI(IHasUI entity)
		{
			if (Main.netMode == NetmodeID.Server || entity == null) return;

			Type entityType = UICache.ContainsKey(entity.GetType()) ? entity.GetType() : entity.GetType().BaseType;

			entity.UI = (BaseUIPanel)Activator.CreateInstance(UICache[entityType], entity);
			entity.UI.InternalActivate();

			if (Main.LocalPlayer.GetModPlayer<BLPlayer>().UIPositions.TryGetValue(entity.UUID, out Vector2 position))
			{
				entity.UI.X = new StyleDimension { Pixels = (int)(position.X * Dimensions.Width) };
				entity.UI.Y = new StyleDimension { Pixels = (int)(position.Y * Dimensions.Height) };
			}
			else
			{
				entity.UI.X.Percent = 50;
				entity.UI.Y.Percent = 50;
			}

			entity.UI.OnMouseDown += args =>
			{
				if (entity.UI == null) return;

				Remove(entity.UI);
				Add(entity.UI);
			};

			Add(entity.UI);

			Main.PlaySound(entity.OpenSound);
		}

		public void CloseAllUIs()
		{
			for (int i = 0; i < Children.Count; i++)
			{
				BaseElement element = Children[i];
				if (element is BaseUIPanel panel)
				{
					Main.LocalPlayer.GetModPlayer<BLPlayer>().UIPositions[panel.Container.UUID] = new Vector2(element.X.Percent, element.Y.Percent);
					panel.Container.UI = null;
				}
			}

			Clear();
		}

		public override BaseElement GetElementAt(Vector2 point)
		{
			BaseElement element = Children.FirstOrDefault(current => current.ContainsPoint(point) && current.Display != Display.None);

			return element?.GetElementAt(point);
		}
	}
}