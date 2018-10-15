using System;
using System.Collections.Generic;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace BaseLibrary.UI.Elements
{
	// todo: introduce Vector2 for position and size
	// todo: make left,right,top,left floats
	// todo: functions for size and position?

	public class BaseElement : UIState
	{
		#region Fields
		public event Action<SpriteBatch> OnPreDraw;
		public event Action<SpriteBatch> OnPostDraw;
		public event Func<string> GetHoverText;

		public bool substituteWidth;
		public bool substituteHeight;

		public new (float pixels, float precent) Width
		{
			get => (base.Width.Pixels, base.Width.Precent);
			set
			{
				base.Width.Pixels = value.pixels;
				base.Width.Precent = value.precent;
			}
		}

		public new (float pixels, float precent) Height
		{
			get => (base.Height.Pixels, base.Height.Precent);
			set
			{
				base.Height.Pixels = value.pixels;
				base.Height.Precent = value.precent;
			}
		}

		public Vector2 Size
		{
			get => GetDimensions().Size();
			set
			{
				base.Width.Pixels = value.X;
				base.Height.Pixels = value.Y;
			}
		}

		public new (float pixels, float precent) Top
		{
			get => (base.Top.Pixels, base.Top.Precent);
			set
			{
				base.Top.Pixels = value.pixels;
				base.Top.Precent = value.precent;
			}
		}

		public new (float pixels, float precent) Left
		{
			get => (base.Left.Pixels, base.Left.Precent);
			set
			{
				base.Left.Pixels = value.pixels;
				base.Left.Precent = value.precent;
			}
		}

		public Vector2 Position
		{
			get => GetDimensions().Position();
			set
			{
				base.Left.Pixels = value.X;
				base.Top.Pixels = value.Y;
			}
		}

		public (float bottom, float left, float right, float top) Padding
		{
			get => (PaddingBottom, PaddingLeft, PaddingRight, PaddingTop);
			set
			{
				PaddingBottom = value.bottom;
				PaddingLeft = value.left;
				PaddingRight = value.right;
				PaddingTop = value.top;
			}
		}

		public new List<UIElement> Elements
		{
			get => base.Elements;
			set => base.Elements = value;
		}

		public event MouseEvent OnClickContinuous;
		public event MouseEvent OnRightClickContinuous;
		#endregion

		public BaseElement() => base.Width.Precent = base.Height.Precent = 0;

		public virtual void ClickContinuous(UIMouseEvent evt) => OnClickContinuous?.Invoke(evt, this);

		public virtual void RightClickContinuous(UIMouseEvent evt) => OnRightClickContinuous?.Invoke(evt, this);

		public override void Update(GameTime gameTime)
		{
			if (Main.hasFocus && IsMouseHovering)
			{
				if (Main.mouseLeft) ClickContinuous(new UIMouseEvent(this, UserInterface.ActiveInstance.MousePosition));
				if (Main.mouseRight) RightClickContinuous(new UIMouseEvent(this, UserInterface.ActiveInstance.MousePosition));
			}

			base.Update(gameTime);
		}

		public override void Recalculate()
		{
			CalculatedStyle parentDimensions = Parent?.GetInnerDimensions() ?? UserInterface.ActiveInstance.GetDimensions();
			if (Parent is UIList) parentDimensions.Height = float.MaxValue;

			CalculatedStyle dimensions;
			dimensions.X = base.Left.GetValue(parentDimensions.Width) + parentDimensions.X;
			dimensions.Y = base.Top.GetValue(parentDimensions.Height) + parentDimensions.Y;

			float minWidth = MinWidth.GetValue(parentDimensions.Width);
			float maxWidth = MaxWidth.GetValue(parentDimensions.Width);
			float minHeight = MinHeight.GetValue(parentDimensions.Height);
			float maxHeight = MaxHeight.GetValue(parentDimensions.Height);

			dimensions.Width = base.Width.GetValue(parentDimensions.Width).Clamp(minWidth, maxWidth);
			dimensions.Height = base.Height.GetValue(parentDimensions.Height).Clamp(minHeight, maxHeight);

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

			if (IsMouseHovering && GetHoverText != null) Utility.Utility.DrawMouseText(GetHoverText.Invoke());
			//}
		}

		public virtual void Append(IEnumerable<BaseElement> elements)
		{
			foreach (BaseElement element in elements) Append(element);
		}

		public virtual void RemoveChildren(IEnumerable<BaseElement> elements)
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