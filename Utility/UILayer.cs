using BaseLibrary.Input;
using BaseLibrary.Input.Keyboard;
using BaseLibrary.Input.Mouse;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace BaseLibrary.UI.New
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
		private static Dictionary<Type, Type> UICache;
		public static PanelUI Instance;

		public override bool Enabled => !Main.ingameOptionsWindow && !Main.gameMenu;

		public PanelUI()
		{
			Width.Percent = 100;
			Height.Percent = 100;

			UICache = new Dictionary<Type, Type>();
			Instance = this;

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
					//element.Deactivate();
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

	// note: when capturing mouse click events mouse down/up still gets run
	public class UILayer : Layer
	{
		public override bool Enabled => /*!Main.ingameOptionsWindow && !Main.gameMenu*/true;

		internal List<BaseState> Elements = new List<BaseState>();
		private BaseElement Current;

		internal UILayer()
		{
			Add(new PanelUI());
			Add(new ChatUI());

			//PanelUI panelUI = new PanelUI();
			//panelUI.Recalculate();

			//ChatUI chatUI = new ChatUI();
			//chatUI.Recalculate();

			//Elements = new List<BaseState> { panelUI , chatUI };
		}

		public void Add(BaseState ui)
		{
			ui.Recalculate();
			Elements.Add(ui);
		}

		private BaseElement MouseDownElement;

		public override void OnDraw(SpriteBatch spriteBatch)
		{
			foreach (BaseState element in Elements.Where(element => element.Display != Display.None).Reverse())
			{
				element.InternalDraw(spriteBatch);
			}
		}

		public override void OnUpdate(GameTime gameTime)
		{
			foreach (BaseState element in Elements.Where(element => element.Display != Display.None))
			{
				element.InternalUpdate(gameTime);
			}

			// todo: handle middle, xbutton1,2, prevent code duplication
			if (MouseEvents.IsMouseDown(MouseButton.Left))
			{
				MouseButtonEventArgs args = new MouseButtonEventArgs
				{
					Modifiers = KeyboardUtil.GetModifiers(MouseEvents.Keyboard),
					Position = new Vector2(MouseEvents.Mouse.X, MouseEvents.Mouse.Y),
					Button = MouseButton.Left
				};

				foreach (BaseState element in Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)))
				{
					element.InternalMouseHeld(args);
					if (args.Handled) break;
				}
			}
			else if (MouseEvents.IsMouseDown(MouseButton.Right))
			{
				MouseButtonEventArgs args = new MouseButtonEventArgs
				{
					Modifiers = KeyboardUtil.GetModifiers(MouseEvents.Keyboard),
					Position = new Vector2(MouseEvents.Mouse.X, MouseEvents.Mouse.Y),
					Button = MouseButton.Right
				};

				foreach (BaseState element in Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)))
				{
					element.InternalMouseHeld(args);
					if (args.Handled) break;
				}
			}
		}

		public override void OnMouseDown(MouseButtonEventArgs args)
		{
			var elements = Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)).ToList();
			foreach (BaseState element in elements)
			{
				MouseDownElement = element.InternalMouseDown(args);
				if (args.Handled) break;
			}
		}

		public override void OnMouseUp(MouseButtonEventArgs args)
		{
			if (MouseDownElement != null)
			{
				MouseDownElement.InternalMouseUp(args);

				MouseDownElement = null;

				return;
			}

			var elements = Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)).ToList();
			foreach (BaseState element in elements)
			{
				element.InternalMouseUp(args);
				if (args.Handled) break;
			}
		}

		public override void OnMouseMove(MouseMoveEventArgs args)
		{
			var elements = Elements.Where(element =>element.Enabled&& element.Display != Display.None && element.ContainsPoint(args.Position)).ToList();
			foreach (BaseState element in elements)
			{
				element.InternalMouseMove(args);
				if (args.Handled) break;
			}

			BaseElement at = Elements.Where(baseElement=>baseElement.Enabled).Select(baseElement =>  baseElement.GetElementAt(args.Position)).FirstOrDefault(baseElement => baseElement != null);
			if (Current != at)
			{
				Current?.InternalMouseLeave(args);
				at?.InternalMouseEnter(args);
				Current = at;

				foreach (string key in PlayerInput.MouseKeys)
				{
					foreach (var pair in PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard].KeyStatus)
					{
						if (pair.Value.Contains(key))
						{
							PlayerInput.Triggers.Current.KeyStatus[pair.Key] = false;
						}
					}
				}

				PlayerInput.MouseKeys.Clear();
			}
		}

		public override void OnMouseScroll(MouseScrollEventArgs args)
		{
			foreach (BaseState element in Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)))
			{
				element.InternalMouseScroll(args);
				if (args.Handled) break;
			}
		}

		public override void OnClick(MouseButtonEventArgs args)
		{
			foreach (BaseState element in Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)))
			{
				element.InternalMouseClick(args);
				if (args.Handled) break;
			}
		}

		public override void OnDoubleClick(MouseButtonEventArgs args)
		{
			foreach (BaseState element in Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)))
			{
				element.InternalDoubleClick(args);
				if (args.Handled) break;
			}
		}

		public override void OnTripleClick(MouseButtonEventArgs args)
		{
			foreach (BaseState element in Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)))
			{
				element.InternalTripleClick(args);
				if (args.Handled) break;
			}
		}

		public override void OnKeyPressed(KeyboardEventArgs args)
		{
			foreach (BaseState element in Elements.Where(element => element.Display != Display.None))
			{
				element.InternalKeyPressed(args);
				if (args.Handled) break;
			}
		}

		public override void OnKeyReleased(KeyboardEventArgs args)
		{
			foreach (BaseState element in Elements.Where(element => element.Display != Display.None))
			{
				element.InternalKeyReleased(args);
				if (args.Handled) break;
			}
		}

		public override void OnKeyTyped(KeyboardEventArgs args)
		{
			foreach (BaseState element in Elements.Where(element => element.Display != Display.None))
			{
				element.InternalKeyTyped(args);
				if (args.Handled) break;
			}
		}

		public override void OnWindowResize(WindowResizedEventArgs args)
		{
			foreach (BaseState element in Elements.Where(element => element.Display != Display.None))
			{
				element.Recalculate();
			}
		}
	}
}