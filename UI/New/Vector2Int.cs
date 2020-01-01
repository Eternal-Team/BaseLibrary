using Microsoft.Xna.Framework;

namespace BaseLibrary.UI.New
{
	public struct Vector2Int
	{
		public static readonly Vector2Int One = new Vector2Int(1, 1);
		public static readonly Vector2Int Zero = new Vector2Int(0, 0);
		public static readonly Vector2Int UnitX = new Vector2Int(1, 0);
		public static readonly Vector2Int UnitY = new Vector2Int(0, 1);

		public int X;
		public int Y;

		public Vector2Int(int value)
		{
			X = value;
			Y = value;
		}

		public Vector2Int(int x, int y)
		{
			X = x;
			Y = y;
		}

		public static Vector2Int operator -(Vector2Int a, Vector2Int b) => new Vector2Int(a.X - b.X, a.Y - b.Y);

		public static Vector2Int operator +(Vector2Int a, Vector2Int b) => new Vector2Int(a.X + b.X, a.Y + b.Y);

		public static implicit operator Vector2(Vector2Int vector) => new Vector2(vector.X, vector.Y);

		public static implicit operator Vector2Int(Vector2 vector) => new Vector2Int((int)vector.X, (int)vector.Y);
		
		public static Vector2Int Transform(Vector2Int position, Matrix matrix) => new Vector2Int((int)(position.X * matrix.M11 + position.Y * matrix.M21 + matrix.M41), (int)(position.X * matrix.M12 + position.Y * matrix.M22 + matrix.M42));

		public override string ToString() => $"X: {X} Y: {Y}";
	}
}