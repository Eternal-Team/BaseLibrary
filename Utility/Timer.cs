using System;

namespace BaseLibrary
{
	public struct Timer
	{
		private readonly int _interval;
		private  int _timer;
		private readonly Action _callback;

		public Timer(int interval, Action callback)
		{
			_interval = interval;
			_callback = callback;
			_timer = 0;
		}

		public void Update()
		{
			if (_timer < _interval)
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