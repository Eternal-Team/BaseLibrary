using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Starbound.Input;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace BaseLibrary.UI.Elements
{
	// note: make left,right,top,left floats
	// note: functions for size and position?

	public class ChildrenCollection<T> : IEnumerable<T> where T : BaseElement
	{
		private List<T> _collection = new List<T>();

		public IEnumerator<T> GetEnumerator() => _collection.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();

		public void Add(T element)
		{
			_collection.Add(element);
		}
	}

	public class BaseElement : UIState
	{
		public ChildrenCollection<BaseElement> Children = new ChildrenCollection<BaseElement>();

		#region Fields
		public event Action<SpriteBatch> OnPreDraw;
		public event Action<SpriteBatch> OnPostDraw;
		public event Func<string> GetHoverText;

		public bool SubstituteWidth;
		public bool SubstituteHeight;

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

		// todo: use MouseEvents
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

		public event Action<CalculatedStyle> OnSizeChanged;

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

			if (SubstituteWidth) dimensions.Width = dimensions.Height;
			else if (SubstituteHeight) dimensions.Height = dimensions.Width;

			dimensions.Width += MarginLeft + MarginRight;
			dimensions.Height += MarginTop + MarginBottom;
			dimensions.X += parentDimensions.Width * HAlign - dimensions.Width * HAlign;
			dimensions.Y += parentDimensions.Height * VAlign - dimensions.Height * VAlign;
			typeof(UIElement).SetValue("_outerDimensions", dimensions, this);
			dimensions.X += MarginLeft;
			dimensions.Y += MarginTop;
			dimensions.Width -= MarginLeft + MarginRight;
			dimensions.Height -= MarginTop + MarginBottom;
			if (dimensions.Size() != GetDimensions().Size()) OnSizeChanged?.Invoke(dimensions);
			typeof(UIElement).SetValue("_dimensions", dimensions, this);
			dimensions.X += PaddingLeft;
			dimensions.Y += PaddingTop;
			dimensions.Width -= PaddingLeft + PaddingRight;
			dimensions.Height -= PaddingTop + PaddingBottom;
			typeof(UIElement).SetValue("_innerDimensions", dimensions, this);
			RecalculateChildren();
		}

		private static readonly Utility.SpriteBatchState immediateState = new Utility.SpriteBatchState
		{
			spriteSortMode = SpriteSortMode.Immediate,
			blendState = BlendState.AlphaBlend,
			samplerState = SamplerState.AnisotropicClamp,
			depthStencilState = DepthStencilState.None,
			rasterizerState = Utility.OverflowHiddenState,
			customEffect = null,
			transformMatrix = Main.UIScaleMatrix
		};

		public override void Draw(SpriteBatch spriteBatch)
		{
			PreDraw(spriteBatch);

			if (_useImmediateMode) spriteBatch.DrawWithState(immediateState, DrawSelf);
			else DrawSelf(spriteBatch);

			if (OverflowHidden) spriteBatch.DrawOverflowHidden(this, DrawChildren);
			else DrawChildren(spriteBatch);

			PostDraw(spriteBatch);

			if (IsMouseHovering && GetHoverText != null) Utility.DrawMouseText(GetHoverText.Invoke());
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