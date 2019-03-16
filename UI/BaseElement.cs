//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Terraria;
//using Terraria.UI;

//namespace BaseLibrary.UI
//{
//	public class BaseElement : IList<BaseElement>, IComparable
//	{
//		public BaseElement Parent;

//		private List<BaseElement> children = new List<BaseElement>();

//		public StyleDimension Top;
//		public StyleDimension Left;
//		public StyleDimension Width;
//		public StyleDimension Height;
//		public StyleDimension MaxWidth = StyleDimension.Fill;
//		public StyleDimension MaxHeight = StyleDimension.Fill;
//		public StyleDimension MinWidth = StyleDimension.Empty;
//		public StyleDimension MinHeight = StyleDimension.Empty;

//		public float PaddingTop;
//		public float PaddingLeft;
//		public float PaddingRight;
//		public float PaddingBottom;
//		public float MarginTop;
//		public float MarginLeft;
//		public float MarginRight;
//		public float MarginBottom;
//		public float HAlign;
//		public float VAlign;
//		private CalculatedStyle _innerDimensions;
//		private CalculatedStyle _dimensions;
//		private CalculatedStyle _outerDimensions;

//		public event UIElement.MouseEvent OnMouseDown;
//		public event UIElement.MouseEvent OnMouseUp;
//		public event UIElement.MouseEvent OnClick;
//		public event UIElement.MouseEvent OnMouseOver;
//		public event UIElement.MouseEvent OnMouseOut;
//		public event UIElement.MouseEvent OnDoubleClick;
//		public event UIElement.MouseEvent OnRightMouseDown;
//		public event UIElement.MouseEvent OnRightMouseUp;
//		public event UIElement.MouseEvent OnRightClick;
//		public event UIElement.MouseEvent OnRightDoubleClick;
//		public event UIElement.MouseEvent OnMiddleMouseDown;
//		public event UIElement.MouseEvent OnMiddleMouseUp;
//		public event UIElement.MouseEvent OnMiddleClick;
//		public event UIElement.MouseEvent OnMiddleDoubleClick;
//		public event UIElement.MouseEvent OnXButton1MouseDown;
//		public event UIElement.MouseEvent OnXButton1MouseUp;
//		public event UIElement.MouseEvent OnXButton1Click;
//		public event UIElement.MouseEvent OnXButton1DoubleClick;
//		public event UIElement.MouseEvent OnXButton2MouseDown;
//		public event UIElement.MouseEvent OnXButton2MouseUp;
//		public event UIElement.MouseEvent OnXButton2Click;
//		public event UIElement.MouseEvent OnXButton2DoubleClick;
//		public event UIElement.ScrollWheelEvent OnScrollWheel;

//		public bool IsMouseHovering
//		{
//			get { return this._isMouseHovering; }
//		}

//		protected virtual void DrawSelf(SpriteBatch spriteBatch)
//		{
//		}

//		protected virtual void DrawChildren(SpriteBatch spriteBatch) => this.ForEach(element => element.Draw(spriteBatch));

//		public void Append(UIElement element)
//		{
//			element.Remove();
//			element.Parent = this;
//			this.Elements.Add(element);
//			element.Recalculate();
//		}

//		public void Remove()
//		{
//			if (Parent != null)
//			{
//				Parent.RemoveChild(this);
//			}
//		}

//		public void RemoveChild(UIElement child)
//		{
//			this.Elements.Remove(child);
//			child.Parent = null;
//		}

//		public void RemoveAllChildren()
//		{
//			foreach (UIElement current in this.Elements)
//			{
//				current.Parent = null;
//			}

//			this.Elements.Clear();
//		}

//		public bool HasChild(UIElement child)
//		{
//			return Elements.Contains(child);
//		}

//		public virtual void Draw(SpriteBatch spriteBatch)
//		{
//			bool overflowHidden = this.OverflowHidden;
//			bool useImmediateMode = this._useImmediateMode;
//			RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
//			Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
//			SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;
//			if (useImmediateMode)
//			{
//				spriteBatch.End();
//				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, UIElement._overflowHiddenRasterizerState, null, Main.UIScaleMatrix);
//				DrawSelf(spriteBatch);
//				spriteBatch.End();
//				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, UIElement._overflowHiddenRasterizerState, null, Main.UIScaleMatrix);
//			}
//			else
//			{
//				DrawSelf(spriteBatch);
//			}

//			if (overflowHidden)
//			{
//				spriteBatch.End();
//				Rectangle clippingRectangle = GetClippingRectangle(spriteBatch);
//				Rectangle adjustedClippingRectangle = Rectangle.Intersect(clippingRectangle, spriteBatch.GraphicsDevice.ScissorRectangle);
//				spriteBatch.GraphicsDevice.ScissorRectangle = adjustedClippingRectangle;
//				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, UIElement._overflowHiddenRasterizerState, null, Main.UIScaleMatrix);
//			}

