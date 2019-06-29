using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace BaseLibrary.UI.Elements
{
	public class UIScrollbar : BaseElement
	{
		private float viewPosition;
		private float viewSize = 1f;
		private float maxViewSize = 20f;
		private bool isDragging;
		private bool isHoveringOverHandle;
		private float dragYOffset;

		//[PathOverride("Terraria/UI/Scrollbar")]
		//public static Texture2D Texture { get; set; }

		//[PathOverride("Terraria/UI/ScrollbarInner")]
		//public static Texture2D InnerTexture { get; set; }

		public float ViewPosition
		{
			get => viewPosition;
			set => viewPosition = MathHelper.Clamp(value, 0f, maxViewSize - viewSize);
		}

		public UIScrollbar()
		{
			Width = (20, 0);
			MaxWidth.Set(20f, 0f);
			Padding = (5, 0, 0, 5);
		}

		public void SetView(float viewSize, float maxViewSize)
		{
			viewSize = MathHelper.Clamp(viewSize, 0f, maxViewSize);
			viewPosition = MathHelper.Clamp(viewPosition, 0f, maxViewSize - viewSize);
			this.viewSize = viewSize;
			this.maxViewSize = maxViewSize;
		}

		public (float size, float maxViewSize) View
		{
			set
			{
				//value.size = MathHelper.Clamp(value.size, 0f, value.maxViewSize);
				//viewPosition = MathHelper.Clamp(viewPosition, 0f, value.maxViewSize - value.size);
				//viewSize = value.size;
				//maxViewSize = value.maxViewSize;
			}
		}

		public float GetValue() => viewPosition;

		private Rectangle GetHandleRectangle()
		{
			if (maxViewSize == 0f && viewSize == 0f)
			{
				viewSize = 1f;
				maxViewSize = 1f;
			}

			return new Rectangle((int)InnerDimensions.X, (int)(InnerDimensions.Y + InnerDimensions.Height * (viewPosition / maxViewSize)) - 3, 20, (int)(InnerDimensions.Height * (viewSize / maxViewSize)) + 7);
		}

		private void DrawBar(SpriteBatch spriteBatch, Texture2D texture, Rectangle dimensions, Color color)
		{
			spriteBatch.Draw(texture, new Rectangle(dimensions.X, dimensions.Y - 6, dimensions.Width, 6), new Rectangle(0, 0, texture.Width, 6), color);
			spriteBatch.Draw(texture, new Rectangle(dimensions.X, dimensions.Y, dimensions.Width, dimensions.Height), new Rectangle(0, 6, texture.Width, 4), color);
			spriteBatch.Draw(texture, new Rectangle(dimensions.X, dimensions.Y + dimensions.Height, dimensions.Width, 6), new Rectangle(0, texture.Height - 6, texture.Width, 6), color);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (isDragging)
			{
				float num = UserInterface.ActiveInstance.MousePosition.Y - InnerDimensions.Y - dragYOffset;
				viewPosition = MathHelper.Clamp(num / InnerDimensions.Height * maxViewSize, 0f, maxViewSize - viewSize);
			}

			Rectangle handleRectangle = GetHandleRectangle();
			Vector2 mousePosition = UserInterface.ActiveInstance.MousePosition;
			bool isHoveringOverHandle = this.isHoveringOverHandle;
			this.isHoveringOverHandle = handleRectangle.Contains(new Point((int)mousePosition.X, (int)mousePosition.Y));
			if (!isHoveringOverHandle && this.isHoveringOverHandle && Main.hasFocus) Main.PlaySound(SoundID.MenuTick);
			//DrawBar(spriteBatch, Texture, dimensions.ToRectangle(), Color.White);
			//DrawBar(spriteBatch, InnerTexture, handleRectangle, Color.White * (isDragging || this.isHoveringOverHandle ? 1f : 0.85f));
		}

		public override void MouseDown(UIMouseEvent evt)
		{
			base.MouseDown(evt);
			if (evt.Target == this)
			{
				Rectangle handleRectangle = GetHandleRectangle();
				if (handleRectangle.Contains(new Point((int)evt.MousePosition.X, (int)evt.MousePosition.Y)))
				{
					isDragging = true;
					dragYOffset = evt.MousePosition.Y - handleRectangle.Y;
					return;
				}

				float num = UserInterface.ActiveInstance.MousePosition.Y - InnerDimensions.Y - (handleRectangle.Height >> 1);
				viewPosition = MathHelper.Clamp(num / InnerDimensions.Height * maxViewSize, 0f, maxViewSize - viewSize);
			}
		}

		public override void MouseUp(UIMouseEvent evt)
		{
			base.MouseUp(evt);
			isDragging = false;
		}
	}
}