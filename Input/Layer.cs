using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;

namespace BaseLibrary.Input;

public abstract class Layer
{
	public virtual bool Enabled => true;

	public virtual void OnUpdate(GameTime gameTime)
	{
	}

	public virtual void OnDraw(SpriteBatch spriteBatch, GameTime gameTime)
	{
	}

	public virtual void OnMouseMove(MouseMoveEventArgs args)
	{
	}

	public virtual void OnMouseDown(MouseButtonEventArgs args)
	{
	}

	public virtual void OnMouseUp(MouseButtonEventArgs args)
	{
	}

	public virtual void OnMouseEnter(MouseEventArgs args)
	{
	}

	public virtual void OnMouseLeave(MouseEventArgs args)
	{
	}

	public virtual void OnClick(MouseButtonEventArgs args)
	{
	}

	public virtual void OnDoubleClick(MouseButtonEventArgs args)
	{
	}

	public virtual void OnTripleClick(MouseButtonEventArgs args)
	{
	}

	public virtual void OnMouseScroll(MouseScrollEventArgs args)
	{
	}

	public virtual void OnKeyPressed(KeyboardEventArgs args)
	{
	}

	public virtual void OnKeyReleased(KeyboardEventArgs args)
	{
	}

	public virtual void OnKeyTyped(KeyboardEventArgs args)
	{
	}

	public virtual void OnWindowResize(WindowResizedEventArgs args)
	{
	}
	
	public virtual void OnScaleChanged()
	{
	}
}

public class TerrariaLayer : Layer
{
	private static KeyConfiguration KeyConfiguration
	{
		get
		{
			if (Main.gameMenu && !PlayerInput.WritingText) return PlayerInput.CurrentProfile.InputModes[InputMode.KeyboardUI];
			return PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard];
		}
	}

	private static string NamedMouseToNumber(MouseButton button)
	{
		switch (button)
		{
			case MouseButton.Left:
				return "Mouse1";
			case MouseButton.Right:
				return "Mouse2";
			case MouseButton.Middle:
				return "Mouse3";
			case MouseButton.XButton1:
				return "Mouse4";
			case MouseButton.XButton2:
				return "Mouse5";
			default:
				throw new Exception("Unsupported mouse button " + button);
		}
	}

	public override void OnMouseMove(MouseMoveEventArgs args)
	{
		PlayerInput.MouseX = (int)(args.X * PlayerInput.RawMouseScale.X);
		PlayerInput.MouseY = (int)(args.Y * PlayerInput.RawMouseScale.Y);

		Main.lastMouseX = Main.mouseX;
		Main.lastMouseY = Main.mouseY;
		Main.mouseX = PlayerInput.MouseX;
		Main.mouseY = PlayerInput.MouseY;

		PlayerInput.CurrentInputMode = InputMode.Mouse;
		PlayerInput.Triggers.Current.UsedMovementKey = false;

		if (!PlayerInput.PreventFirstMousePositionGrab) PlayerInput.SettingsForUI.SetCursorMode(CursorMode.Mouse);
		PlayerInput.PreventFirstMousePositionGrab = false;
	}

	public override void OnMouseDown(MouseButtonEventArgs args)
	{
		PlayerInput.CurrentInputMode = InputMode.Mouse;
		PlayerInput.Triggers.Current.UsedMovementKey = false;

		foreach (var item in KeyConfiguration.KeyStatus)
		{
			if (item.Value.Contains(NamedMouseToNumber(args.Button)))
				PlayerInput.Triggers.Current.KeyStatus[item.Key] = true;
		}
	}

	public override void OnMouseUp(MouseButtonEventArgs args)
	{
		foreach (var pair in KeyConfiguration.KeyStatus)
		{
			if (pair.Value.Contains(NamedMouseToNumber(args.Button)))
				PlayerInput.Triggers.Current.KeyStatus[pair.Key] = false;
		}
	}

	public override void OnMouseScroll(MouseScrollEventArgs args)
	{
		PlayerInput.ScrollWheelValue = (int)args.Y;
		PlayerInput.ScrollWheelDelta = (int)args.OffsetY;
		PlayerInput.ScrollWheelDeltaForUI = PlayerInput.ScrollWheelDelta;

		PlayerInput.CurrentInputMode = InputMode.Mouse;
		PlayerInput.Triggers.Current.UsedMovementKey = false;
	}

	public override void OnKeyPressed(KeyboardEventArgs args)
	{
		Main.ChromaPainter.PressKey(args.Key);

		if (args.Key.ToString() == Main.blockKey)
		{
			Main.blockKey = Keys.None.ToString();
			return;
		}

		if (PlayerInput.WritingText)
			return;

		KeyConfiguration.Processkey(PlayerInput.Triggers.Current, args.Key.ToString(), InputMode.Keyboard);
	}

	public override void OnKeyReleased(KeyboardEventArgs args)
	{
		foreach (var pair in KeyConfiguration.KeyStatus)
		{
			if (pair.Value.Contains(args.Key.ToString()))
			{
				PlayerInput.Triggers.Current.KeyStatus[pair.Key] = false;
			}
		}
	}
}

public class LayerStack : IEnumerable<Layer>
{
	public IEnumerator<Layer> GetEnumerator()
	{
		for (int i = layers.Count - 1; i >= 0; i--)
		{
			if (!layers[i].Enabled) continue;
			yield return layers[i];
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	private int layerInsertIndex;
	private List<Layer> layers;

	public LayerStack() => layers = new List<Layer>();

	public void PushLayer(Layer layer)
	{
		layers.Insert(layerInsertIndex++, layer);
	}

	public void PushOverlay(Layer layer)
	{
		layers.Add(layer);
	}

	public void PopLayer(Layer layer)
	{
		int index = layers.IndexOf(layer);
		if (index >= 0 && index < layerInsertIndex)
		{
			layers.Remove(layer);
			layerInsertIndex--;
		}
	}

	public void PopOverlay(Layer layer)
	{
		int index = layers.IndexOf(layer);
		if (index >= layerInsertIndex && index < layers.Count)
		{
			layers.Remove(layer);
		}
	}
}