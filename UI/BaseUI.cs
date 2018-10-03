using BaseLibrary.UI.Elements;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace TheOneLibrary.Base.UI
{
	public abstract class BaseUI : UIState
	{
		public UIPanel panelMain = new UIPanel();

		public override void OnInitialize()
		{
			panelMain.OnMouseDown += DragStart;
			panelMain.OnMouseUp += DragEnd;
			panelMain.SetPadding(0f);
			Initialize();
			Append(panelMain);
		}

		public new virtual void Initialize()
		{
		}

		#region Dragging
		public Vector2 offset;
		public bool dragging;

		public virtual void DragStart(UIMouseEvent evt, UIElement listeningElement)
		{
			if (evt.Target != panelMain) return;

			CalculatedStyle dimensions = panelMain.GetDimensions();
			offset = evt.MousePosition - dimensions.Position();
			panelMain.HAlign = panelMain.VAlign = 0f;

			dragging = true;
		}

		public virtual void DragEnd(UIMouseEvent evt, UIElement listeningElement) => dragging = false;

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (panelMain.ContainsPoint(Main.MouseScreen))
			{
				BaseLibrary.BaseLibrary.InUI = true;
				Main.LocalPlayer.mouseInterface = true;
				Main.LocalPlayer.showItemIcon = false;
				Main.ItemIconCacheUpdate(0);
			}
			else BaseLibrary.BaseLibrary.InUI = false;

			if (dragging)
			{
				CalculatedStyle dimensions = panelMain.GetDimensions();

				panelMain.Left.Pixels = (Main.MouseScreen.X - offset.X).Clamp(0, Main.screenWidth - dimensions.Width);
				panelMain.Top.Pixels = (Main.MouseScreen.Y - offset.Y).Clamp(0, Main.screenHeight - dimensions.Height);

				Recalculate();
			}
		}
		#endregion
	}
}