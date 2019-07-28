using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;

namespace BaseLibrary
{
	/// <summary>
	///     Allows the execution of tasks on the UI thread
	/// </summary>
	public static class Dispatcher
	{
		private static Queue<Action> queue = new Queue<Action>();

		public static void Load()
		{
			if (Main.dedServ) return;

			Main.OnPreDraw += ProcessQueue;
		}

		public static void Unload()
		{
			if (Main.dedServ) return;

			Main.OnPreDraw -= ProcessQueue;
		}

		private static void ProcessQueue(GameTime gameTime)
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
			if (Main.dedServ) return;

			lock (queue) queue.Enqueue(action);
		}
	}
}