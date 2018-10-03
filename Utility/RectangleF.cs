using Microsoft.Xna.Framework;

namespace BaseLibrary.Utility
{
	public struct RectangleF
	{
		public float X;
		public float Y;
		public float Width;
		public float Height;

		public Rectangle rectangle;

		public RectangleF(float x, float y, float width, float height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
			rectangle = new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
		}

		public static RectangleF operator +(RectangleF rect1, RectangleF rect2) => new RectangleF(rect1.X + rect2.X, rect1.Y + rect2.Y, rect1.Width + rect2.Width, rect1.Height + rect2.Height);

		public static RectangleF operator -(RectangleF rect1, RectangleF rect2) => new RectangleF(rect1.X - rect2.X, rect1.Y - rect2.Y, rect1.Width - rect2.Width, rect1.Height - rect2.Height);
	}
}