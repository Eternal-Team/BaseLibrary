using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace BaseLibrary.Input.GamePad
{
	public class GamePadThumbstickEventArgs : GamePadEventArgs
	{
		public GamePadThumbsticks Thumbstick { get; set; }

		public Vector2 Position { get; set; }

		public float Angle { get; set; }

		public float Amount { get; set; }

		public GamePadThumbstickEventArgs(TimeSpan gameTime, PlayerIndex logicalIndex, GamePadThumbsticks thumbstick, Vector2 position, GamePadState current) : base(gameTime, logicalIndex, current)
		{
			Thumbstick = thumbstick;
			Position = position;
			PolarCoordinate polar = PolarCoordinate.FromCartesian(position);
			Angle = polar.Angle;
			Amount = polar.Distance;
		}
	}
}