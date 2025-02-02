using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace BaseLibrary.UI;

public interface IHasUI
{
	Guid GetID();

	// SoundStyle? GetOpenSound() => null;

	// SoundStyle? GetCloseSound() => null;
}

public class BaseUIPanel : UIPanel
{
	public IHasUI Container;

	public BaseUIPanel(IHasUI container)
	{
		Settings.Draggable = true;
		Container = container;
	}
}

public class BaseUIPanel<T>(T container) : BaseUIPanel(container) where T : IHasUI
{
	public new T Container => (T)base.Container;
}

public class PanelUISystem : ModSystem
{
	public override void PreSaveAndQuit()
	{
		PanelUI.Instance?.CloseAllUIs();
	}
}

public class PanelUI : BaseElement
{
	public static PanelUI? Instance;

	private Dictionary<Type, Type> EntityToUIMap = [];
	private Dictionary<Guid, BaseElement> Panels = [];
	private List<IHasUI> ClosedUICache = [];

	public PanelUI()
	{
		Instance = this;

		Size = Dimension.FromPercent(100);

		foreach (Mod mod in ModLoader.Mods.Where(mod => mod.Name != "ModLoader"))
		{
			Type[]? types = AssemblyManager.GetLoadableTypes(mod.Code);
			if (types is null) continue;

			foreach (Type type in types)
			{
				// if (ReflectionUtility.IsSubclassOfRawGeneric(type, typeof(BaseUIPanel<>)) && type.BaseType != null && type.BaseType.GenericTypeArguments.Length > 0)
				// EntityToUIMap[type.BaseType.GenericTypeArguments[0]] = type;
			}
		}
	}

	public void HandleUI(IHasUI? entity)
	{
		if (Main.netMode == NetmodeID.Server || entity is null) return;

		if (Panels.TryGetValue(entity.GetID(), out BaseElement? panel) && panel.Display == Display.Visible) CloseUI(entity);
		else
		{
			/*if (!ModContent.GetInstance<PanelUISystem>().ClosedUICache.Contains(entity))*/ OpenUI(entity);

			// if (!Main.playerInventory) Main.playerInventory = true;
		}
	}

	public void CloseUI(IHasUI? entity)
	{
		if (Main.netMode == NetmodeID.Server || entity is null) return;
		if (!Panels.TryGetValue(entity.GetID(), out BaseElement? panel) || panel.Display != Display.Visible) return;

		Remove(panel);
		Panels.Remove(entity.GetID());
		
		/*panel.Display = Display.None;
		panel.InternalDeactivate();

		SoundStyle? closeSound = entity.GetCloseSound();
		if (closeSound != null) SoundEngine.PlaySound(closeSound.Value);*/
	}

	public void OpenUI(IHasUI? entity)
	{
		if (Main.netMode == NetmodeID.Server || entity is null) return;

		if (Panels.ContainsKey(entity.GetID())) return;

		BaseUIPanel panel = new BaseUIPanel(entity) {
			Size = Dimension.FromPixels(200),
			Position = Dimension.FromPercent(50)
		};
		Add(panel);
		Panels.Add(entity.GetID(), panel);

		/*SoundStyle? openSound = entity.GetOpenSound();
		Guid id = entity.GetID();

		if (Panels.TryGetValue(id, out BaseUIPanel? ui) && ui.Display == Display.None)
		{
			ui.Display = Display.Visible;
			ui.InternalActivate();

			if (openSound != null) SoundEngine.PlaySound(openSound.Value);
		}
		else if (!Panels.ContainsKey(id))
		{
			Type type = entity.GetType();
			Type? entityType = EntityToUIMap.ContainsKey(type) ? type : type.BaseType;
			if (entityType is null) return;

			ui = (BaseUIPanel?)Activator.CreateInstance(EntityToUIMap[entityType], entity);
			if (ui is null) return;
			ui.Display = Display.Visible;

			ui.Position.PercentX = 50;
			ui.Position.PercentY = 25;

			ui.OnMouseDown += _ => {
				Children.Remove(ui);
				Children.Add(ui);
			};

			Add(ui);
			ui.InternalActivate();
			Panels.Add(id, ui);

			if (openSound != null) SoundEngine.PlaySound(openSound.Value);
		}*/
	}

	public void CloseAllUIs()
	{
		for (int i = 0; i < Children.Count; i++)
		{
			BaseElement element = Children[i];
			if (element is BaseUIPanel panel)
			{
				panel.Display = Display.None;
				panel.InternalDeactivate();
			}
		}

		Clear();

		Panels.Clear();
		ClosedUICache.Clear();
	}

	protected override void Update(GameTime gameTime)
	{
		/*PanelUI? gui = PanelUI.Instance;

		// bug: check if IHasUI entity still exists

		if (gui != null)
		{
			if (!Main.playerInventory)
			{
				List<BaseUIPanel> panels = gui.Children.Cast<BaseUIPanel>().ToList();
				foreach (BaseUIPanel ui in panels)
				{
					if (ui.Display != Display.Visible) continue;

					ClosedUICache.Add(ui.Container);
					gui.CloseUI(ui.Container);
				}
			}
			else
			{
				foreach (IHasUI ui in ClosedUICache) gui.OpenUI(ui);

				ClosedUICache.Clear();
			}
		}*/

		base.Update(gameTime);
	}

	/*public override BaseElement? GetElementAt(Vector2 point)
	{
		BaseElement? element = Children.LastOrDefault(current => current.ContainsPoint(point) && current.Display != Display.None);

		return element?.GetElementAt(point);
	}*/
}