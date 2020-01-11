using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace BaseLibrary.Input.GamePad
{
	public class GamePadTriggerEventArgs : GamePadEventArgs
	{
		public GamePadTriggers Trigger { get; set; }

		public float Value { get; set; }

		public GamePadTriggerEventArgs(TimeSpan gameTime, PlayerIndex logicalIndex, GamePadTriggers trigger, float value, GamePadState current) : base(gameTime, logicalIndex, current)
		{
			Trigger = trigger;
			Value = value;
		}
	}
}