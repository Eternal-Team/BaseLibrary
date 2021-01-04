using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader.Input;
using Terraria.ModLoader.Input.Mouse;
using Terraria.UI;

namespace BaseLibrary.UI
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
				Main.LocalPlayer.cursorItemIconEnabled = false;
				Main.ItemIconCacheUpdate(0);
				Main.mouseText = false;
				Main.HoverItem = new Item();
			}

			if (dragging)
			{
				X.Percent = 0;
				Y.Percent = 0;

				Rectangle parent = Parent?.InnerDimensions ?? UserInterface.ActiveInstance.GetDimensions().ToRectangle();

				X.Pixels = Utils.Clamp((int)(Main.mouseX - offset.X - parent.X), 0, parent.Width - OuterDimensions.Width);
				Y.Pixels = Utils.Clamp((int)(Main.mouseY - offset.Y - parent.Y), 0, parent.Height - OuterDimensions.Height);

				Recalculate();
			}
		}
	}
}