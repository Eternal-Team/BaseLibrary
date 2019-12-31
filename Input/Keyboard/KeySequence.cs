using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace BaseLibrary.Input.Keyboard
{
	public class KeySequence
	{
		private int awaitingKey;

		public List<Keys> Sequence { get; set; }

		public KeySequence(params Keys[] keys)
		{
			if (keys.Length == 0) throw new ArgumentException("Must supply at least one key to the KeySequence constructor.");

			Sequence = new List<Keys>(keys);
		}

		public void HandleKey(Keys key)
		{
			if (key == Sequence[awaitingKey])
			{
				awaitingKey++;
				if (awaitingKey >= Sequence.Count)
				{
					awaitingKey = 0;
					OnKeySequenceEntered(this, EventArgs.Empty);
				}
			}
		}

		public void OnKeySequenceEntered(object sender, EventArgs args)
		{
			KeySequenceEntered?.Invoke(sender, args);
		}

		public event EventHandler<EventArgs> KeySequenceEntered;
	}
}