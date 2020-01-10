using BaseLibrary.Input;
using BaseLibrary.Input.Mouse;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace BaseLibrary.UI
{
	public class UIScrollbar : BaseElement
	{
		private float viewPosition;
		private float viewSize = 1f;
		private float maxViewSize = 20f;
		private bool isDragging;
		private bool isHoveringOverHandle;
		private float offset;

		private static Texture2D Texture { get; set; }
		private static Texture2D BarSliderTexture { get; set; }

		public event Action OnScroll;

		public float ViewPosition
		{
			get => viewPosition;
			set
			{
				viewPosition = MathHelper.Clamp(value, 0f, maxViewSize - viewSize);
				OnScroll?.Invoke();
			}
		}

		public UIScrollbar()
		{
			if (Texture == null) Texture = ModContent.GetTexture("Terraria/UI/Scrollbar");
			if (BarSliderTexture == null) BarSliderTexture = ModContent.GetTexture("BaseLibrary/Textures/UI/BarSlider");

			Width.Pixels = 20;
			Padding = new Padding(4, 2, 4, 2);
		}

		public void SetView(float viewSize, float maxViewSize)
		{
			viewSize = MathHelper.Clamp(viewSize, 0f, maxViewSize);
			viewPosition = MathHelper.Clamp(viewPosition, 0f, maxViewSize - viewSize);
			this.viewSize = viewSize;
			this.maxViewSize = maxViewSize;
		}

		private Rectangle HandleRectangle => new Rectangle(InnerDimensions.X, (int)(InnerDimensions.Y + InnerDimensions.Height * (viewPosition / maxViewSize)), InnerDimensions.Width, (int)(InnerDimensions.Height * (viewSize / maxViewSize)) - 6);

		protected override void MouseScroll(MouseScrollEventArgs args) => ViewPosition -= args.OffsetY;

		protected override void Draw(SpriteBatch spriteBatch)
		{
			if (isDragging)
			{
				float num = UserInterface.ActiveInstance.MousePosition.Y - InnerDimensions.Y - offset;
				ViewPosition = num / InnerDimensions.Height * maxViewSize;
			}

			Vector2 mousePosition = UserInterface.ActiveInstance.MousePosition;

			bool wasHovering = isHoveringOverHandle;
			isHoveringOverHandle = HandleRectangle.Contains(mousePosition);
			if (!wasHovering && isHoveringOverHandle && Main.hasFocus) Main.PlaySound(SoundID.MenuTick);

			{
				spriteBatch.Draw(Texture, Dimensions.Position(), new Rectangle(0, 0, 8, 8), Color.White);
				spriteBatch.Draw(Texture, Dimensions.TopRight() - new Vector2(8, 0), new Rectangle(12, 0, 8, 8), Color.White);
				spriteBatch.Draw(Texture, Dimensions.BottomLeft() - new Vector2(0, 8), new Rectangle(0, 8, 8, 8), Color.White);
				spriteBatch.Draw(Texture, Dimensions.BottomRight() - new Vector2(8, 8), new Rectangle(12, 8, 8, 8), Color.White);

				spriteBatch.Draw(Texture, new Rectangle(Dimensions.X + 8, Dimensions.Y, Dimensions.Width - 16, 8), new Rectangle(8, 0, 1, 8), Color.White);
				spriteBatch.Draw(Texture, new Rectangle(Dimensions.X + 8, Dimensions.Y + Dimensions.Height - 8, Dimensions.Width - 16, 8), new Rectangle(8, 8, 1, 8), Color.White);
				spriteBatch.Draw(Texture, new Rectangle(Dimensions.X, Dimensions.Y + 8, 8, Dimensions.Height - 16), new Rectangle(0, 8, 8, 1), Color.White);
				spriteBatch.Draw(Texture, new Rectangle(Dimensions.X + Dimensions.Width - 8, Dimensions.Y + 8, 8, Dimensions.Height - 16), new Rectangle(12, 8, 8, 1), Color.White);

				spriteBatch.Draw(Texture, new Rectangle(Dimensions.X + 8, Dimensions.Y + 8, Dimensions.Width - 16, Dimensions.Height - 16), new Rectangle(8, 8, 1, 1), Color.White);
			}

			{
				Color color = Color.White * (isDragging || isHoveringOverHandle ? 1f : 0.85f);
				spriteBatch.Draw(BarSliderTexture, HandleRectangle.TopLeft(), new Rectangle(0, 0, 6, 6), color);
				spriteBatch.Draw(BarSliderTexture, HandleRectangle.TopRight() - new Vector2(6, 0), new Rectangle(6, 0, 6, 6), color);
				spriteBatch.Draw(BarSliderTexture, HandleRectangle.BottomLeft(), new Rectangle(0, 10, 6, 6), color);
				spriteBatch.Draw(BarSliderTexture, HandleRectangle.BottomRight() - new Vector2(6, 0), new Rectangle(6, 10, 6, 6), color);

				spriteBatch.Draw(BarSliderTexture, new Rectangle(HandleRectangle.X + 6, HandleRectangle.Y, HandleRectangle.Width - 12, 6), new Rectangle(5, 0, 1, 6), color);
				spriteBatch.Draw(BarSliderTexture, new Rectangle(HandleRectangle.X + 6, HandleRectangle.Y + HandleRectangle.Height, HandleRectangle.Width - 12, 6), new Rectangle(5, 10, 1, 6), color);
				spriteBatch.Draw(BarSliderTexture, new Rectangle(HandleRectangle.X, HandleRectangle.Y + 6, 6, HandleRectangle.Height - 6), new Rectangle(0, 5, 6, 1), color);
				spriteBatch.Draw(BarSliderTexture, new Rectangle(HandleRectangle.X + HandleRectangle.Width - 6, HandleRectangle.Y + 6, 6, HandleRectangle.Height - 6), new Rectangle(6, 5, 6, 1), color);

				spriteBatch.Draw(BarSliderTexture, new Rectangle(HandleRectangle.X + 6, HandleRectangle.Y + 6, HandleRectangle.Width - 12, HandleRectangle.Height - 6), new Rectangle(6, 6, 1, 1), color);
			}
		}

		protected override void MouseDown(MouseButtonEventArgs args)
		{
			if (args.Button != MouseButton.Left) return;

			if (HandleRectangle.Contains(args.Position))
			{
				isDragging = true;
				offset = args.Position.Y - HandleRectangle.Y;

				args.Handled = true;
			}
		}

		protected override void MouseUp(MouseButtonEventArgs args)
		{
			if (args.Button != MouseButton.Left) return;

			args.Handled = true;

			isDragging = false;
		}
	}
}