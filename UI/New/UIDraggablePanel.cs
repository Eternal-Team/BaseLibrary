using BaseLibrary.Input;
using BaseLibrary.Input.Mouse;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;

namespace BaseLibrary.UI.New
{
	public class UIDraggablePanel : UIPanel
	{
		private Vector2 offset;
		private bool dragging;

		protected override void MouseDown(MouseButtonEventArgs args)
		{
			if (args.Button != MouseButton.Left) return;

			offset = args.Position - Position;

			dragging = true;

			args.Handled = true;
		}

		protected override void MouseUp(MouseButtonEventArgs args)
		{
			if (args.Button != MouseButton.Left) return;

			dragging = false;

			args.Handled = true;
		}

		protected override void Update(GameTime gameTime)
		{
			if (IsMouseHovering)
			{
				Main.LocalPlayer.mouseInterface = true;
				Main.LocalPlayer.showItemIcon = false;
				Main.ItemIconCacheUpdate(0);
				Main.mouseText = false;
				Main.HoverItem = new Item();
			}

			if (dragging)
			{
				X.Percent = 0;
				Y.Percent = 0;

				Rectangle parent = Parent?.InnerDimensions ?? UserInterface.ActiveInstance.GetDimensions().ToRectangle();

				X.Pixels = (int)(Main.mouseX - offset.X - parent.X).Clamp(0, parent.Width - OuterDimensions.Width);
				Y.Pixels = (int)(Main.mouseY - offset.Y - parent.Y).Clamp(0, parent.Height - OuterDimensions.Height);

				Recalculate();
			}
		}
	}
}