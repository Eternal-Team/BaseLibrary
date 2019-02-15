using System;
using System.Collections.Generic;
using Terraria;

namespace BaseLibrary.InputInterceptor
{
	public static class Scheduler
	{
		private static Queue<Action> queue = new Queue<Action>();

		public static void Load()
		{
			Main.OnPostDraw += gameTime => ProcessQueue();
		}

		public static void Unload()
		{
			Main.OnPostDraw += gameTime => ProcessQueue();
		}

		private static void ProcessQueue()
		{
			lock (queue)
			{
				while (queue.Count > 0)
				{
					queue.Dequeue()();
				}
			}
		}

		public static void EnqueueMessage(Action action)
		{
			lock (queue) queue.Enqueue(action);
		}
	}
}