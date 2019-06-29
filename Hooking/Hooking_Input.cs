using BaseLibrary.UI.Elements;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Starbound.Input;
using System;
using Terraria;
using Terraria.GameInput;
using Terraria.UI;
using PlayerInput = On.Terraria.GameInput.PlayerInput;

namespace BaseLibrary
{
	public static partial class Hooking
	{
		private static UIElement UIElement => Main.hasFocus ? UserInterface.ActiveInstance.CurrentState?.GetElementAt(UserInterface.ActiveInstance.MousePosition) : null;

		private static UIElement LastElementHover
		{
			get => UserInterface.ActiveInstance.GetValue<UIElement>("_lastElementHover");
			set => UserInterface.ActiveInstance.SetValue("_lastElementHover", value);
		}

		private static GameTime gameTime;

		public static void Initialize()
		{
			IL.Terraria.Main.DoUpdate += il =>
			{
				ILCursor cursor = new ILCursor(il);

				if (cursor.TryGotoNext(i => i.MatchCall(typeof(Main).GetMethod("DoUpdate_HandleInput", Utility.defaultFlags))))
				{
					cursor.Emit(OpCodes.Ldarg, 1);
					cursor.EmitDelegate<Action<GameTime>>(time => { gameTime = time; });
				}
			};

			On.Terraria.UI.UserInterface.Update += (orig, self, time) => { UserInterface.ActiveInstance.CurrentState?.Update(time); };

			PlayerInput.UpdateInput += orig =>
			{
				Utility.Input.Update(gameTime);

				orig();
			};

			PlayerInput.MouseInput += orig => { };

			PlayerInput.KeyboardInput += orig =>
			{
				if (!Utility.Input.InterceptKeyboard()) orig();
			};

			MouseEvents.ButtonClicked += (sender, args) =>
			{
				if (UIElement == null) return;

				switch (args.Button)
				{
					case MouseButton.Left:
						UIElement.Click(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.Right:
						UIElement.RightClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.Middle:
						UIElement.MiddleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.XButton1:
						UIElement.XButton1Click(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.XButton2:
						UIElement.XButton2Click(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
				}
			};

			MouseEvents.ButtonDoubleClicked += (sender, args) =>
			{
				if (UIElement == null) return;

				switch (args.Button)
				{
					case MouseButton.Left:
						UIElement.DoubleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.Right:
						UIElement.RightDoubleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.Middle:
						UIElement.MiddleDoubleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.XButton1:
						UIElement.XButton1DoubleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.XButton2:
						UIElement.XButton2DoubleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
				}
			};

			MouseEvents.ButtonTripleClicked += (sender, args) =>
			{
				if (UIElement is BaseElement cast)
				{
					switch (args.Button)
					{
						case MouseButton.Left:
							cast.TripleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
							break;
						case MouseButton.Right:
							cast.RightTripleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
							break;
						case MouseButton.Middle:
							cast.MiddleTripleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
							break;
						case MouseButton.XButton1:
							cast.XButton1TripleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
							break;
						case MouseButton.XButton2:
							cast.XButton2TripleClick(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
							break;
					}
				}
			};

			MouseEvents.ButtonPressed += (sender, args) =>
			{
				if (UIElement == null) return;

				switch (args.Button)
				{
					case MouseButton.Left:
						UIElement.MouseDown(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.Right:
						UIElement.RightMouseDown(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.Middle:
						UIElement.MiddleMouseDown(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.XButton1:
						UIElement.XButton1MouseDown(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.XButton2:
						UIElement.XButton2MouseDown(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
				}
			};

			MouseEvents.ButtonReleased += (sender, args) =>
			{
				if (UIElement == null) return;

				switch (args.Button)
				{
					case MouseButton.Left:
						UIElement?.MouseUp(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.Right:
						UIElement.RightMouseUp(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.Middle:
						UIElement.MiddleMouseUp(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.XButton1:
						UIElement.XButton1MouseUp(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
					case MouseButton.XButton2:
						UIElement.XButton2MouseUp(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));
						break;
				}
			};

			MouseEvents.MouseWheelMoved += (sender, args) =>
			{
				UIElement?.ScrollWheel(new UIScrollWheelEvent(UIElement, UserInterface.ActiveInstance.MousePosition, args.Delta));

				Terraria.GameInput.PlayerInput.ScrollWheelDelta = 0;
				Terraria.GameInput.PlayerInput.ScrollWheelValue -= args.Delta;

				Terraria.GameInput.PlayerInput.CurrentInputMode = InputMode.Mouse;
				Terraria.GameInput.PlayerInput.Triggers.Current.UsedMovementKey = false;
			};

			MouseEvents.MouseMoved += (sender, args) =>
			{
				UserInterface.ActiveInstance.MousePosition = new Vector2(args.Current.X, args.Current.Y);

				if (UIElement != LastElementHover)
				{
					LastElementHover?.MouseOut(new UIMouseEvent(LastElementHover, UserInterface.ActiveInstance.MousePosition));

					UIElement?.MouseOver(new UIMouseEvent(UIElement, UserInterface.ActiveInstance.MousePosition));

					LastElementHover = UIElement;
				}
			};

			MouseEvents.MouseMoved += (sender, args) =>
			{
				Terraria.GameInput.PlayerInput.MouseX = args.Current.X;
				Terraria.GameInput.PlayerInput.MouseY = args.Current.Y;

				Terraria.GameInput.PlayerInput.CurrentInputMode = InputMode.Mouse;
				Terraria.GameInput.PlayerInput.Triggers.Current.UsedMovementKey = false;
			};

			MouseEvents.ButtonPressed += (sender, args) =>
			{
				Terraria.GameInput.PlayerInput.MouseKeys.Clear();

				if (Main.instance.IsActive)
				{
					switch (args.Button)
					{
						case MouseButton.Left:
							Terraria.GameInput.PlayerInput.MouseKeys.Add("Mouse1");
							Main.mouseLeft = true;
							break;
						case MouseButton.Right:
							Terraria.GameInput.PlayerInput.MouseKeys.Add("Mouse2");
							Main.mouseRight = true;
							break;
						case MouseButton.Middle:
							Terraria.GameInput.PlayerInput.MouseKeys.Add("Mouse3");
							Main.mouseMiddle = true;
							break;
						case MouseButton.XButton1:
							Terraria.GameInput.PlayerInput.MouseKeys.Add("Mouse4");
							Main.mouseXButton1 = true;
							break;
						case MouseButton.XButton2:
							Terraria.GameInput.PlayerInput.MouseKeys.Add("Mouse5");
							Main.mouseXButton2 = true;
							break;
					}

					Terraria.GameInput.PlayerInput.CurrentInputMode = InputMode.Mouse;
					Terraria.GameInput.PlayerInput.Triggers.Current.UsedMovementKey = false;
				}
			};

			MouseEvents.ButtonReleased += (sender, args) => Terraria.GameInput.PlayerInput.MouseKeys.Clear();

			MouseEvents.MouseDragged += (sender, args) =>
			{
				if (UIElement is BaseElement cast) cast.MouseDragged(args);
			};
		}
	}
}