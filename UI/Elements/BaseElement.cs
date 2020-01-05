//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using Terraria;
//using Terraria.UI;

//namespace BaseLibrary.UI.Elements
//{
//	public class BaseElement : UIState
//	{
//		private string hoverText
//		{
//			get
//			{
//				StringBuilder builder = new StringBuilder();
//				if (HoverText != null) builder.AddLine(HoverText.ToString());
//				if (GetHoverText != null) builder.Add(GetHoverText.Invoke());
//				return builder.ToString();
//			}
//		}

//		public new (float pixels, float precent) Width
//		{
//			get => (base.Width.Pixels, base.Width.Precent);
//			set
//			{
//				base.Width.Pixels = value.pixels;
//				base.Width.Precent = value.precent;
//			}
//		}

//		public new (float pixels, float precent) Height
//		{
//			get => (base.Height.Pixels, base.Height.Precent);
//			set
//			{
//				base.Height.Pixels = value.pixels;
//				base.Height.Precent = value.precent;
//			}
//		}

//		public Vector2 Size
//		{
//			get => Dimensions.Size();
//			set
//			{
//				base.Width.Pixels = value.X;
//				base.Height.Pixels = value.Y;
//			}
//		}

//		public new (float pixels, float precent) Top
//		{
//			get => (base.Top.Pixels, base.Top.Precent);
//			set
//			{
//				base.Top.Pixels = value.pixels;
//				base.Top.Precent = value.precent;
//			}
//		}

//		public new (float pixels, float precent) Left
//		{
//			get => (base.Left.Pixels, base.Left.Precent);
//			set
//			{
//				base.Left.Pixels = value.pixels;
//				base.Left.Precent = value.precent;
//			}
//		}

//		public Vector2 Position
//		{
//			get => Dimensions.Position();
//			set
//			{
//				base.Left.Pixels = value.X;
//				base.Top.Pixels = value.Y;
//			}
//		}

//		public (float bottom, float left, float right, float top) Padding
//		{
//			get => (PaddingBottom, PaddingLeft, PaddingRight, PaddingTop);
//			set
//			{
//				PaddingBottom = value.bottom;
//				PaddingLeft = value.left;
//				PaddingRight = value.right;
//				PaddingTop = value.top;
//			}
//		}

//		public new List<BaseElement> Elements
//		{
//			get => base.Elements;
//			set => base.Elements = value;
//		}

//		public CalculatedStyle Dimensions => GetDimensions();
//		public CalculatedStyle InnerDimensions => GetInnerDimensions();
//		public CalculatedStyle OuterDimensions => GetOuterDimensions();

//		public new BaseElement Parent
//		{
//			get => base.Parent as BaseElement;
//			set => base.Parent = value;
//		}

//		public object HoverText = null;
//		public bool SubstituteHeight;

//		public bool SubstituteWidth;

//		public bool Visible = true;

//		public BaseElement() => base.Width.Precent = base.Height.Precent = 0;

//		public void Add(IEnumerable<BaseElement> elements)
//		{
//			foreach (BaseElement element in elements)
//			{
//				element.Activate();
//				Add(element);
//			}
//		}

//		public virtual void ClickContinuous(UIMouseEvent evt)
//		{
//			OnClickContinuous?.Invoke(evt, this);
//			Parent?.ClickContinuous(evt);
//		}

//		public override void Draw(SpriteBatch spriteBatch)
//		{
//			if (!Visible) return;

//			PreDraw(spriteBatch);

//			if (_useImmediateMode) spriteBatch.Draw(Utility.ImmediateState, () => DrawSelf(spriteBatch));
//			else DrawSelf(spriteBatch);

//			if (OverflowHidden) spriteBatch.DrawOverflowHidden(this, () => DrawChildren(spriteBatch));
//			else DrawChildren(spriteBatch);

//			PostDraw(spriteBatch);

//			if (IsMouseHovering && !string.IsNullOrWhiteSpace(hoverText)) Utility.DrawMouseText(hoverText);
//		}

//		protected override void DrawChildren(SpriteBatch spriteBatch)
//		{
//			foreach (BaseElement current in Elements) current.Draw(spriteBatch);
//		}

//		public event Func<object> GetHoverText;

//		public void Insert(int index, BaseElement element)
//		{
//			element.Remove();
//			element.Parent = this;
//			Elements.Insert(index, element);
//			element.Recalculate();
//		}

//		public virtual void MiddleTripleClick(UIMouseEvent evt)
//		{
//			OnMiddleTripleClick?.Invoke(evt, this);
//			Parent?.MiddleTripleClick(evt);
//		}