//			DrawChildren(spriteBatch);
//			if (overflowHidden)
//			{
//				rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
//				spriteBatch.End();
//				spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
//				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);
//			}
//		}

//		public virtual void Update(GameTime gameTime)
//		{
//			foreach (UIElement current in this.Elements)
//			{
//				current.Update(gameTime);
//			}
//		}

//		public Rectangle GetClippingRectangle(SpriteBatch spriteBatch)
//		{
//			Vector2 vector = new Vector2(_innerDimensions.X, _innerDimensions.Y);
//			Vector2 position = new Vector2(_innerDimensions.Width, _innerDimensions.Height) + vector;
//			vector = Vector2.Transform(vector, Main.UIScaleMatrix);
//			position = Vector2.Transform(position, Main.UIScaleMatrix);
//			Rectangle result = new Rectangle((int)vector.X, (int)vector.Y, (int)(position.X - vector.X), (int)(position.Y - vector.Y));
//			int width = spriteBatch.GraphicsDevice.Viewport.Width;
//			int height = spriteBatch.GraphicsDevice.Viewport.Height;
//			result.X = Utils.Clamp(result.X, 0, width);
//			result.Y = Utils.Clamp(result.Y, 0, height);
//			result.Width = Utils.Clamp(result.Width, 0, width - result.X);
//			result.Height = Utils.Clamp(result.Height, 0, height - result.Y);
//			return result;
//		}

//		public virtual void Recalculate()
//		{
//			CalculatedStyle calculatedStyle;
//			if (Parent != null)
//			{
//				calculatedStyle = Parent.GetInnerDimensions();
//			}
//			else
//			{
//				calculatedStyle = UserInterface.ActiveInstance.GetDimensions();
//			}

//			if (Parent != null && Parent is UIList)
//			{
//				calculatedStyle.Height = 3.40282347E+38f;
//			}

//			CalculatedStyle calculatedStyle2;
//			calculatedStyle2.X = Left.GetValue(calculatedStyle.Width) + calculatedStyle.X;
//			calculatedStyle2.Y = Top.GetValue(calculatedStyle.Height) + calculatedStyle.Y;
//			float value = MinWidth.GetValue(calculatedStyle.Width);
//			float value2 = MaxWidth.GetValue(calculatedStyle.Width);
//			float value3 = MinHeight.GetValue(calculatedStyle.Height);
//			float value4 = MaxHeight.GetValue(calculatedStyle.Height);
//			calculatedStyle2.Width = MathHelper.Clamp(Width.GetValue(calculatedStyle.Width), value, value2);
//			calculatedStyle2.Height = MathHelper.Clamp(Height.GetValue(calculatedStyle.Height), value3, value4);
//			calculatedStyle2.Width += MarginLeft + MarginRight;
//			calculatedStyle2.Height += MarginTop + MarginBottom;
//			calculatedStyle2.X += calculatedStyle.Width * HAlign - calculatedStyle2.Width * HAlign;
//			calculatedStyle2.Y += calculatedStyle.Height * VAlign - calculatedStyle2.Height * VAlign;
//			_outerDimensions = calculatedStyle2;
//			calculatedStyle2.X += MarginLeft;
//			calculatedStyle2.Y += MarginTop;
//			calculatedStyle2.Width -= MarginLeft + MarginRight;
//			calculatedStyle2.Height -= MarginTop + MarginBottom;
//			_dimensions = calculatedStyle2;
//			calculatedStyle2.X += PaddingLeft;
//			calculatedStyle2.Y += PaddingTop;
//			calculatedStyle2.Width -= PaddingLeft + PaddingRight;
//			calculatedStyle2.Height -= PaddingTop + PaddingBottom;
//			_innerDimensions = calculatedStyle2;
//			RecalculateChildren();
//		}

//		public UIElement GetElementAt(Vector2 point)
//		{
//			UIElement uIElement = null;
//			foreach (UIElement current in this.Elements)
//			{
//				if (current.ContainsPoint(point))
//				{
//					uIElement = current;
//					break;
//				}
//			}

//			if (uIElement != null)
//			{
//				return uIElement.GetElementAt(point);
//			}

//			if (ContainsPoint(point))
//			{
//				return this;
//			}

//			return null;
//		}

//		public virtual bool ContainsPoint(Vector2 point)
//		{
//			return point.X > _dimensions.X && point.Y > _dimensions.Y && point.X < _dimensions.X + _dimensions.Width && point.Y < _dimensions.Y + _dimensions.Height;
//		}

