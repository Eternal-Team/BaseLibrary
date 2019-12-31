using BaseLibrary.Input.Keyboard;
using BaseLibrary.Input.Mouse;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;

namespace BaseLibrary.Input
{
	public static class Input
	{
		public static LayerStack Layers;

		internal static void Load()
		{
			if (Main.dedServ) return;

			Layers = new LayerStack();
			Layers.PushLayer(new TerrariaLayer());
			Layers.PushOverlay(new UILayer());

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

			var sequence = new KeySequence(Keys.LeftControl, Keys.V);
			sequence.KeySequenceEntered += (sender, args) =>
			{
				Main.NewText("ctrl + v");
			};
			KeyboardEvents.AddKeySequence(sequence);
		}

		internal static void Update(GameTime time)
		{
			if (Main.dedServ) return;

			MouseEvents.Update(time);
			KeyboardEvents.Update(time);
		}
	}
}