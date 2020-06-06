using System;

namespace BaseLibrary
{
	public struct Timer
	{
		private int _interval;
		private int _timer;
		private readonly Action _callback;

		public Timer(int interval, Action callback)
		{
			_interval = interval;
			_callback = callback;
			_timer = 0;
		}

		public int Interval
		{
			get => _interval;
			set
			{
				if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value), "Interval must be a number bigger than zero");

				_interval = value;
				_timer = 0;
			}
		}

		public void Update()
		{
			if (_timer < Interval)
			{
				_timer++;
			}
			else
			{
				_timer = 0;
				_callback();
			}
		}
	}
}