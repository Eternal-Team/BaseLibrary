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

		// Main.NewText(BaseElement.HoveredElement is not null ? BaseElement.HoveredElement.GetType().Name : "NONE");
	}

	public override void OnMouseDown(MouseButtonEventArgs args)
	{
		mouseDownElement = Element.InternalMouseDown(args);
	}

	public override void OnMouseUp(MouseButtonEventArgs args)
	{
		if (mouseDownElement is not null)
		{
			mouseDownElement.InternalMouseUp(args);
			mouseDownElement = null;

			return;
		}

		Element.InternalMouseUp(args);
	}

	private static BaseElement? HoveredElement;

	public override void OnMouseMove(MouseMoveEventArgs args)
	{
		Element.InternalMouseMove(args);

		BaseElement? at = Element.GetElementAt(args.Position);
		if (current == at) return;

		current?.InternalMouseLeave(args);
		at?.InternalMouseEnter(args);
		HoveredElement = at;

		current = at;

		foreach ((string? key, _) in PlayerInput.MouseKeys.SelectMany(key => PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard].KeyStatus.Where(pair => pair.Value.Contains(key))))
		{
			PlayerInput.Triggers.Current.KeyStatus[key] = false;
		}

		PlayerInput.MouseKeys.Clear();
	}

	public override void OnMouseScroll(MouseScrollEventArgs args)
	{
		Element.InternalMouseScroll(args);
	}

	public override void OnClick(MouseButtonEventArgs args)
	{
		Element.InternalMouseClick(args);
	}

	public override void OnDoubleClick(MouseButtonEventArgs args)
	{
		Element.InternalDoubleClick(args);
	}

	public override void OnTripleClick(MouseButtonEventArgs args)
	{
		Element.InternalTripleClick(args);
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