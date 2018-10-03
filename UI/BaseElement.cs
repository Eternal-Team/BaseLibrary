using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using static BaseLibrary.Utility.Utility;

namespace BaseLibrary.UI
{
	// todo: introduce Vector2 for position and size
	// todo: make left,right,top,left floats
	// todo: functions for size and position?

	public class BaseElement : UIElement
	{
		public event Action<SpriteBatch> OnPreDraw;
		public event Action<SpriteBatch> OnPostDraw;
		public event Func<string> GetHoverText;

		public bool substituteWidth;
		public bool substituteHeight;

		//public (float pixels, float precent) Widthtest
		//{
		//	get => (Width.Pixels, Width.Precent);
		//	set
		//	{
		//		Width.Pixels = value.pixels;
		//		Width.Precent = value.precent;
		//	}
		//}

		public override void Recalculate()
		{
			CalculatedStyle parentDimensions = Parent?.GetInnerDimensions() ?? UserInterface.ActiveInstance.GetDimensions();
			if (Parent is UIList) parentDimensions.Height = float.MaxValue;

			CalculatedStyle dimensions;
			dimensions.X = Left.GetValue(parentDimensions.Width) + parentDimensions.X;
			dimensions.Y = Top.GetValue(parentDimensions.Height) + parentDimensions.Y;

			float minWidth = MinWidth.GetValue(parentDimensions.Width);
			float maxWidth = MaxWidth.GetValue(parentDimensions.Width);
			float minHeight = MinHeight.GetValue(parentDimensions.Height);
			float maxHeight = MaxHeight.GetValue(parentDimensions.Height);


			dimensions.Width = Width.GetValue(parentDimensions.Width).Clamp(minWidth, maxWidth);
			dimensions.Height = Height.GetValue(parentDimensions.Height).Clamp(minHeight, maxHeight);

			if (substituteWidth) dimensions.Width = dimensions.Height;
			else if (substituteHeight) dimensions.Height = dimensions.Width;

			dimensions.Width += MarginLeft + MarginRight;
			dimensions.Height += MarginTop + MarginBottom;
			dimensions.X += parentDimensions.Width * HAlign - dimensions.Width * HAlign;
			dimensions.Y += parentDimensions.Height * VAlign - dimensions.Height * VAlign;
			typeof(UIElement).SetValue("_outerDimensions", dimensions, this);
			dimensions.X += MarginLeft;
			dimensions.Y += MarginTop;
			dimensions.Width -= MarginLeft + MarginRight;
			dimensions.Height -= MarginTop + MarginBottom;
			typeof(UIElement).SetValue("_dimensions", dimensions, this);
			dimensions.X += PaddingLeft;
			dimensions.Y += PaddingTop;
			dimensions.Width -= PaddingLeft + PaddingRight;
			dimensions.Height -= PaddingTop + PaddingBottom;
			typeof(UIElement).SetValue("_innerDimensions", dimensions, this);
			RecalculateChildren();
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			//if (Visible && (VisibleFunc?.Invoke() ?? true))
			//{
			PreDraw(spriteBatch);

			if (_useImmediateMode) spriteBatch.DrawImmediate(DrawSelf);
			else DrawSelf(spriteBatch);

			if (OverflowHidden) spriteBatch.DrawOverflowHidden(this, DrawChildren);
			else DrawChildren(spriteBatch);

			PostDraw(spriteBatch);

			if (IsMouseHovering && GetHoverText != null) DrawMouseText(GetHoverText.Invoke());
			//}
		}

		public virtual void AppendRange(IEnumerable<BaseElement> elements)
		{
			foreach (BaseElement element in elements) Append(element);
		}

		public virtual void RemoveRange(IEnumerable<BaseElement> elements)
		{
			foreach (BaseElement element in elements) RemoveChild(element);
		}

		public virtual void PreDraw(SpriteBatch spriteBatch)
		{
			OnPreDraw?.Invoke(spriteBatch);
		}

		public virtual void PostDraw(SpriteBatch spriteBatch)
		{
			OnPostDraw?.Invoke(spriteBatch);
		}
	}
}