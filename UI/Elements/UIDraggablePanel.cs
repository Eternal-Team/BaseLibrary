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
				Hooking.InUI = true;
				Main.LocalPlayer.mouseInterface = true;
				Main.LocalPlayer.showItemIcon = false;
				Main.ItemIconCacheUpdate(0);
				Main.mouseText = false;
			}

			if (dragging)
			{
				Left = ((Main.MouseScreen.X - offset.X).Clamp(0, Main.screenWidth - Dimensions.Width), 0);
				Top = ((Main.MouseScreen.Y - offset.Y).Clamp(0, Main.screenHeight - Dimensions.Height), 0);

				Recalculate();
			}
		}
	}
}