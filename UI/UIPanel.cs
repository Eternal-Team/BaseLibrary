using System.Collections.Generic;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace BaseLibrary.UI;

public class DragZone
{
	public static readonly DragZone Panel = new() { Width = { Percent = 100 }, Height = { Percent = 100 } };
	public static readonly DragZone LeftBorder = new() { Width = { Pixels = 8 }, Height = { Percent = 100 } };
	public static readonly DragZone RightBorder = new() { X = { Percent = 100 }, Width = { Pixels = 8 }, Height = { Percent = 100 } };
	public static readonly DragZone TopBorder = new() { Width = { Percent = 100 }, Height = { Pixels = 8 } };
	public static readonly DragZone BottomBorder = new() { Y = { Percent = 100 }, Width = { Percent = 100 }, Height = { Pixels = 8 } };

	public static readonly List<DragZone> Border = new()
	{
		LeftBorder,
		RightBorder,
		TopBorder,
		BottomBorder
	};

	public StyleDimension X = new();
	public StyleDimension Y = new();
	public StyleDimension Width = new();
	public StyleDimension Height = new();
}

public struct UIPanelSettings
{
	public static readonly UIPanelSettings Default = new()
	{
		BackgroundColor = UICommon.DefaultUIBlue,
		BorderColor = Color.Black,
		Draggable = false,
		Resizeable = false,
		DragZones = new List<DragZone> { DragZone.Panel },
		Texture = null
	};

	public Color BackgroundColor;
	public Color BorderColor;
	public bool Draggable;
	public List<DragZone> DragZones;
	public bool Resizeable;
	public Texture2D Texture;
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
		if (!Settings.Draggable || args.Button != MouseButton.Left || GetElementAt(args.Position) != this) return;

		foreach (var zone in Settings.DragZones)
		{
			Rectangle parent = Dimensions;

			Rectangle dimensions = Rectangle.Empty;
			dimensions.Width = (int)(zone.Width.Percent * parent.Width / 100f + zone.Width.Pixels);
			dimensions.Height = (int)(zone.Height.Percent * parent.Height / 100f + zone.Height.Pixels);
			dimensions.X = (int)(parent.X + (zone.X.Percent * parent.Width / 100f - dimensions.Width * zone.X.Percent / 100f) + zone.X.Pixels);
			dimensions.Y = (int)(parent.Y + (zone.Y.Percent * parent.Height / 100f - dimensions.Height * zone.Y.Percent / 100f) + zone.Y.Pixels);

			if (dimensions.Contains(args.Position))
			{
				offset = args.Position - Position;

				dragging = true;

				args.Handled = true;

				return;
			}
		}
	}

	protected override void MouseUp(MouseButtonEventArgs args)
	{
		if (!Settings.Draggable || args.Button != MouseButton.Left) return;

		dragging = false;

		args.Handled = true;
	}

	protected override void Update(GameTime gameTime)
	{
		if (dragging)
		{
			X.Percent = 0;
			Y.Percent = 0;

			Rectangle parent = Parent?.InnerDimensions ?? UserInterface.ActiveInstance.GetDimensions().ToRectangle();

			X.Pixels = Utils.Clamp((int)(Main.mouseX - offset.X - parent.X), 0, parent.Width - OuterDimensions.Width);
			Y.Pixels = Utils.Clamp((int)(Main.mouseY - offset.Y - parent.Y), 0, parent.Height - OuterDimensions.Height);

			Recalculate();
		}
	}
	#endregion

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
		}

		if (Settings.Texture != null) spriteBatch.Draw(Settings.Texture, Dimensions);
		else DrawingUtility.DrawPanel(spriteBatch, Dimensions, Settings.BackgroundColor, Settings.BorderColor);
	}
}