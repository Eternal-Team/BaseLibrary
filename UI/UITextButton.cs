using System;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;

namespace BaseLibrary.UI
{
	public struct UITextButtonOptions
	{
		public static readonly UITextButtonOptions Default = new UITextButtonOptions
		{
			TextColor = Color.White,
			BorderColor = Color.Black,
			PanelColor = DrawingUtility.Colors.Panel,
			PanelHoveredColor = DrawingUtility.Colors.PanelHovered,
			PanelSelectedColor = DrawingUtility.Colors.PanelSelected,
			PanelDisabledColor = DrawingUtility.Colors.PanelDisabled,
			HorizontalAlignment = HorizontalAlignment.Left,
			VerticalAlignment = VerticalAlignment.Top,
			ScaleToFit = false,
			Font = FontAssets.MouseText.Value,
			Disabled = false
		};

		public Color TextColor;
		public Color BorderColor;
		public Color PanelColor;
		public Color PanelHoveredColor;
		public Color PanelSelectedColor;
		public Color PanelDisabledColor;

		public HorizontalAlignment HorizontalAlignment;
		public VerticalAlignment VerticalAlignment;
		public bool ScaleToFit;
		public DynamicSpriteFont Font;
		public bool Disabled;
	}

	public class UITextButton : BaseElement
	{
		public UITextButtonOptions Settings = UITextButtonOptions.Default;

		public object Text
		{
			get => text;
			set
			{
				text = value;
				CalculateTextMetrics();
			}
		}

		private object text;

		private Vector2 textSize;
		private Vector2 textPosition;
		private float textScale;

		public UITextButton(string text, float scale = 1f)
		{
			this.text = text;
			Padding = new Padding(8);

			this.text = text;
			Settings.Font = scale > 1f ? FontAssets.DeathText.Value : FontAssets.MouseText.Value;
			textScale = scale > 1f ? scale * 0.5f : scale;
		}

		public UITextButton(LocalizedText text, float scale = 1f)
		{
			this.text = text.ToString();
			Padding = new Padding(8);

			this.text = text.ToString();
			Settings.Font = scale > 1f ? FontAssets.DeathText.Value : FontAssets.MouseText.Value;
			textScale = scale > 1f ? scale * 0.5f : scale;
		}

		// protected override void MouseEnter(MouseEventArgs args)
		// {
		// 	base.MouseEnter(args);
		//
		// 	SoundEngine.PlaySound(SoundID.MenuTick);
		// }
		//
		// protected override void MouseLeave(MouseEventArgs args)
		// {
		// 	base.MouseLeave(args);
		//
		// 	SoundEngine.PlaySound(SoundID.MenuTick);
		// }
		//
		// protected override void MouseClick(MouseButtonEventArgs args)
		// {
		// 	base.MouseClick(args);
		//
		// 	if (args.Button != MouseButton.Left) return;
		//
		// 	// if (Toggleable) Selected = true;
		// 	SoundEngine.PlaySound(SoundID.MenuTick);
		// }

		public override void Recalculate()
		{
			base.Recalculate();

			CalculateTextMetrics();
		}

		protected override void Draw(SpriteBatch spriteBatch)
		{
			// if (Toggleable) spriteBatch.DrawPanel(Dimensions, IsMouseHovering ? Utility.ColorPanel_Hovered : Selected ? Utility.ColorPanel : Utility.ColorPanel_Selected);
			// else 

			Color color = Settings.PanelColor;
			if (IsMouseHovering) color = Main.mouseLeft ? Settings.PanelSelectedColor : Settings.PanelHoveredColor;
			if (Settings.Disabled) color = Settings.PanelDisabledColor;
			
			DrawingUtility.DrawPanel(spriteBatch, Dimensions, color);

			RasterizerState rasterizer = new RasterizerState { CullMode = CullMode.None, ScissorTestEnable = true };

			spriteBatch.End();

			SamplerState samplerText = SamplerState.LinearClamp;
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, samplerText, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);

			Utils.DrawBorderStringFourWay(spriteBatch, Settings.Font, text.ToString(), textPosition.X, textPosition.Y, Settings.TextColor, Settings.BorderColor, Vector2.Zero, textScale);

			spriteBatch.End();

			SamplerState sampler = SamplerState.PointClamp;
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);
		}

		private void CalculateTextMetrics()
		{
			if (text == null || string.IsNullOrWhiteSpace(text.ToString()))
			{
				textSize = Vector2.Zero;
				textPosition = Vector2.Zero;
				return;
			}

			textSize = Settings.Font.MeasureString(text.ToString());
			if (Settings.ScaleToFit) textScale = Math.Min(InnerDimensions.Width / textSize.X, InnerDimensions.Height / textSize.Y);
			textSize *= textScale;

			var hAlign = Settings.HorizontalAlignment;
			var vAlign = Settings.VerticalAlignment;

			if (hAlign == HorizontalAlignment.Left) textPosition.X = InnerDimensions.X;
			else if (hAlign == HorizontalAlignment.Center) textPosition.X = InnerDimensions.X + InnerDimensions.Width * 0.5f - textSize.X * 0.5f;
			else if (hAlign == HorizontalAlignment.Right) textPosition.X = InnerDimensions.X + InnerDimensions.Width - textSize.X;

			if (vAlign == VerticalAlignment.Top) textPosition.Y = InnerDimensions.Y;
			else if (vAlign == VerticalAlignment.Center) textPosition.Y = InnerDimensions.Y + InnerDimensions.Height * 0.5f - textSize.Y * 0.5f + 4f * textScale;
			else if (vAlign == VerticalAlignment.Bottom) textPosition.Y = InnerDimensions.Y + InnerDimensions.Height - textSize.Y + 8f * textScale;
		}
	}
}