//		public event MouseEvent OnClickContinuous;
//		public event MouseEvent OnMiddleTripleClick;
//		public event Action<SpriteBatch> OnPostDraw;
//		public event Action<SpriteBatch> OnPreDraw;
//		public event MouseEvent OnRightClickContinuous;
//		public event MouseEvent OnRightTripleClick;

//		public event MouseEvent OnTripleClick;
//		public event MouseEvent OnXButton1TripleClick;
//		public event MouseEvent OnXButton2TripleClick;

//		public virtual void PostDraw(SpriteBatch spriteBatch)
//		{
//			OnPostDraw?.Invoke(spriteBatch);
//		}

//		public virtual void PreDraw(SpriteBatch spriteBatch)
//		{
//			OnPreDraw?.Invoke(spriteBatch);
//		}

//		public override void Recalculate()
//		{
//			CalculatedStyle parentDimensions = Parent?.InnerDimensions ?? UserInterface.ActiveInstance.GetDimensions();

//			CalculatedStyle dimensions;
//			dimensions.X = base.Left.GetValue(parentDimensions.Width) + parentDimensions.X;
//			dimensions.Y = base.Top.GetValue(parentDimensions.Height) + parentDimensions.Y;

//			float minWidth = MinWidth.GetValue(parentDimensions.Width);
//			float maxWidth = MaxWidth.GetValue(parentDimensions.Width);
//			float minHeight = MinHeight.GetValue(parentDimensions.Height);
//			float maxHeight = MaxHeight.GetValue(parentDimensions.Height);

//			dimensions.Width = base.Width.GetValue(parentDimensions.Width).Clamp(minWidth, maxWidth);
//			dimensions.Height = base.Height.GetValue(parentDimensions.Height).Clamp(minHeight, maxHeight);

//			if (SubstituteWidth) dimensions.Width = dimensions.Height;
//			else if (SubstituteHeight) dimensions.Height = dimensions.Width;

//			dimensions.Width += MarginLeft + MarginRight;
//			dimensions.Height += MarginTop + MarginBottom;
//			dimensions.X += parentDimensions.Width * HAlign - dimensions.Width * HAlign;
//			dimensions.Y += parentDimensions.Height * VAlign - dimensions.Height * VAlign;
//			typeof(BaseElement).SetValue("_outerDimensions", dimensions, this);
//			dimensions.X += MarginLeft;
//			dimensions.Y += MarginTop;
//			dimensions.Width -= MarginLeft + MarginRight;
//			dimensions.Height -= MarginTop + MarginBottom;
//			typeof(BaseElement).SetValue("_dimensions", dimensions, this);
//			dimensions.X += PaddingLeft;
//			dimensions.Y += PaddingTop;
//			dimensions.Width -= PaddingLeft + PaddingRight;
//			dimensions.Height -= PaddingTop + PaddingBottom;
//			typeof(BaseElement).SetValue("_innerDimensions", dimensions, this);

//			RecalculateChildren();
//		}

//		public void Remove(IEnumerable<BaseElement> elements)
//		{
//			foreach (BaseElement element in elements) Remove(element);
//		}

//		public virtual void RightClickContinuous(UIMouseEvent evt)
//		{
//			OnRightClickContinuous?.Invoke(evt, this);
//			Parent?.RightClickContinuous(evt);
//		}

//		public virtual void RightTripleClick(UIMouseEvent evt)
//		{
//			OnRightTripleClick?.Invoke(evt, this);
//			Parent?.RightTripleClick(evt);
//		}

//		public virtual void TripleClick(UIMouseEvent evt)
//		{
//			OnTripleClick?.Invoke(evt, this);
//			Parent?.TripleClick(evt);
//		}

//		protected override void Update(GameTime gameTime)
//		{
//			base.Update(gameTime);

//			if (!Visible) return;

//			if (Main.hasFocus && IsMouseHovering)
//			{
//				if (Main.mouseLeft) ClickContinuous(new UIMouseEvent(this, UserInterface.ActiveInstance.MousePosition));
//				if (Main.mouseRight) RightClickContinuous(new UIMouseEvent(this, UserInterface.ActiveInstance.MousePosition));
//			}
//		}

//		public virtual void XButton1TripleClick(UIMouseEvent evt)
//		{
//			OnXButton1TripleClick?.Invoke(evt, this);
//			Parent?.XButton1TripleClick(evt);
//		}

//		public virtual void XButton2TripleClick(UIMouseEvent evt)
//		{
//			OnXButton2TripleClick?.Invoke(evt, this);
//			Parent?.XButton2TripleClick(evt);
//		}
//	}
//}

