using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using BaseLibrary.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace BaseLibrary.UI;

public struct DragZone(Dimension size, Dimension position)
{
	public static readonly DragZone Panel = new DragZone(Dimension.FromPercent(100), Dimension.Zero);
	// public static readonly DragZone LeftBorder = new(new Dimension(8, 0, 0, 100), Dimension.Zero);
	// public static readonly DragZone RightBorder = new() { X = { Percent = 100 }, Width = { Pixels = 8 }, Height = { Percent = 100 } };
	// public static readonly DragZone TopBorder = new() { Width = { Percent = 100 }, Height = { Pixels = 8 } };
	// public static readonly DragZone BottomBorder = new() { Y = { Percent = 100 }, Width = { Percent = 100 }, Height = { Pixels = 8 } };

	// public static readonly List<DragZone> Border =
	// [
	// 	LeftBorder,
	// 	RightBorder,
	// 	TopBorder,
	// 	BottomBorder
	// ];

	public Dimension Size = size;
	public Dimension Position = position;
}

public struct UIPanelSettings
{
	public static readonly UIPanelSettings Default = new UIPanelSettings {
		BackgroundColor = UICommon.DefaultUIBlue,
		BorderColor = Color.Black,
		Draggable = false,
		Resizeable = false,
		DragZones = [DragZone.Panel],
		Texture = null,
		CaptureAllInputs = false
	};

	public Color BackgroundColor;
	public Color BorderColor;
	public bool Draggable;
	public bool CaptureAllInputs;
	public List<DragZone> DragZones;
	public bool Resizeable;
	public Asset<Texture2D>? Texture;
}

public class UIPanel : BaseElement
{
	public UIPanelSettings Settings = UIPanelSettings.Default;

	public UIPanel()
	{
		Padding = new Padding(8);
	}

	#region Dragging

	private Vector2 offset;
	private bool dragging;

	protected override void MouseDown(MouseButtonEventArgs args)
	{
		if (Settings.CaptureAllInputs) args.Handled = true;

		if (!Settings.Draggable || args.Button != MouseButton.Left /*|| GetElementAt(args.Position) != this*/)
		{
			base.MouseDown(args);
			return;
		}

		foreach (DragZone zone in Settings.DragZones)
		{
			Rectangle parent = Dimensions;

			Rectangle dimensions = Rectangle.Empty;
			dimensions.Width = (int)(zone.Size.PercentX * parent.Width * 0.01f + zone.Size.PixelsX);
			dimensions.Height = (int)(zone.Size.PercentY * parent.Height * 0.01f + zone.Size.PixelsY);
			dimensions.X = (int)(parent.X + (zone.Size.PercentX * parent.Width * 0.01f - dimensions.Width * zone.Size.PercentX * 0.01f) + zone.Size.PixelsX);
			dimensions.Y = (int)(parent.Y + (zone.Size.PercentY * parent.Height * 0.01f - dimensions.Height * zone.Size.PercentY * 0.01f) + zone.Size.PixelsY);

			if (!dimensions.Contains(args.Position)) continue;

			offset = (args.Position - dimensions.TopLeft()).Floor();
			dragging = true;
			args.Handled = true;

			return;
		}
	}

	protected override void MouseUp(MouseButtonEventArgs args)
	{
		// if (Settings.CaptureAllInputs) args.Handled = true;

		if (!Settings.Draggable || args.Button != MouseButton.Left)
		{
			base.MouseUp(args);
			return;
		}

		dragging = false;

		args.Handled = true;
	}

	protected override void Update(GameTime gameTime)
	{
		if (!dragging) return;

		Position.PercentX = 0;
		Position.PercentY = 0;

		Rectangle parent = Parent?.InnerDimensions ?? UserInterface.ActiveInstance.GetDimensions().ToRectangle();

		Position.PixelsX = Utils.Clamp((int)(Main.mouseX - offset.X - parent.X), 0, parent.Width - OuterDimensions.Width);
		Position.PixelsY = Utils.Clamp((int)(Main.mouseY - offset.Y - parent.Y), 0, parent.Height - OuterDimensions.Height);

		Recalculate();
	}

	#endregion

	protected override void MouseHeld(MouseButtonEventArgs args)
	{
		base.MouseHeld(args);

		if (Settings.CaptureAllInputs) args.Handled = true;
	}

	protected override void MouseScroll(MouseScrollEventArgs args)
	{
		base.MouseScroll(args);

		if (Settings.CaptureAllInputs) args.Handled = true;
	}

	protected override void MouseClick(MouseButtonEventArgs args)
	{
		base.MouseClick(args);

		if (Settings.CaptureAllInputs) args.Handled = true;
	}

	protected static Type? TypeMouseTextCache = typeof(Main).GetNestedType("MouseTextCache", BindingFlags.NonPublic);
	protected static FieldInfo? FieldIsValid = TypeMouseTextCache.GetField("isValid");
	protected static FieldInfo? FieldMouseTextCache = typeof(Main).GetField("_mouseTextCache", BindingFlags.Instance | BindingFlags.NonPublic);

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_achievementAdvisor")]
	protected static extern ref AchievementAdvisor? FieldAchievementAdvisor(Main? c);

	protected override void Draw(SpriteBatch spriteBatch)
	{
		if (IsMouseHovering)
		{
			Main.LocalPlayer.mouseInterface = true;
			Main.instance.SetMouseNPC(-1, -1);
			Main.LocalPlayer.cursorItemIconEnabled = false;
			Main.LocalPlayer.cursorItemIconText = string.Empty;
			Main.signHover = -1;
			Main.ItemIconCacheUpdate(0);
			Main.mouseText = true;
			Main.HoverItem = new Item();
			Main.hoverItemName = "";

			object mouseTextCache = FieldMouseTextCache.GetValue(Main.instance);
			FieldIsValid.SetValue(mouseTextCache, false);
			FieldMouseTextCache.SetValue(Main.instance, mouseTextCache);

			FieldAchievementAdvisor(Main.instance)?.Update();
		}

		if (Settings.Texture is not null) spriteBatch.Draw(Settings.Texture.Value, Dimensions, Color.White);
		else DrawingUtility.DrawPanel(spriteBatch, Dimensions, Settings.BackgroundColor, Settings.BorderColor);
	}
}