//		public void SetPadding(float pixels)
//		{
//			PaddingBottom = pixels;
//			PaddingLeft = pixels;
//			PaddingRight = pixels;
//			PaddingTop = pixels;
//		}

//		public virtual void RecalculateChildren()
//		{
//			foreach (UIElement current in this.Elements)
//			{
//				current.Recalculate();
//			}
//		}

//		public CalculatedStyle GetInnerDimensions()
//		{
//			return _innerDimensions;
//		}

//		public CalculatedStyle GetDimensions()
//		{
//			return _dimensions;
//		}

//		public CalculatedStyle GetOuterDimensions()
//		{
//			return _outerDimensions;
//		}

//		public virtual void MouseDown(UIMouseEvent evt)
//		{
//			if (OnMouseDown != null)
//			{
//				OnMouseDown(evt, this);
//			}

//			if (Parent != null)
//			{
//				Parent.MouseDown(evt);
//			}
//		}

//		public virtual void MouseUp(UIMouseEvent evt)
//		{
//			if (OnMouseUp != null)
//			{
//				OnMouseUp(evt, this);
//			}

//			if (Parent != null)
//			{
//				Parent.MouseUp(evt);
//			}
//		}

//		public virtual void MouseOver(UIMouseEvent evt)
//		{
//			this._isMouseHovering = true;
//			if (OnMouseOver != null)
//			{
//				OnMouseOver(evt, this);
//			}

//			if (Parent != null)
//			{
//				Parent.MouseOver(evt);
//			}
//		}

//		public virtual void MouseOut(UIMouseEvent evt)
//		{
//			this._isMouseHovering = false;
//			if (OnMouseOut != null)
//			{
//				OnMouseOut(evt, this);
//			}

//			if (Parent != null)
//			{
//				Parent.MouseOut(evt);
//			}
//		}

//		public virtual void Click(UIMouseEvent evt)
//		{
//			if (OnClick != null)
//			{
//				OnClick(evt, this);
//			}

//			if (Parent != null)
//			{
//				Parent.Click(evt);
//			}
//		}

//		public virtual void DoubleClick(UIMouseEvent evt)
//		{
//			if (OnDoubleClick != null)
//			{
//				OnDoubleClick(evt, this);
//			}

//			if (Parent != null)
//			{
//				Parent.DoubleClick(evt);
//			}
//		}

//		public virtual void RightMouseDown(UIMouseEvent evt)
//		{
//			if (OnRightMouseDown != null)
//			{
//				OnRightMouseDown(evt, this);
//			}

//			if (Parent != null)
//			{
//				Parent.RightMouseDown(evt);
//			}
//		}

//		public virtual void RightMouseUp(UIMouseEvent evt)
//		{
//			if (OnRightMouseUp != null)
//			{
//				OnRightMouseUp(evt, this);
//			}

//			if (Parent != null)
//			{
//				Parent.RightMouseUp(evt);
//			}
//		}

//		public virtual void RightClick(UIMouseEvent evt)
//		{
//			if (OnRightClick != null)
//			{
//				OnRightClick(evt, this);
//			}

//			if (Parent != null)
//			{
//				Parent.RightClick(evt);
//			}
//		}

//		public virtual void RightDoubleClick(UIMouseEvent evt)
//		{
//			if (OnRightDoubleClick != null)
//			{
//				OnRightDoubleClick(evt, this);
//			}

//			if (Parent != null)
//			{
//				Parent.RightDoubleClick(evt);
//			}
//		}

//		public virtual void MiddleMouseDown(UIMouseEvent evt)
//		{
//			if (OnMiddleMouseDown != null)
//			{
//				OnMiddleMouseDown(evt, this);
//			}

//			if (Parent != null)
//			{
//				Parent.MiddleMouseDown(evt);
//			}
//		}

//		public virtual void MiddleMouseUp(UIMouseEvent evt)
//		{
//			if (OnMiddleMouseUp != null)
//			{
//				OnMiddleMouseUp(evt, this);
//			}

//			if (Parent != null)
//			{
//				Parent.MiddleMouseUp(evt);
//			}
//		}

//		public virtual void MiddleClick(UIMouseEvent evt)
//		{
//			if (OnMiddleClick != null)
//			{
//				OnMiddleClick(evt, this);
//			}

//			if (Parent != null)
//			{
//				Parent.MiddleClick(evt);
//			}
//		}

//		public virtual void MiddleDoubleClick(UIMouseEvent evt)
//		{
//			if (OnMiddleDoubleClick != null)
//			{
//				OnMiddleDoubleClick(evt, this);
//			}

//			if (Parent != null)
//			{
//				Parent.MiddleDoubleClick(evt);
//			}
//		}

