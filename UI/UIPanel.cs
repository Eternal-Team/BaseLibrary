using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader.Input;
using Terraria.ModLoader.Input.Mouse;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace BaseLibrary.UI
{
	public class UIPanel : BaseElement
	{
		public struct Settings
		{
			public static readonly Settings Default = new Settings
			{
				BackgroundColor = UICommon.DefaultUIBlue,
				BorderColor = Color.Black,
				Draggable = false,
				Resizeable = false
			};

			public Color BackgroundColor;
			public Color BorderColor;
			public bool Draggable;
			public bool Resizeable;
			// public Texture2D customTexture;
		}

		public Settings settings;

		public UIPanel(Settings? settings = null)
		{
			this.settings = settings ?? Settings.Default;

			Padding = new Padding(8);
		}

		#region Dragging
		private Vector2 offset;
		private bool dragging;

		protected override void MouseDown(MouseButtonEventArgs args)
		{
			if (!settings.Draggable || args.Button != MouseButton.Left) return;

			offset = args.Position - Position;

			dragging = true;

			args.Handled = true;
		}

		protected override void MouseUp(MouseButtonEventArgs args)
		{
			if (!settings.Draggable || args.Button != MouseButton.Left) return;

			dragging = false;

			args.Handled = true;
		}

		protected override void Update(GameTime gameTime)
		{
			if (!settings.Draggable) return;

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
		#endregion

		protected override void Draw(SpriteBatch spriteBatch)
		{
			// if (customTexture != null) spriteBatch.Draw(customTexture, Dimensions);
			// else
			DrawingUtility.DrawPanel(spriteBatch, Dimensions, settings.BackgroundColor, settings.BorderColor);

			if (IsMouseHovering)
			{
				Main.LocalPlayer.cursorItemIconEnabled = false;
				Main.ItemIconCacheUpdate(0);
			}
		}
	}
}