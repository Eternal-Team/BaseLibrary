using System;
using System.Collections.Generic;
using System.Linq;
using BaseLibrary.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;

namespace BaseLibrary.UI;

public partial class BaseElement
{
	internal void InternalUpdate(GameTime gameTime)
	{
		Update(gameTime);

		foreach (BaseElement current in _children.Where(current => current.Display != Display.None))
		{
			current.InternalUpdate(gameTime);
		}

		IsMouseHovering = Dimensions.Contains(Main.MouseScreen);
	}

	internal void InternalDraw(SpriteBatch spriteBatch)
	{
		GraphicsDevice device = spriteBatch.GraphicsDevice;
		SamplerState sampler = SamplerState.LinearClamp;
		RasterizerState rasterizer = new() { CullMode = CullMode.None, ScissorTestEnable = true };

		Rectangle original = device.ScissorRectangle;

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);

		Draw(spriteBatch);

		spriteBatch.End();

		if (Overflow == Overflow.Hidden)
		{
			Rectangle clippingRectangle = GetClippingRectangle(spriteBatch);
			Rectangle adjustedClippingRectangle = Rectangle.Intersect(clippingRectangle, device.ScissorRectangle);
			device.ScissorRectangle = adjustedClippingRectangle;
		}

		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);

		DrawChildren(spriteBatch);
		
		if (IsMouseHovering && HoverText is not null)
		{
			switch (HoverText)
			{
				case Func<string> func:
					Main.instance.MouseText(func());
					break;
				case LocalizedText translation:
					Main.instance.MouseText(translation.ToString());
					break;
				default:
					Main.instance.MouseText(HoverText.ToString());
					break;
			}
		}

		spriteBatch.End();

		device.ScissorRectangle = original;

		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);

#if DEBUG
			// if (debugDraw && debug)
			// {
				// if (OuterDimensions != Dimensions) spriteBatch.Draw(TextureAssets.MagicPixel.Value, OuterDimensions, Color.Goldenrod * 0.5f);
				// spriteBatch.Draw(TextureAssets.MagicPixel.Value, Dimensions, Color.LimeGreen * 0.2f);
				// if (InnerDimensions != Dimensions) spriteBatch.Draw(TextureAssets.MagicPixel.Value, InnerDimensions, Color.LightBlue * 0.5f);
			// }
#endif
	}

	internal void InternalMouseHeld(MouseButtonEventArgs args)
	{
		foreach (BaseElement element in ElementsAt(args.Position))
		{
			element.MouseHeld(args);
			if (args.Handled) return;
		}

		MouseHeld(args);
	}

	internal BaseElement InternalMouseDown(MouseButtonEventArgs args)
	{
		foreach (BaseElement element in ElementsAt(args.Position))
		{
			element.MouseDown(args);
			if (args.Handled) return element;
		}

		MouseDown(args);
		return this;
	}

	internal void InternalMouseUp(MouseButtonEventArgs args)
	{
		foreach (BaseElement element in ElementsAt(args.Position))
		{
			element.MouseUp(args);
			if (args.Handled) return;
		}

		MouseUp(args);
	}

	internal void InternalMouseClick(MouseButtonEventArgs args)
	{
		foreach (BaseElement element in ElementsAt(args.Position))
		{
			element.MouseClick(args);
			if (args.Handled) return;
		}

		MouseClick(args);
	}

	internal void InternalDoubleClick(MouseButtonEventArgs args)
	{
		foreach (BaseElement element in ElementsAt(args.Position))
		{
			element.DoubleClick(args);
			if (args.Handled) return;
		}

		DoubleClick(args);
	}

	internal void InternalTripleClick(MouseButtonEventArgs args)
	{
		foreach (BaseElement element in ElementsAt(args.Position))
		{
			element.TripleClick(args);
			if (args.Handled) return;
		}

		TripleClick(args);
	}

	internal void InternalMouseMove(MouseMoveEventArgs args)
	{
		foreach (BaseElement element in ElementsAt(args.Position))
		{
			element.MouseMove(args);
		}

		MouseMove(args);
	}

	internal void InternalMouseScroll(MouseScrollEventArgs args)
	{
		foreach (BaseElement element in ElementsAt(args.Position))
		{
			element.MouseScroll(args);
			if (args.Handled) return;
		}

		MouseScroll(args);
	}

	internal void InternalMouseEnter(MouseMoveEventArgs args)
	{
		MouseEnter(args);
	}

	internal void InternalMouseLeave(MouseMoveEventArgs args)
	{
		MouseLeave(args);
	}

	internal void InternalKeyPressed(KeyboardEventArgs args)
	{
		foreach (BaseElement element in Children)
		{
			element.InternalKeyPressed(args);
			if (args.Handled) return;
		}

		KeyPressed(args);
	}

	internal void InternalKeyReleased(KeyboardEventArgs args)
	{
		foreach (BaseElement element in Children)
		{
			element.InternalKeyReleased(args);
			if (args.Handled) return;
		}

		KeyReleased(args);
	}

	internal void InternalKeyTyped(KeyboardEventArgs args)
	{
		foreach (BaseElement element in Children)
		{
			element.InternalKeyTyped(args);
			if (args.Handled) return;
		}

		KeyTyped(args);
	}

	internal void InternalActivate()
	{
		foreach (BaseElement element in Children)
		{
			element.InternalActivate();
		}

		Activate();
	}

	internal void InternalDeactivate()
	{
		foreach (BaseElement element in Children)
		{
			element.InternalDeactivate();
		}

		Deactivate();
	}

	private Rectangle GetClippingRectangle(SpriteBatch spriteBatch)
	{
		Vector2 topLeft = InnerDimensions.TopLeft();
		Vector2 bottomRight = InnerDimensions.BottomRight();

		topLeft = Vector2.Transform(topLeft, Main.UIScaleMatrix);
		bottomRight = Vector2.Transform(bottomRight, Main.UIScaleMatrix);

		int width = spriteBatch.GraphicsDevice.Viewport.Width;
		int height = spriteBatch.GraphicsDevice.Viewport.Height;

		Rectangle result = new()
		{
			X = (int)Utils.Clamp(topLeft.X, 0, width),
			Y = (int)Utils.Clamp(topLeft.Y, 0, height),
			Width = (int)Utils.Clamp(bottomRight.X - topLeft.X, 0, width - topLeft.X),
			Height = (int)Utils.Clamp(bottomRight.Y - topLeft.Y, 0, height - topLeft.Y)
		};
		
		return result;
	}

	internal bool ContainsPoint(Vector2 point) => point.X >= Dimensions.X && point.X <= Dimensions.X + Dimensions.Width && point.Y >= Dimensions.Y && point.Y <= Dimensions.Y + Dimensions.Height;
	
	private List<BaseElement> ElementsAt(Vector2 point)
	{
		List<BaseElement> elements = [];

		foreach (BaseElement element in Children)
		{
			if (!element.ContainsPoint(point) || element.Display == Display.None) continue;
			
			elements.Add(element);
			elements.AddRange(element.ElementsAt(point));
		}

		elements.Reverse();
		return elements;
	}
	
	
	public virtual BaseElement? GetElementAt(Vector2 point)
	{
		BaseElement? element = Children.LastOrDefault(current => current.ContainsPoint(point) && current.Display != Display.None);

		if (element != null) return element.GetElementAt(point);

		return Dimensions.Contains(point) && Display != Display.None ? this : null;
	}
}