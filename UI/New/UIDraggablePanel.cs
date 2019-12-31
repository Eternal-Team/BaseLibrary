using BaseLibrary.Input;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;

namespace BaseLibrary.UI.New
{
	public class UIDraggablePanel : UIPanel
	{
		private Vector2Int offset;
		private bool dragging;

		public override void MouseDown(MouseButtonEventArgs args)
		{
			offset = args.Position - Position;

			dragging = true;

			args.Handled = true;
		}

		public override void MouseUp(MouseButtonEventArgs args)
		{
			dragging = false;

			args.Handled = true;
		}

		public override void Update(GameTime gameTime)
		{
			if (IsMouseHovering)
			{
				Main.LocalPlayer.mouseInterface = true;
				Main.LocalPlayer.showItemIcon = false;
				Main.ItemIconCacheUpdate(0);
				Main.mouseText = false;
			}

			if (dragging)
			{
				X.Percent = 0;
				Y.Percent = 0;

				Rectangle parent = Parent?.InnerDimensions ?? UserInterface.ActiveInstance.GetDimensions().ToRectangle();

				X.Pixels = (Main.mouseX - offset.X - parent.X).Clamp(0, parent.Width - OuterDimensions.Width);
				Y.Pixels = (Main.mouseY - offset.Y - parent.Y).Clamp(0, parent.Height - OuterDimensions.Height);

				Recalculate();
			}
		}
	}
}