using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;

namespace BaseLibrary.UI.Elements
{
	public class UIDraggablePanel : UIPanel
	{
		private Vector2 offset;
		private bool dragging;

		public override void MouseDown(UIMouseEvent evt)
		{
			base.MouseDown(evt);

			if (evt.Target != this) return;

			offset = evt.MousePosition - Position;
			HAlign = VAlign = 0f;

			dragging = true;
		}

		public override void MouseUp(UIMouseEvent evt)
		{
			base.MouseUp(evt);

			dragging = false;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (ContainsPoint(Main.MouseScreen))
			{
				BaseLibrary.InUI = true;
				Main.LocalPlayer.mouseInterface = true;
				Main.LocalPlayer.showItemIcon = false;
				Main.ItemIconCacheUpdate(0);
			}

			if (dragging)
			{
				CalculatedStyle dimensions = GetDimensions();

				Left = ((Main.MouseScreen.X - offset.X).Clamp(0, Main.screenWidth - dimensions.Width), 0);
				Top = ((Main.MouseScreen.Y - offset.Y).Clamp(0, Main.screenHeight - dimensions.Height), 0);

				Recalculate();
			}
		}
	}
}