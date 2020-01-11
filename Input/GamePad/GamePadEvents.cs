using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace BaseLibrary.Input.GamePad
{
	public class GamePadEvents : GameComponent
	{
		public static float TriggerThreshold { get; set; }

		public static float ThumbstickThreshold { get; set; }

		private GamePadState previous;

		public PlayerIndex PhysicalIndex { get; set; }

		public PlayerIndex LogicalIndex { get; set; }

		static GamePadEvents()
		{
			TriggerThreshold = 0.5f;
			ThumbstickThreshold = 0.5f;
		}

		public GamePadEvents(PlayerIndex physicalIndex, Game game) : this(physicalIndex, physicalIndex, game)
		{
		}

		public GamePadEvents(PlayerIndex physicalIndex, PlayerIndex logicalIndex, Game game) : base(game)
		{
			PhysicalIndex = physicalIndex;
			LogicalIndex = logicalIndex;
		}

		public override void Update(GameTime gameTime)
		{
			if (!Game.IsActive) return;

			base.Update(gameTime);

			GamePadState current = Microsoft.Xna.Framework.Input.GamePad.GetState(PhysicalIndex);

			if (current.Buttons.A == ButtonState.Pressed && previous.Buttons.A == ButtonState.Released)
			{
				OnButtonDown(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.A, current));
			}

			if (current.Buttons.B == ButtonState.Pressed && previous.Buttons.B == ButtonState.Released)
			{
				OnButtonDown(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.B, current));
			}

			if (current.Buttons.X == ButtonState.Pressed && previous.Buttons.X == ButtonState.Released)
			{
				OnButtonDown(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.X, current));
			}

			if (current.Buttons.Y == ButtonState.Pressed && previous.Buttons.Y == ButtonState.Released)
			{
				OnButtonDown(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.Y, current));
			}

			if (current.Buttons.Back == ButtonState.Pressed && previous.Buttons.Back == ButtonState.Released)
			{
				OnButtonDown(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.Back, current));
			}

			if (current.Buttons.Start == ButtonState.Pressed && previous.Buttons.Start == ButtonState.Released)
			{
				OnButtonDown(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.Start, current));
			}

			if (current.Buttons.LeftShoulder == ButtonState.Pressed && previous.Buttons.LeftShoulder == ButtonState.Released)
			{
				OnButtonDown(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.LeftShoulder, current));
			}

			if (current.Buttons.RightShoulder == ButtonState.Pressed && previous.Buttons.RightShoulder == ButtonState.Released)
			{
				OnButtonDown(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.RightShoulder, current));
			}

			if (current.DPad.Left == ButtonState.Pressed && previous.DPad.Left == ButtonState.Released)
			{
				OnButtonDown(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.DPadLeft, current));
			}

			if (current.DPad.Right == ButtonState.Pressed && previous.DPad.Right == ButtonState.Released)
			{
				OnButtonDown(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.DPadRight, current));
			}

			if (current.DPad.Up == ButtonState.Pressed && previous.DPad.Up == ButtonState.Released)
			{
				OnButtonDown(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.DPadUp, current));
			}

			if (current.DPad.Down == ButtonState.Pressed && previous.DPad.Down == ButtonState.Released)
			{
				OnButtonDown(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.DPadDown, current));
			}

			if (current.Triggers.Left > TriggerThreshold && previous.Triggers.Left <= TriggerThreshold)
			{
				OnButtonDown(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.LeftTrigger, current));
			}

			if (current.Triggers.Right > TriggerThreshold && previous.Triggers.Right <= TriggerThreshold)
			{
				OnButtonDown(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.RightTrigger, current));
			}

			if (current.Buttons.LeftStick == ButtonState.Pressed && previous.Buttons.LeftStick == ButtonState.Released)
			{
				OnButtonDown(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.LeftThumbstick, current));
			}

			if (current.Buttons.RightStick == ButtonState.Pressed && previous.Buttons.RightStick == ButtonState.Released)
			{
				OnButtonDown(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.RightThumbstick, current));
			}

			PolarCoordinate oldLeft = PolarCoordinate.FromCartesian(previous.ThumbSticks.Left);
			PolarCoordinate newLeft = PolarCoordinate.FromCartesian(current.ThumbSticks.Left);
			PolarCoordinate newRight = PolarCoordinate.FromCartesian(current.ThumbSticks.Right);
			PolarCoordinate oldRight = PolarCoordinate.FromCartesian(previous.ThumbSticks.Right);

			if (InLeftSection(newLeft) && !InLeftSection(oldLeft))
			{
				OnButtonDown(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.LeftThumbstickLeft, current));
			}

			if (InRightSection(newLeft) && !InRightSection(oldLeft))
			{
				OnButtonDown(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.LeftThumbstickRight, current));
			}

			if (InUpSection(newLeft) && !InUpSection(oldLeft))
			{
				OnButtonDown(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.LeftThumbstickUp, current));
			}

			if (InDownSection(newLeft) && !InDownSection(oldLeft))
			{
				OnButtonDown(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.LeftThumbstickDown, current));
			}

			if (InLeftSection(newRight) && !InLeftSection(oldRight))
			{
				OnButtonDown(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.RightThumbstickLeft, current));
			}

			if (InRightSection(newRight) && !InRightSection(oldRight))
			{
				OnButtonDown(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.RightThumbstickRight, current));
			}

			if (InUpSection(newRight) && !InUpSection(oldRight))
			{
				OnButtonDown(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.RightThumbstickUp, current));
			}

			if (InDownSection(newRight) && !InDownSection(oldRight))
			{
				OnButtonDown(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.RightThumbstickDown, current));
			}

			if (current.Buttons.A == ButtonState.Released && previous.Buttons.A == ButtonState.Pressed)
			{
				OnButtonUp(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.A, current));
			}

			if (current.Buttons.B == ButtonState.Released && previous.Buttons.B == ButtonState.Pressed)
			{
				OnButtonUp(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.B, current));
			}

			if (current.Buttons.X == ButtonState.Released && previous.Buttons.X == ButtonState.Pressed)
			{
				OnButtonUp(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.X, current));
			}

			if (current.Buttons.Y == ButtonState.Released && previous.Buttons.Y == ButtonState.Pressed)
			{
				OnButtonUp(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.Y, current));
			}

			if (current.Buttons.Back == ButtonState.Released && previous.Buttons.Back == ButtonState.Pressed)
			{
				OnButtonUp(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.Back, current));
			}

			if (current.Buttons.Start == ButtonState.Released && previous.Buttons.Start == ButtonState.Pressed)
			{
				OnButtonUp(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.Start, current));
			}

			if (current.Buttons.LeftShoulder == ButtonState.Released && previous.Buttons.LeftShoulder == ButtonState.Pressed)
			{
				OnButtonUp(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.LeftShoulder, current));
			}

			if (current.Buttons.RightShoulder == ButtonState.Released && previous.Buttons.RightShoulder == ButtonState.Pressed)
			{
				OnButtonUp(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.RightShoulder, current));
			}

			if (current.DPad.Left == ButtonState.Released && previous.DPad.Left == ButtonState.Pressed)
			{
				OnButtonUp(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.DPadLeft, current));
			}

			if (current.DPad.Right == ButtonState.Released && previous.DPad.Right == ButtonState.Pressed)
			{
				OnButtonUp(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.DPadRight, current));
			}

			if (current.DPad.Up == ButtonState.Released && previous.DPad.Up == ButtonState.Pressed)
			{
				OnButtonUp(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.DPadUp, current));
			}

			if (current.DPad.Down == ButtonState.Released && previous.DPad.Down == ButtonState.Pressed)
			{
				OnButtonUp(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.DPadDown, current));
			}

			if (current.Triggers.Left <= TriggerThreshold && previous.Triggers.Left > TriggerThreshold)
			{
				OnButtonUp(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.LeftTrigger, current));
			}

			if (current.Triggers.Right <= TriggerThreshold && previous.Triggers.Right > TriggerThreshold)
			{
				OnButtonUp(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.RightTrigger, current));
			}

			if (current.Buttons.LeftStick == ButtonState.Released && previous.Buttons.LeftStick == ButtonState.Pressed)
			{
				OnButtonUp(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.LeftThumbstick, current));
			}

			if (current.Buttons.RightStick == ButtonState.Released && previous.Buttons.RightStick == ButtonState.Pressed)
			{
				OnButtonUp(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.RightThumbstick, current));
			}

			if (!InLeftSection(newLeft) && InLeftSection(oldLeft))
			{
				OnButtonUp(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.LeftThumbstickLeft, current));
			}

			if (!InRightSection(newLeft) && InRightSection(oldLeft))
			{
				OnButtonUp(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.LeftThumbstickRight, current));
			}

			if (!InUpSection(newLeft) && InUpSection(oldLeft))
			{
				OnButtonUp(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.LeftThumbstickUp, current));
			}

			if (!InDownSection(newLeft) && InDownSection(oldLeft))
			{
				OnButtonUp(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.LeftThumbstickDown, current));
			}

			if (!InLeftSection(newRight) && InLeftSection(oldRight))
			{
				OnButtonUp(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.RightThumbstickLeft, current));
			}

			if (!InRightSection(newRight) && InRightSection(oldRight))
			{
				OnButtonUp(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.RightThumbstickRight, current));
			}

			if (!InUpSection(newRight) && InUpSection(oldRight))
			{
				OnButtonUp(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.RightThumbstickUp, current));
			}

			if (!InDownSection(newRight) && InDownSection(oldRight))
			{
				OnButtonUp(this, new GamePadButtonEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadButtons.RightThumbstickDown, current));
			}

			if (current.Triggers.Left != previous.Triggers.Left)
			{
				OnTriggerMoved(this, new GamePadTriggerEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadTriggers.Left, current.Triggers.Left, current));
				OnLeftTriggerMoved(this, new GamePadTriggerEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadTriggers.Left, current.Triggers.Left, current));
			}

			if (current.Triggers.Right != previous.Triggers.Right)
			{
				OnTriggerMoved(this, new GamePadTriggerEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadTriggers.Right, current.Triggers.Right, current));
				OnRightTriggerMoved(this, new GamePadTriggerEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadTriggers.Right, current.Triggers.Right, current));
			}

			if (current.ThumbSticks.Left.X != previous.ThumbSticks.Left.X || current.ThumbSticks.Left.Y != previous.ThumbSticks.Left.Y)
			{
				OnThumbstickMoved(this, new GamePadThumbstickEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadThumbsticks.Left, new Vector2(current.ThumbSticks.Left.X, current.ThumbSticks.Left.Y), current));
				OnLeftThumbstickMoved(this, new GamePadThumbstickEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadThumbsticks.Left, new Vector2(current.ThumbSticks.Left.X, current.ThumbSticks.Left.Y), current));
			}

			if (current.ThumbSticks.Right.X != previous.ThumbSticks.Right.X || current.ThumbSticks.Right.Y != previous.ThumbSticks.Right.Y)
			{
				OnThumbstickMoved(this, new GamePadThumbstickEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadThumbsticks.Right, new Vector2(current.ThumbSticks.Right.X, current.ThumbSticks.Right.Y), current));
				OnRightThumbstickMoved(this, new GamePadThumbstickEventArgs(gameTime.TotalGameTime, LogicalIndex, GamePadThumbsticks.Right, new Vector2(current.ThumbSticks.Right.X, current.ThumbSticks.Right.Y), current));
			}

			if (current.IsConnected && !previous.IsConnected)
			{
				OnConnected(this, new GamePadEventArgs(gameTime.TotalGameTime, LogicalIndex, current));
			}

			if (!current.IsConnected && previous.IsConnected)
			{
				OnDisconnected(this, new GamePadEventArgs(gameTime.TotalGameTime, LogicalIndex, current));
			}

			previous = current;
		}

		private bool InLeftSection(PolarCoordinate value)
		{
			return value.Distance > ThumbstickThreshold && InLeftDirection(value.Angle);
		}

		private bool InRightSection(PolarCoordinate value)
		{
			return value.Distance > ThumbstickThreshold && InRightDirection(value.Angle);
		}

		private bool InUpSection(PolarCoordinate value)
		{
			return value.Distance > ThumbstickThreshold && InUpDirection(value.Angle);
		}

		private bool InDownSection(PolarCoordinate value)
		{
			return value.Distance > ThumbstickThreshold && InDownDirection(value.Angle);
		}

		private bool InRightDirection(float angle)
		{
			return angle <= MathHelper.PiOver4 && angle > -MathHelper.PiOver4;
		}

		private bool InLeftDirection(float angle)
		{
			return angle > MathHelper.Pi - MathHelper.PiOver4 || angle <= -MathHelper.Pi + MathHelper.PiOver4;
		}

		private bool InUpDirection(float angle)
		{
			return angle > MathHelper.PiOver2 - MathHelper.PiOver4 && angle <= MathHelper.PiOver2 + MathHelper.PiOver4;
		}

		private bool InDownDirection(float angle)
		{
			return angle < -MathHelper.PiOver2 + MathHelper.PiOver4 && angle >= -MathHelper.PiOver2 - MathHelper.PiOver4;
		}

		public void OnButtonDown(object sender, GamePadButtonEventArgs args)
		{
			ButtonDown?.Invoke(sender, args);
		}

		public void OnButtonUp(object sender, GamePadButtonEventArgs args)
		{
			ButtonUp?.Invoke(sender, args);
		}

		public void OnLeftTriggerMoved(object sender, GamePadTriggerEventArgs args)
		{
			LeftTriggerMoved?.Invoke(sender, args);
		}

		public void OnRightTriggerMoved(object sender, GamePadTriggerEventArgs args)
		{
			RightTriggerMoved?.Invoke(sender, args);
		}

		public void OnTriggerMoved(object sender, GamePadTriggerEventArgs args)
		{
			TriggerMoved?.Invoke(sender, args);
		}

		public void OnLeftThumbstickMoved(object sender, GamePadThumbstickEventArgs args)
		{
			LeftThumbstickMoved?.Invoke(sender, args);
		}

		public void OnRightThumbstickMoved(object sender, GamePadThumbstickEventArgs args)
		{
			RightThumbstickMoved?.Invoke(sender, args);
		}

		public void OnThumbstickMoved(object sender, GamePadThumbstickEventArgs args)
		{
			ThumbstickMoved?.Invoke(sender, args);
		}

		public void OnConnected(object sender, GamePadEventArgs args)
		{
			Connected?.Invoke(sender, args);
		}

		public void OnDisconnected(object sender, GamePadEventArgs args)
		{
			Disconnected?.Invoke(sender, args);
		}

		public static event EventHandler<GamePadButtonEventArgs> ButtonDown;

		public static event EventHandler<GamePadButtonEventArgs> ButtonUp;

		public static event EventHandler<GamePadTriggerEventArgs> LeftTriggerMoved;

		public static event EventHandler<GamePadTriggerEventArgs> RightTriggerMoved;

		public static event EventHandler<GamePadTriggerEventArgs> TriggerMoved;

		public static event EventHandler<GamePadThumbstickEventArgs> LeftThumbstickMoved;

		public static event EventHandler<GamePadThumbstickEventArgs> RightThumbstickMoved;

		public static event EventHandler<GamePadThumbstickEventArgs> ThumbstickMoved;

		public static event EventHandler<GamePadEventArgs> Connected;

		public static event EventHandler<GamePadEventArgs> Disconnected;
	}
}