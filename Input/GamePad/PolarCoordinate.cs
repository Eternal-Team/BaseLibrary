using Microsoft.Xna.Framework;
using System;

namespace BaseLibrary.Input.GamePad
{
	public class PolarCoordinate
	{
		public float Angle { get; }

		public float Distance { get; }

		public PolarCoordinate(float angle, float distance)
		{
			Angle = angle;
			Distance = distance;
		}

		public static PolarCoordinate FromCartesian(Vector2 cartesian)
		{
			float angle = (float)Math.Atan2(cartesian.Y, cartesian.X);
			float distance = cartesian.Length();

			return new PolarCoordinate(angle, distance);
		}
	}
}