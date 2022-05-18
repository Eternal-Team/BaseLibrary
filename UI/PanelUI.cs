// using System;
// using System.Collections.Generic;
// using System.Linq;
// using BaseLibrary.Utility;
// using Microsoft.Xna.Framework;
// using Terraria;
// using Terraria.Audio;
// using Terraria.ID;
// using Terraria.ModLoader;
// using Terraria.ModLoader.Container;
//
// namespace BaseLibrary.UI
// {
// 	public interface IHasUI
// 	{
// 		Guid GetID();
//
// 		LegacySoundStyle CloseSound { get; }
//
// 		LegacySoundStyle OpenSound { get; }
// 	}
//
// 	public class BaseUIPanel : UIPanel
// 	{
// 		public IHasUI Container;
//
// 		public BaseUIPanel(IHasUI container)
// 		{
// 			Settings.Draggable = true;
// 			Container = container;
// 		}
// 	}
//
// 	public class BaseUIPanel<T> : BaseUIPanel where T : IHasUI
// 	{
// 		public new T Container => (T)base.Container;
//
// 		public BaseUIPanel(T container) : base(container)
// 		{
// 		}
// 	}
//
// 	public interface IItemStorageUI
// 	{
// 		ItemStorage GetItemStorage();
//
// 		string GetTexture(Item item);
// 	}
//
// 	public class PanelUISystem : ModSystem
// 	{
// 		internal static List<IHasUI> ClosedUICache = new List<IHasUI>();
//
// 		public override void UpdateUI(GameTime gameTime)
// 		{
// 			var gui = PanelUI.Instance;
//
// 			// bug: check if IHasUI entity still exists 
//
// 			if (!Main.playerInventory)
// 			{
// 				List<BaseUIPanel> panels = gui.Children.Cast<BaseUIPanel>().ToList();
// 				foreach (BaseUIPanel ui in panels)
// 				{
// 					if (ui.Display != Display.Visible) continue;
// 					
// 					ClosedUICache.Add(ui.Container);
// 					gui.CloseUI(ui.Container);
// 				}
// 			}
// 			else
// 			{
// 				foreach (IHasUI ui in ClosedUICache) gui.OpenUI(ui);
//
// 				ClosedUICache.Clear();
// 			}
// 		}
//
// 		public override void PreSaveAndQuit()
// 		{
// 			ClosedUICache.Clear();
// 			PanelUI.Instance.CloseAllUIs();
// 		}
// 	}
//
// 	public class PanelUI : BaseState
// 	{
// 		public static PanelUI Instance;
//
// 		private Dictionary<Type, Type> UICache = new Dictionary<Type, Type>();
// 		private Dictionary<Guid, BaseUIPanel> Panels = new Dictionary<Guid, BaseUIPanel>();
//
// 		public PanelUI()
// 		{
// 			Instance = this;
//
// 			foreach (Type type in ModLoader.Mods.Where(mod => mod.Name != "ModLoader").SelectMany(mod => mod.Code?.GetTypes()))
// 			{
// 				if (ReflectionUtility.IsSubclassOfRawGeneric(type, typeof(BaseUIPanel<>)) && type.BaseType != null && type.BaseType.GenericTypeArguments.Length > 0)
// 					UICache[type.BaseType.GenericTypeArguments[0]] = type;
// 			}
// 		}
//
// 		public void HandleUI(IHasUI entity)
// 		{
// 			if (Main.netMode == NetmodeID.Server || entity == null) return;
//
// 			if (Panels.TryGetValue(entity.GetID(), out BaseUIPanel panel) && panel.Display == Display.Visible) CloseUI(entity);
// 			else
// 			{
// 				if (!PanelUISystem.ClosedUICache.Contains(entity)) OpenUI(entity);
//
// 				// if (!Main.playerInventory) Main.playerInventory = true;
// 			}
// 		}
//
// 		public void CloseUI(IHasUI entity)
// 		{
// 			if (Main.netMode == NetmodeID.Server || entity == null) return;
// 			if (!Panels.TryGetValue(entity.GetID(), out BaseUIPanel panel) || panel.Display != Display.Visible) return;
//
// 			panel.Display = Display.None;
// 			panel.InternalDeactivate();
//
// 			SoundEngine.PlaySound(entity.CloseSound);
// 		}
//
// 		public void OpenUI(IHasUI entity)
// 		{
// 			if (Main.netMode == NetmodeID.Server || entity == null) return;
//
// 			if (Panels.TryGetValue(entity.GetID(), out BaseUIPanel ui) && ui.Display == Display.None)
// 			{
// 				ui.Display = Display.Visible;
// 				ui.InternalActivate();
//
// 				SoundEngine.PlaySound(entity.OpenSound);
// 			}
// 			else if (!Panels.ContainsKey(entity.GetID()))
// 			{
// 				Type entityType = UICache.ContainsKey(entity.GetType()) ? entity.GetType() : entity.GetType().BaseType;
//
// 				ui = (BaseUIPanel)Activator.CreateInstance(UICache[entityType], entity);
// 				ui.Display = Display.Visible;
// 				ui.InternalActivate();
//
// 				ui.X.Percent = 50;
// 				ui.Y.Percent = 25;
//
// 				ui.OnMouseDown += args =>
// 				{
// 					Children.Remove(ui);
// 					Children.Add(ui);
// 				};
//
// 				Add(ui);
// 				Panels.Add(entity.GetID(), ui);
// 				
// 				SoundEngine.PlaySound(entity.OpenSound);
// 			}
// 		}
//
// 		public void CloseAllUIs()
// 		{
// 			for (int i = 0; i < Children.Count; i++)
// 			{
// 				BaseElement element = Children[i];
// 				if (element is BaseUIPanel panel) panel.Display = Display.None;
// 			}
//
// 			Clear();
// 		}
//
// 		public override BaseElement GetElementAt(Vector2 point)
// 		{
// 			BaseElement element = Children.FirstOrDefault(current => current.ContainsPoint(point) && current.Display != Display.None);
//
// 			return element?.GetElementAt(point);
// 		}
// 	}
// }