//		public virtual void XButton1MouseDown(UIMouseEvent evt)
//		{
//			if (OnXButton1MouseDown != null)
//			{
//				OnXButton1MouseDown(evt, this);
//			}

//			if (Parent != null)
//			{
//				Parent.XButton1MouseDown(evt);
//			}
//		}

//		public virtual void XButton1MouseUp(UIMouseEvent evt)
//		{
//			if (OnXButton1MouseUp != null)
//			{
//				OnXButton1MouseUp(evt, this);
//			}

//			if (Parent != null)
//			{
//				Parent.XButton1MouseUp(evt);
//			}
//		}

//		public virtual void XButton1Click(UIMouseEvent evt)
//		{
//			if (OnXButton1Click != null)
//			{
//				OnXButton1Click(evt, this);
//			}

//			if (Parent != null)
//			{
//				Parent.XButton1Click(evt);
//			}
//		}

//		public virtual void XButton1DoubleClick(UIMouseEvent evt)
//		{
//			if (OnXButton1DoubleClick != null)
//			{
//				OnXButton1DoubleClick(evt, this);
//			}

//			if (Parent != null)
//			{
//				Parent.XButton1DoubleClick(evt);
//			}
//		}

//		public virtual void XButton2MouseDown(UIMouseEvent evt)
//		{
//			if (OnXButton2MouseDown != null)
//			{
//				OnXButton2MouseDown(evt, this);
//			}

//			if (Parent != null)
//			{
//				Parent.XButton2MouseDown(evt);
//			}
//		}

//		public virtual void XButton2MouseUp(UIMouseEvent evt)
//		{
//			if (OnXButton2MouseUp != null)
//			{
//				OnXButton2MouseUp(evt, this);
//			}

//			if (Parent != null)
//			{
//				Parent.XButton2MouseUp(evt);
//			}
//		}

//		public virtual void XButton2Click(UIMouseEvent evt)
//		{
//			if (OnXButton2Click != null)
//			{
//				OnXButton2Click(evt, this);
//			}

//			if (Parent != null)
//			{
//				Parent.XButton2Click(evt);
//			}
//		}

//		public virtual void XButton2DoubleClick(UIMouseEvent evt)
//		{
//			if (OnXButton2DoubleClick != null)
//			{
//				OnXButton2DoubleClick(evt, this);
//			}

//			if (Parent != null)
//			{
//				Parent.XButton2DoubleClick(evt);
//			}
//		}

//		public virtual void ScrollWheel(UIScrollWheelEvent evt)
//		{
//			if (OnScrollWheel != null)
//			{
//				OnScrollWheel(evt, this);
//			}

//			if (Parent != null)
//			{
//				Parent.ScrollWheel(evt);
//			}
//		}

//		public void Activate()
//		{
//			if (!this._isInitialized)
//			{
//				Initialize();
//			}

//			OnActivate();
//			foreach (UIElement current in this.Elements)
//			{
//				current.Activate();
//			}
//		}

//		public virtual void OnActivate()
//		{
//		}

//		public void Deactivate()
//		{
//			OnDeactivate();
//			foreach (UIElement current in this.Elements)
//			{
//				current.Deactivate();
//			}
//		}

//		public virtual void OnDeactivate()
//		{
//		}

//		public void Initialize()
//		{
//			OnInitialize();
//			this._isInitialized = true;
//		}

//		public virtual void OnInitialize()
//		{
//		}

//		public virtual int CompareTo(object obj)
//		{
//			return 0;
//		}

//		public IEnumerator<BaseElement> GetEnumerator()
//		{
//			throw new NotImplementedException();
//		}

//		IEnumerator IEnumerable.GetEnumerator()
//		{
//			return GetEnumerator();
//		}

//		public void Add(BaseElement item)
//		{
//			throw new NotImplementedException();
//		}

//		public void Clear()
//		{
//			throw new NotImplementedException();
//		}

//		public bool Contains(BaseElement item)
//		{
//			throw new NotImplementedException();
//		}

//		public void CopyTo(BaseElement[] array, int arrayIndex)
//		{
//			throw new NotImplementedException();
//		}

//		public bool Remove(BaseElement item)
//		{
//			throw new NotImplementedException();
//		}

//		public int Count => children.Count;
//		public bool IsReadOnly => false;

//		public int IndexOf(BaseElement item)
//		{
//			throw new NotImplementedException();
//		}

//		public void Insert(int index, BaseElement item)
//		{
//			children.Insert(index, item);
//		}

//		public void RemoveAt(int index)
//		{
//			children.RemoveAt(index);
//		}

//		public BaseElement this[int index]
//		{
//			get => children[index];
//			set => children[index] = value;
//		}
//	}
//}

