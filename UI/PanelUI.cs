using System;
using System.Collections.Generic;
using System.Linq;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.UI;

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
		Settings.Draggable = true;
		Container = container;
	}
}

public class BaseUIPanel<T> : BaseUIPanel where T : IHasUI
{
	public new T Container => (T)base.Container;

	public BaseUIPanel(T container) : base(container)
	{
	}
}

public class PanelUISystem : ModSystem
{
	internal List<IHasUI> ClosedUICache = new();

	public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
	{
		int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));

		if (MouseTextIndex != -1)
		{
			layers.Insert(MouseTextIndex, new LegacyGameInterfaceLayer("Layers", delegate
			{
				foreach (Layer layer in Input.Layers)
				{
					layer.OnDraw(Main.spriteBatch, Main._drawInterfaceGameTime);
				}

				return true;
			}, InterfaceScaleType.UI));
		}
	}

	public override void UpdateUI(GameTime gameTime)
	{
		var gui = PanelUI.Instance;

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
		}

		foreach (Layer layer in Input.Layers)
		{
			layer.OnUpdate(gameTime);
		}
	}

	public override void PreSaveAndQuit()
	{
		ClosedUICache.Clear();
		PanelUI.Instance?.CloseAllUIs();
	}
}

public class PanelUI : BaseState
{
	public static PanelUI? Instance;

	private Dictionary<Type, Type> EntityToUIMap = new();
	private Dictionary<Guid, BaseUIPanel> Panels = new();

	public PanelUI()
	{
		Instance = this;

		foreach (Mod mod in ModLoader.Mods.Where(mod => mod.Name != "ModLoader"))
		{
			var types = AssemblyManager.GetLoadableTypes(mod.Code);
			if (types is null) continue;

			foreach (Type type in types)
			{
				if (ReflectionUtility.IsSubclassOfRawGeneric(type, typeof(BaseUIPanel<>)) && type.BaseType != null && type.BaseType.GenericTypeArguments.Length > 0)
					EntityToUIMap[type.BaseType.GenericTypeArguments[0]] = type;
			}
		}
	}

	public void HandleUI(IHasUI? entity)
	{
		if (Main.netMode == NetmodeID.Server || entity is null) return;

		if (Panels.TryGetValue(entity.GetID(), out BaseUIPanel? panel) && panel.Display == Display.Visible) CloseUI(entity);
		else
		{
			if (!ModContent.GetInstance<PanelUISystem>().ClosedUICache.Contains(entity)) OpenUI(entity);

			if (!Main.playerInventory) Main.playerInventory = true;
		}
	}

	public void CloseUI(IHasUI? entity)
	{
		if (Main.netMode == NetmodeID.Server || entity is null) return;
		if (!Panels.TryGetValue(entity.GetID(), out BaseUIPanel? panel) || panel.Display != Display.Visible) return;

		panel.Display = Display.None;
		panel.InternalDeactivate();

		var closeSound = entity.GetCloseSound();
		if (closeSound != null) SoundEngine.PlaySound(closeSound.Value);
	}

	public void OpenUI(IHasUI? entity)
	{
		if (Main.netMode == NetmodeID.Server || entity is null) return;

		var openSound = entity.GetOpenSound();
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

			ui.X.Percent = 50;
			ui.Y.Percent = 25;

			ui.OnMouseDown += _ =>
			{
				Children.Remove(ui);
				Children.Add(ui);
			};

			Add(ui);
			ui.InternalActivate();
			Panels.Add(id, ui);

			if (openSound != null) SoundEngine.PlaySound(openSound.Value);
		}
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
	}

	public override BaseElement? GetElementAt(Vector2 point)
	{
		BaseElement? element = Children.LastOrDefault(current => current.ContainsPoint(point) && current.Display != Display.None);

		return element?.GetElementAt(point);
	}
}