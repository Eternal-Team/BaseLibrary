using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader.Input;
using Terraria.ModLoader.Input.Keyboard;
using Terraria.ModLoader.Input.Mouse;

namespace BaseLibrary.UI
{
	public class UILayer : Layer
	{
		public override bool Enabled => !Main.gameMenu && !Main.ingameOptionsWindow;

		public static UILayer Instance;

		private static float Scale => Main.gameMenu ? 1f : Main.UIScale;

		internal List<BaseState> Elements = new List<BaseState>();
		private BaseElement current;
		private BaseElement mouseDownElement;

		internal UILayer()
		{
			Instance = this;

			// Add(new Doot());
			// Add(new PanelUI());
			// Add(new ChatUI());
		}

		public void Add(BaseState ui)
		{
			ui.Recalculate();
			Elements.Add(ui);
		}

		public override void OnDraw(SpriteBatch spriteBatch, GameTime gameTime)
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
				MouseButtonEventArgs args = new MouseButtonEventArgs(mousePos * (1f / Scale), button, modifiers);

				foreach (BaseState element in Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)))
				{
					element.InternalMouseHeld(args);
					if (args.Handled) break;
				}
			}
		}

		public override void OnMouseDown(MouseButtonEventArgs inArgs)
		{
			MouseButtonEventArgs args = new MouseButtonEventArgs(inArgs.Position * (1f / Scale), inArgs.Button, inArgs.Modifiers);

			var elements = Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)).ToList();
			foreach (BaseState element in elements)
			{
				mouseDownElement = element.InternalMouseDown(args);
				if (args.Handled) break;
			}

			inArgs.Handled = args.Handled;
		}

		public override void OnMouseUp(MouseButtonEventArgs inArgs)
		{
			MouseButtonEventArgs args = new MouseButtonEventArgs(inArgs.Position * (1f / Scale), inArgs.Button, inArgs.Modifiers);

			if (mouseDownElement != null)
			{
				mouseDownElement.InternalMouseUp(args);

				mouseDownElement = null;

				return;
			}

			var elements = Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)).ToList();
			foreach (BaseState element in elements)
			{
				element.InternalMouseUp(args);
				if (args.Handled) break;
			}

			inArgs.Handled = args.Handled;
		}

		public override void OnMouseMove(MouseMoveEventArgs inArgs)
		{
			MouseMoveEventArgs args = new MouseMoveEventArgs(inArgs.Position * (1f / Scale), inArgs.Delta);

			var elements = Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)).ToList();
			foreach (BaseState element in elements)
			{
				element.InternalMouseMove(args);
				if (args.Handled) break;
			}

			BaseElement at = Elements.Where(baseElement => baseElement.Display != Display.None).Select(baseElement => baseElement.GetElementAt(args.Position)).FirstOrDefault(baseElement => baseElement != null);
			if (current != at)
			{
				current?.InternalMouseLeave(args);
				at?.InternalMouseEnter(args);
				current = at;

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

			inArgs.Handled = args.Handled;
		}

		public override void OnMouseScroll(MouseScrollEventArgs inArgs)
		{
			MouseScrollEventArgs args = new MouseScrollEventArgs(inArgs.Position * (1f / Scale), inArgs.Offset);

			foreach (BaseState element in Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)))
			{
				element.InternalMouseScroll(args);
				if (args.Handled) break;
			}

			inArgs.Handled = args.Handled;
		}

		public override void OnClick(MouseButtonEventArgs inArgs)
		{
			MouseButtonEventArgs args = new MouseButtonEventArgs(inArgs.Position * (1f / Scale), inArgs.Button, inArgs.Modifiers);

			foreach (BaseState element in Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)))
			{
				element.InternalMouseClick(args);
				if (args.Handled) break;
			}

			inArgs.Handled = args.Handled;
		}

		public override void OnDoubleClick(MouseButtonEventArgs inArgs)
		{
			MouseButtonEventArgs args = new MouseButtonEventArgs(inArgs.Position * (1f / Scale), inArgs.Button, inArgs.Modifiers);

			foreach (BaseState element in Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)))
			{
				element.InternalDoubleClick(args);
				if (args.Handled) break;
			}

			inArgs.Handled = args.Handled;
		}

		public override void OnTripleClick(MouseButtonEventArgs inArgs)
		{
			MouseButtonEventArgs args = new MouseButtonEventArgs(inArgs.Position * (1f / Scale), inArgs.Button, inArgs.Modifiers);

			foreach (BaseState element in Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)))
			{
				element.InternalTripleClick(args);
				if (args.Handled) break;
			}

			inArgs.Handled = args.Handled;
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

		public override void OnWindowResize(WindowResizedEventArgs inArgs)
		{
			WindowResizedEventArgs args = new WindowResizedEventArgs(inArgs.Size * (1f / Scale));

			foreach (BaseState element in Elements.Where(element => element.Display != Display.None))
			{
				element.Recalculate();
			}
		}
	}
}