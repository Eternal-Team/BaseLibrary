using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;

namespace BaseLibrary.UI;

public class UILayer : Layer
{
	public override bool Enabled => !Main.gameMenu && !Main.ingameOptionsWindow;

	public static UILayer Instance;

	private static float Scale => Main.gameMenu ? 1f : Main.UIScale;

	private List<BaseState> Elements = new();
	private BaseElement current;
	private BaseElement mouseDownElement;

	private IEnumerable<BaseState> VisibleElements() => Elements.Where(element => element.Display != Display.None);
	private IEnumerable<BaseState> HoveredElements(Vector2 mouse) => Elements.Where(element => element.Display != Display.None && element.ContainsPoint(mouse));

	internal UILayer()
	{
		Instance = this;

		Add(new PanelUI());
	}

	public void Add(BaseState ui)
	{
		ui.Recalculate();
		Elements.Add(ui);
	}

	public void Remove(BaseState ui)
	{
		Elements.Remove(ui);
	}

	public override void OnDraw(SpriteBatch spriteBatch, GameTime gameTime)
	{
		foreach (BaseState element in VisibleElements().Reverse())
		{
			element.InternalDraw(spriteBatch);
		}
	}

	public override void OnUpdate(GameTime gameTime)
	{
		foreach (BaseState element in VisibleElements())
		{
			element.InternalUpdate(gameTime);
		}

		var modifiers = KeyboardUtil.GetModifiers(MouseInput.Keyboard);
		var mousePos = new Vector2(MouseInput.Mouse.X, MouseInput.Mouse.Y);
		foreach (MouseButton button in MouseInput.GetHeldButtons())
		{
			MouseButtonEventArgs args = new MouseButtonEventArgs(mousePos * (1f / Scale), button, modifiers);

			foreach (BaseState element in HoveredElements(args.Position))
			{
				element.InternalMouseHeld(args);
				if (args.Handled) break;
			}
		}
	}

	public override void OnMouseDown(MouseButtonEventArgs inArgs)
	{
		MouseButtonEventArgs args = new MouseButtonEventArgs(inArgs.Position * (1f / Scale), inArgs.Button, inArgs.Modifiers);

		foreach (BaseState element in HoveredElements(args.Position))
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

		foreach (BaseState element in HoveredElements(args.Position))
		{
			element.InternalMouseUp(args);
			if (args.Handled) break;
		}

		inArgs.Handled = args.Handled;
	}

	public override void OnMouseMove(MouseMoveEventArgs inArgs)
	{
		MouseMoveEventArgs args = new MouseMoveEventArgs(inArgs.Position * (1f / Scale), inArgs.Delta);

		foreach (BaseState element in HoveredElements(args.Position))
		{
			element.InternalMouseMove(args);
			if (args.Handled) break;
		}

		BaseElement at = Elements.Where(baseElement => baseElement.Display != Display.None)
			.Select(baseElement => baseElement.GetElementAt(args.Position))
			.FirstOrDefault(baseElement => baseElement != null && baseElement.Display != Display.None);
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

		foreach (BaseState element in HoveredElements(args.Position))
		{
			element.InternalMouseScroll(args);
			if (args.Handled) break;
		}

		inArgs.Handled = args.Handled;
	}

	public override void OnClick(MouseButtonEventArgs inArgs)
	{
		MouseButtonEventArgs args = new MouseButtonEventArgs(inArgs.Position * (1f / Scale), inArgs.Button, inArgs.Modifiers);

		foreach (BaseState element in HoveredElements(args.Position))
		{
			element.InternalMouseClick(args);
			if (args.Handled) break;
		}

		inArgs.Handled = args.Handled;
	}

	public override void OnDoubleClick(MouseButtonEventArgs inArgs)
	{
		MouseButtonEventArgs args = new MouseButtonEventArgs(inArgs.Position * (1f / Scale), inArgs.Button, inArgs.Modifiers);

		foreach (BaseState element in HoveredElements(args.Position))
		{
			element.InternalDoubleClick(args);
			if (args.Handled) break;
		}

		inArgs.Handled = args.Handled;
	}

	public override void OnTripleClick(MouseButtonEventArgs inArgs)
	{
		MouseButtonEventArgs args = new MouseButtonEventArgs(inArgs.Position * (1f / Scale), inArgs.Button, inArgs.Modifiers);

		foreach (BaseState element in HoveredElements(args.Position))
		{
			element.InternalTripleClick(args);
			if (args.Handled) break;
		}

		inArgs.Handled = args.Handled;
	}

	public override void OnKeyPressed(KeyboardEventArgs args)
	{
		foreach (BaseState element in VisibleElements())
		{
			element.InternalKeyPressed(args);
			if (args.Handled) break;
		}
	}

	public override void OnKeyReleased(KeyboardEventArgs args)
	{
		foreach (BaseState element in VisibleElements())
		{
			element.InternalKeyReleased(args);
			if (args.Handled) break;
		}
	}

	public override void OnKeyTyped(KeyboardEventArgs args)
	{
		foreach (BaseState element in VisibleElements())
		{
			element.InternalKeyTyped(args);
			if (args.Handled) break;
		}
	}

	public override void OnWindowResize(WindowResizedEventArgs inArgs)
	{
		foreach (BaseState element in VisibleElements())
		{
			element.Recalculate();
		}
	}
}