using System.Linq;
using BaseLibrary.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace BaseLibrary.UI;

public class UISystem : ModSystem
{
	public static readonly UILayer UILayer = new();

	public override void OnModLoad()
	{
		InputSystem.LayerStack.PushLayer(UILayer);
	}

	public override void PostDrawInterface(SpriteBatch spriteBatch)
	{
		UILayer.OnDraw(spriteBatch, Main.gameTimeCache);
	}

	public override void UpdateUI(GameTime gameTime)
	{
		UILayer.OnUpdate(gameTime);
	}
}

public class UILayer : Layer
{
	public override bool Enabled => !Main.gameMenu;

	private readonly BaseElement Element = new() { Size = Dimension.FromPercent(100) };
	private BaseElement? current;
	private BaseElement? mouseDownElement;

	public UILayer()
	{
		Element.Recalculate();
	}
	
	public void Add(BaseElement element)
	{
		Element.Add(element);
	}

	public override void OnDraw(SpriteBatch spriteBatch, GameTime gameTime)
	{
		Element.InternalDraw(spriteBatch);
	}

	public override void OnUpdate(GameTime gameTime)
	{
		Element.InternalUpdate(gameTime);

		Modifiers modifiers = KeyboardUtil.GetModifiers(MouseInput.Keyboard);
		Vector2 mouse = new(MouseInput.currentMouseState.X, MouseInput.currentMouseState.Y);
		foreach (MouseButton button in MouseInput.GetHeldButtons())
		{
			MouseButtonEventArgs args = new(mouse, button, modifiers);

			Element.InternalMouseHeld(args);
		}
	}

	public override void OnMouseDown(MouseButtonEventArgs args)
	{
		MouseButtonEventArgs a = new(args.Position * (1f / Main.UIScale), args.Button, args.Modifiers);
		
		mouseDownElement = Element.InternalMouseDown(a);
		args.Handled = a.Handled;
	}

	public override void OnMouseUp(MouseButtonEventArgs args)
	{
		MouseButtonEventArgs a = new(args.Position * (1f / Main.UIScale), args.Button, args.Modifiers);
		
		if (mouseDownElement is not null)
		{
			mouseDownElement.InternalMouseUp(a);
			mouseDownElement = null;

			return;
		}

		Element.InternalMouseUp(a);
		args.Handled = a.Handled;
	}

	private static BaseElement? HoveredElement;

	public override void OnMouseMove(MouseMoveEventArgs args)
	{
		MouseMoveEventArgs a = new(args.Position * (1f / Main.UIScale), args.Delta );
		
		Element.InternalMouseMove(a);

		BaseElement? at = Element.GetElementAt(a.Position);
		if (current == at) return;

		current?.InternalMouseLeave(a);
		at?.InternalMouseEnter(a);
		HoveredElement = at;

		current = at;

		foreach ((string? key, _) in PlayerInput.MouseKeys.SelectMany(key => PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard].KeyStatus.Where(pair => pair.Value.Contains(key))))
		{
			PlayerInput.Triggers.Current.KeyStatus[key] = false;
		}

		PlayerInput.MouseKeys.Clear();
		args.Handled = a.Handled;
	}

	public override void OnMouseScroll(MouseScrollEventArgs args)
	{
		MouseScrollEventArgs a = new(args.Position * (1f / Main.UIScale), args.Offset);
		Element.InternalMouseScroll(a);
		args.Handled = a.Handled;
	}

	public override void OnClick(MouseButtonEventArgs args)
	{
		MouseButtonEventArgs a = new(args.Position * (1f / Main.UIScale), args.Button, args.Modifiers);
		Element.InternalMouseClick(a);
		args.Handled = a.Handled;
	}

	public override void OnDoubleClick(MouseButtonEventArgs args)
	{
		MouseButtonEventArgs a = new(args.Position * (1f / Main.UIScale), args.Button, args.Modifiers);
		Element.InternalDoubleClick(a);
		args.Handled = a.Handled;
	}

	public override void OnTripleClick(MouseButtonEventArgs args)
	{
		MouseButtonEventArgs a = new(args.Position * (1f / Main.UIScale), args.Button, args.Modifiers);
		Element.InternalTripleClick(a);
		args.Handled = a.Handled;
	}

	public override void OnKeyPressed(KeyboardEventArgs args)
	{
		Element.InternalKeyPressed(args);
	}

	public override void OnKeyReleased(KeyboardEventArgs args)
	{
		Element.InternalKeyReleased(args);
	}

	public override void OnKeyTyped(KeyboardEventArgs args)
	{
		Element.InternalKeyTyped(args);
	}

	public override void OnWindowResize(WindowResizedEventArgs inArgs)
	{
		Element.Recalculate();
	}

	public override void OnScaleChanged()
	{
		// foreach (BaseElement element in VisibleElements())
		// {
		// 	element.Recalculate();
		// }
	}
}