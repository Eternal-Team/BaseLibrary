﻿using BaseLibrary.Input.Keyboard;
using BaseLibrary.Input.Mouse;
using Microsoft.Xna.Framework;
using Terraria;

namespace BaseLibrary.Input
{
	public static class Input
	{
		private static LayerStack Layers => BaseLibrary.Layers;

		private static int lastScreenWidth;
		private static int lastScreenHeight;

		internal static void Load()
		{
			if (Main.dedServ) return;

			MouseEvents.Load();
			KeyboardEvents.Load();
			KeyboardEvents.RepeatDelay = 31;

			MouseEvents.MouseMoved += args =>
			{
				foreach (Layer layer in Layers)
				{
					layer.OnMouseMove(args);
				}
			};

			MouseEvents.MouseScroll += args =>
			{
				foreach (Layer layer in Layers)
				{
					layer.OnMouseScroll(args);
					if (args.Handled) break;
				}
			};

			MouseEvents.ButtonClicked += args =>
			{
				foreach (Layer layer in Layers)
				{
					layer.OnClick(args);
					if (args.Handled) break;
				}
			};

			MouseEvents.ButtonDoubleClicked += args =>
			{
				foreach (Layer layer in Layers)
				{
					layer.OnDoubleClick(args);
					if (args.Handled) break;
				}
			};

			MouseEvents.ButtonTripleClicked += args =>
			{
				foreach (Layer layer in Layers)
				{
					layer.OnTripleClick(args);
					if (args.Handled) break;
				}
			};

			MouseEvents.ButtonPressed += args =>
			{
				foreach (Layer layer in Layers)
				{
					layer.OnMouseDown(args);
					if (args.Handled) break;
				}
			};

			MouseEvents.ButtonReleased += args =>
			{
				foreach (Layer layer in Layers)
				{
					layer.OnMouseUp(args);
					if (args.Handled) break;
				}
			};

			KeyboardEvents.KeyPressed += args =>
			{
				foreach (Layer layer in Layers)
				{
					layer.OnKeyPressed(args);
					if (args.Handled) break;
				}
			};

			KeyboardEvents.KeyReleased += args =>
			{
				foreach (Layer layer in Layers)
				{
					layer.OnKeyReleased(args);
					if (args.Handled) break;
				}
			};

			KeyboardEvents.KeyTyped += args =>
			{
				foreach (Layer layer in Layers)
				{
					layer.OnKeyTyped(args);
					if (args.Handled) break;
				}
			};
		}

		internal static void Update(GameTime time)
		{
			if (Main.dedServ) return;

			if (lastScreenWidth != Main.screenWidth || lastScreenHeight != Main.screenHeight)
			{
				WindowResizedEventArgs args = new WindowResizedEventArgs
				{
					Size = new Vector2(Main.screenWidth, Main.screenHeight)
				};

				foreach (Layer layer in Layers) layer.OnWindowResize(args);

				lastScreenWidth = Main.screenWidth;
				lastScreenHeight = Main.screenHeight;
			}

			MouseEvents.Update(time);
			KeyboardEvents.Update(time);
		}
	}
}