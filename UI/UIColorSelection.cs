using System;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace BaseLibrary.UI;

// todo: add different styles

public struct UIColorSelectionSettings
{
	public static readonly UIColorSelectionSettings Default = new()
	{
		Style = UIColorSelection.Style.Hue
	};

	public UIColorSelection.Style Style;
}

public class UIColorSelection : BaseElement
{
	public enum Style
	{
		Hue
	}

	public UIColorSelectionSettings Settings = UIColorSelectionSettings.Default;
	public Action<Color>? OnColorChange;

	private readonly UITexture colorDot;

	public UIColorSelection()
	{
		UIGradient gradient = new UIGradient
		{
			Width = { Percent = 100 },
			Height = { Percent = 100 }
		};
		gradient.OnMouseHeld += args =>
		{
			if (args.Button != MouseButton.Left)
				return;

			args.Handled = true;

			Vector2 selectedPosRelative = (args.Position - Dimensions.TopLeft()) / Dimensions.Size();
			if (selectedPosRelative.X > 1f) selectedPosRelative.X = 1f;

			Color selectedColor = ColorUtility.FromHSV(selectedPosRelative.X, 1f, 1f);

			if (colorDot != null)
			{
				colorDot.Settings.Color = selectedColor;
				colorDot.X.Pixels = (int)(gradient.Dimensions.Width * selectedPosRelative.X);
				colorDot.Y.Pixels = (int)(gradient.Dimensions.Height * selectedPosRelative.Y);
				colorDot.Recalculate();
			}

			OnColorChange?.Invoke(selectedColor);
		};
		Add(gradient);

		colorDot = new UITexture(Main.Assets.Request<Texture2D>("Images/UI/Settings_Toggle"))
		{
			Size = new Vector2(14f),
			Settings =
			{
				SourceRectangle = new Rectangle(16, 0, 14, 14),
				Origin = new Vector2(7f),
				ImageX = { Percent = 0 },
				ImageY = { Percent = 0 }
			}
		};
		gradient.Add(colorDot);
	}

	public void SetColor(Color color)
	{
		Vector3 hsv = ColorUtility.ToHSV(color);

		colorDot.Settings.Color = color;
		if (colorDot.Parent != null)
		{
			colorDot.X.Pixels = (int)(colorDot.Parent.Dimensions.Width * hsv.X);
			colorDot.Y.Pixels = colorDot.Parent.Dimensions.Height / 2;
		}

		colorDot.Recalculate();

		OnColorChange?.Invoke(color);
	}
}