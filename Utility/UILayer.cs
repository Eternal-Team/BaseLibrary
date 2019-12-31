using BaseLibrary.Input;
using BaseLibrary.UI;
using BaseLibrary.UI.New;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameInput;

namespace BaseLibrary
{
	public class UILayer : Layer
	{
		public override bool Enabled => !Main.ingameOptionsWindow && !Main.gameMenu;

		private List<BaseElement> Elements;

		internal UILayer()
		{
			Elements = new List<BaseElement>();

			TestUI ui = new TestUI();
			ui.Recalculate();
			Elements = new List<BaseElement> { ui };
		}

		public override void OnDraw(SpriteBatch spriteBatch)
		{
			foreach (BaseElement element in Elements.Where(element => element.Display != Display.None))
			{
				element.InternalDraw(spriteBatch);
			}
		}

		public override void OnUpdate(GameTime gameTime)
		{
			foreach (BaseElement element in Elements.Where(element => element.Display != Display.None))
			{
				element.InternalUpdate(gameTime);
			}
		}

		public override void OnMouseDown(MouseButtonEventArgs args)
		{
			var elements = Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)).ToList();
			foreach (BaseElement element in elements)
			{
				element.InternalMouseDown(args);
				if (args.Handled) break;
			}

			if (elements.Count > 0) args.Handled = true;
		}

		public override void OnMouseUp(MouseButtonEventArgs args)
		{
			var elements = Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)).ToList();
			foreach (BaseElement element in elements)
			{
				element.InternalMouseUp(args);
				if (args.Handled) break;
			}

			if (elements.Count > 0) args.Handled = true;
		}

		public override void OnMouseMove(MouseMoveEventArgs args)
		{
			var elements = Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)).ToList();
			foreach (BaseElement element in elements)
			{
				element.MouseMove(args);
				if (args.Handled) break;
			}

			if (elements.Count > 0)
			{
				args.Handled = true;

				foreach (string key in PlayerInput.MouseKeys)
				{
					foreach (var current in PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard].KeyStatus)
					{
						if (current.Value.Contains(key))
						{
							PlayerInput.Triggers.Old.KeyStatus[current.Key] = true;
							PlayerInput.Triggers.Current.KeyStatus[current.Key] = false;
						}
					}
				}

				PlayerInput.MouseKeys.Clear();
			}
		}

		public override void OnMouseScroll(MouseScrollEventArgs args)
		{
			foreach (BaseElement element in Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)))
			{
				element.MouseScroll(args);
				if (args.Handled) break;
			}
		}

		public override void OnClick(MouseButtonEventArgs args)
		{
			foreach (BaseElement element in Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)))
			{
				element.MouseClick(args);
				if (args.Handled) break;
			}
		}

		public override void OnDoubleClick(MouseButtonEventArgs args)
		{
			foreach (BaseElement element in Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)))
			{
				element.DoubleClick(args);
				if (args.Handled) break;
			}
		}

		public override void OnTripleClick(MouseButtonEventArgs args)
		{
			foreach (BaseElement element in Elements.Where(element => element.Display != Display.None && element.ContainsPoint(args.Position)))
			{
				element.TripleClick(args);
				if (args.Handled) break;
			}
		}

		public override void OnKeyPressed(KeyboardEventArgs args)
		{
			foreach (BaseElement element in Elements.Where(element => element.Display != Display.None))
			{
				element.KeyPressed(args);
				if (args.Handled) break;
			}
		}

		public override void OnKeyReleased(KeyboardEventArgs args)
		{
			foreach (BaseElement element in Elements.Where(element => element.Display != Display.None))
			{
				element.KeyReleased(args);
				if (args.Handled) break;
			}
		}

		public override void OnKeyTyped(KeyboardEventArgs args)
		{
			foreach (BaseElement element in Elements.Where(element => element.Display != Display.None))
			{
				element.KeyTyped(args);
				if (args.Handled) break;
			}
		}
	}
}