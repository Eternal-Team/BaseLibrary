using BaseLibrary.Input;
using BaseLibrary.Input.Keyboard;
using BaseLibrary.Input.Mouse;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameInput;

namespace BaseLibrary.UI
{
	// note: when capturing mouse click events mouse down/up still gets run
	public class UILayer : Layer
	{
		public override bool Enabled => true;

		internal List<BaseState> Elements = new List<BaseState>();
		private BaseElement Current;

		internal UILayer()
		{
			Add(new PanelUI());
			Add(new ChatUI());
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

			var modifiers = KeyboardUtil.GetModifiers(MouseInput.Keyboard);
			var mousePos = new Vector2(MouseInput.Mouse.X, MouseInput.Mouse.Y);
			foreach (MouseButton button in MouseInput.GetHeldButtons())
			{
				MouseButtonEventArgs args = new MouseButtonEventArgs
				{
					Modifiers = modifiers,
					Position = mousePos * (1f / Main.UIScale),
					Button = button
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
			args.Position *= 1f / Main.UIScale;

			var elements = Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)).ToList();
			foreach (BaseState element in elements)
			{
				MouseDownElement = element.InternalMouseDown(args);
				if (args.Handled) break;
			}

			args.Position *= Main.UIScale;
		}

		public override void OnMouseUp(MouseButtonEventArgs args)
		{
			args.Position *= 1f / Main.UIScale;

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

			args.Position *= Main.UIScale;
		}

		public override void OnMouseMove(MouseMoveEventArgs args)
		{
			args.Position *= 1f / Main.UIScale;

			var elements = Elements.Where(element => element.Enabled && element.Display != Display.None && element.ContainsPoint(args.Position)).ToList();
			foreach (BaseState element in elements)
			{
				element.InternalMouseMove(args);
				if (args.Handled) break;
			}

			BaseElement at = Elements.Where(baseElement => baseElement.Enabled).Select(baseElement => baseElement.GetElementAt(args.Position)).FirstOrDefault(baseElement => baseElement != null);
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

			args.Position *= Main.UIScale;
		}

		public override void OnMouseScroll(MouseScrollEventArgs args)
		{
			args.Position *= 1f / Main.UIScale;

			foreach (BaseState element in Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)))
			{
				element.InternalMouseScroll(args);
				if (args.Handled) break;
			}

			args.Position *= Main.UIScale;
		}

		public override void OnClick(MouseButtonEventArgs args)
		{
			args.Position *= 1f / Main.UIScale;

			foreach (BaseState element in Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)))
			{
				element.InternalMouseClick(args);
				if (args.Handled) break;
			}

			args.Position *= Main.UIScale;
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
			args.Size *= 1f / Main.UIScale;

			foreach (BaseState element in Elements.Where(element => element.Display != Display.None))
			{
				element.Recalculate();
			}

			args.Size *= Main.UIScale;
		}
	}
}