using System;
using System.Collections.Generic;
using System.Linq;
using BaseLibrary.Input;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace BaseLibrary.UI;

public interface IHasUI
{
	Guid GetID();

	SoundStyle? GetOpenSound() => null;

	SoundStyle? GetCloseSound() => null;
}

public class BaseUIPanel : UIPanel
{
	public IHasUI Container;

	public BaseUIPanel(IHasUI container)
	{
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
		WindowUI.Instance?.CloseAllUIs();
	}
}

public class WindowUI : BaseElement
{
	public static WindowUI? Instance;

	private Dictionary<Type, Type> EntityToUIMap = [];
	private Dictionary<Guid, BaseElement> Panels = [];
	private List<IHasUI> ClosedUICache = [];

	public WindowUI()
	{
		Instance = this;

		Size = Dimension.FromPercent(100);

		foreach (Mod mod in ModLoader.Mods.Where(mod => mod.Name != "ModLoader"))
		{
			Type[]? types = AssemblyManager.GetLoadableTypes(mod.Code);
			if (types is null) continue;

			foreach (Type type in types)
			{
				if (ReflectionUtility.IsSubclassOfRawGeneric(type, typeof(BaseUIPanel<>)) && type.BaseType is { GenericTypeArguments.Length: > 0 })
					EntityToUIMap[type.BaseType.GenericTypeArguments[0]] = type;
			}
		}
	}

	public void HandleUI(IHasUI? entity)
	{
		if (Main.netMode == NetmodeID.Server || entity is null) return;

		if (Panels.TryGetValue(entity.GetID(), out BaseElement? panel) && panel.Display == Display.Visible) CloseUI(entity);
		else
		{
			/*if (!ModContent.GetInstance<PanelUISystem>().ClosedUICache.Contains(entity))*/
			OpenUI(entity);

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
		panel.InternalDeactivate();*/

		SoundStyle? closeSound = entity.GetCloseSound();
		if (closeSound != null) SoundEngine.PlaySound(closeSound.Value);
	}

	public void OpenUI(IHasUI? entity)
	{
		if (Main.netMode == NetmodeID.Server || entity is null) return;

		if (Panels.ContainsKey(entity.GetID())) return;

		Type type = entity.GetType();
		Type? entityType = EntityToUIMap.ContainsKey(type) ? type : type.BaseType;
		if (entityType is null) return;

		BaseUIPanel? panel = (BaseUIPanel?)Activator.CreateInstance(EntityToUIMap[entityType], entity);
		if (panel is null) return;

		panel.Position = Dimension.FromPercent(50);

		Add(panel);
		Panels.Add(entity.GetID(), panel);

		SoundStyle? openSound = entity.GetOpenSound();
		if (openSound != null) SoundEngine.PlaySound(openSound.Value);
		// Guid id = entity.GetID();

		/*if (Panels.TryGetValue(id, out BaseUIPanel? ui) && ui.Display == Display.None)
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

	private Vector2 offset;
	private bool dragging;
	private BaseUIPanel? draggedPanel;

	protected override void MouseDown(MouseButtonEventArgs args)
	{
		BaseElement? element = GetElementAt(args.Position);

		if (element is BaseUIPanel panel && args.Button == MouseButton.Left)
		{
			offset = (args.Position - panel.Dimensions.TopLeft()).Floor();
			dragging = true;
			draggedPanel = panel;
			Remove(panel);
			Add(panel);

			args.Handled = true;
			return;
		}

		if (element is not null and not WindowUI)
		{
			while (element is not BaseUIPanel)
			{
				element = element.Parent;
			}

			Remove(element);
			Add(element);
		}

		base.MouseDown(args);
	}

	protected override void MouseUp(MouseButtonEventArgs args)
	{
		if (args.Button != MouseButton.Left)
		{
			base.MouseUp(args);
			return;
		}

		dragging = false;
		draggedPanel = null;

		args.Handled = true;
	}

	public void CloseAllUIs()
	{
		foreach (BaseElement element in Children)
		{
			element.Display = Display.None;
			element.InternalDeactivate();
		}

		Clear();

		Panels.Clear();
		ClosedUICache.Clear();
	}

	protected override void Update(GameTime gameTime)
	{
		if (!dragging || draggedPanel is null) return;

		Rectangle draggedDim = draggedPanel.OuterDimensions;

		draggedPanel.Position.PercentX = 0;
		draggedPanel.Position.PercentY = 0;

		draggedPanel.Position.PixelsX = Utils.Clamp((int)(Main.mouseX - offset.X - InnerDimensions.X), 0, InnerDimensions.Width - draggedDim.Width);
		draggedPanel.Position.PixelsY = Utils.Clamp((int)(Main.mouseY - offset.Y - InnerDimensions.Y), 0, InnerDimensions.Height - draggedDim.Height);

		// https://github.com/Flupp/sticky-window-snapping/blob/master/package/contents/code/main.js

		int l1 = draggedPanel.Position.PixelsX;
		int r1 = l1 + draggedDim.Width;
		int t1 = draggedPanel.Position.PixelsY;
		int b1 = t1 + draggedDim.Height;

		int threshold = ModContent.GetInstance<BaseLibraryConfig>().WindowSnapDistance;

		foreach (BaseElement element in Children.Except(draggedPanel))
		{
			Rectangle otherDim = element.OuterDimensions;
			int l2 = otherDim.Left;
			int r2 = otherDim.Right;
			int t2 = otherDim.Top;
			int b2 = otherDim.Bottom;

			if (Math.Max(l1, l2) < Math.Min(r1, r2))
			{
				if (Math.Abs(t1 - b2) <= threshold)
				{
					draggedPanel.Position.PixelsY = b2;
				}
				else if (Math.Abs(b1 - t2) <= threshold)
				{
					draggedPanel.Position.PixelsY = t2 - draggedDim.Height;
				}
			}

			if (Math.Max(t1, t2) < Math.Min(b1, b2))
			{
				if (Math.Abs(l1 - r2) <= threshold)
				{
					draggedPanel.Position.PixelsX = r2;
				}
				else if (Math.Abs(r1 - l2) <= threshold)
				{
					draggedPanel.Position.PixelsX = l2 - draggedDim.Width;
				}
			}
		}

		Recalculate();

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
}