using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace BaseLibrary.Input.GamePad
{
	public class GamePadEventArgs : InputEventArgs
	{
		public PlayerIndex LogicalIndex { get; set; }

		public GamePadState Current { get; set; }

		public GamePadEventArgs(TimeSpan time, PlayerIndex logicalIndex, GamePadState current)
		{
			LogicalIndex = logicalIndex;
			Current = current;
		}
	}
}