using System;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;

namespace SideScroller
{
	public class PlayerControls : IDisposable
	{
		public PlayerControls(PlayerPlane planeToControl)
		{
			commands = new Command[9];
			this.planeToControl = planeToControl;
			SetUpCommands();
		}

		private readonly Command[] commands;
		private readonly PlayerPlane planeToControl;

		private void SetUpCommands()
		{
			commands[0] = new Command(() => planeToControl.AccelerateVertically(-Time.Delta));
			commands[1] = new Command(() => planeToControl.StopVertically());
			commands[2] = new Command(() => planeToControl.AccelerateVertically(Time.Delta));
			commands[3] = new Command(() => planeToControl.AccelerateHorizontally());
			commands[4] = new Command(() => planeToControl.DecelerateHorizontally());
			commands[5] = new Command(() => planeToControl.IsFireing = true);
			commands[6] = new Command(() => planeToControl.IsFireing = false);
			commands[7] = new Command(planeToControl.FireMissileIfAllowed);
			commands[8] = new Command(ControlMovementAnalogue);
			AddAscensionControls();
			AddSinkingControls();
			AddFireingControls();
			AddAccelerationControls();
			AddSlowingDownControls();
			commands[8].Add(new GamePadAnalogTrigger(GamePadAnalog.LeftThumbStick));
		}

		private void AddAscensionControls()
		{
			commands[0].Add(new KeyTrigger(Key.W, State.Pressed));
			commands[0].Add(new KeyTrigger(Key.W));
			commands[0].Add(new KeyTrigger(Key.CursorUp, State.Pressed));
			commands[0].Add(new KeyTrigger(Key.CursorUp));
			commands[1].Add(new KeyTrigger(Key.W, State.Releasing));
			commands[1].Add(new KeyTrigger(Key.CursorUp, State.Releasing));
		}

		private void AddSinkingControls()
		{
			commands[2].Add(new KeyTrigger(Key.S, State.Pressed));
			commands[2].Add(new KeyTrigger(Key.S));
			commands[2].Add(new KeyTrigger(Key.CursorDown, State.Pressed));
			commands[2].Add(new KeyTrigger(Key.CursorDown));
			commands[1].Add(new KeyTrigger(Key.S, State.Releasing));
			commands[1].Add(new KeyTrigger(Key.CursorDown, State.Releasing));
		}

		private void AddAccelerationControls()
		{
			commands[3].Add(new KeyTrigger(Key.D, State.Pressed));
			commands[3].Add(new KeyTrigger(Key.D));
			commands[3].Add(new KeyTrigger(Key.CursorRight, State.Pressed));
			commands[3].Add(new KeyTrigger(Key.CursorRight));
		}

		private void AddSlowingDownControls()
		{
			commands[4].Add(new KeyTrigger(Key.A, State.Pressed));
			commands[4].Add(new KeyTrigger(Key.A));
			commands[4].Add(new KeyTrigger(Key.CursorLeft, State.Pressed));
			commands[4].Add(new KeyTrigger(Key.CursorLeft));
		}

		private void AddFireingControls()
		{
			commands[5].Add(new KeyTrigger(Key.Space));
			commands[5].Add(new GamePadButtonTrigger(GamePadButton.RightShoulder));
			commands[6].Add(new KeyTrigger(Key.Space, State.Releasing));
			commands[6].Add(new GamePadButtonTrigger(GamePadButton.RightShoulder, State.Releasing));
			commands[7].Add(new KeyTrigger(Key.LeftControl));
			commands[7].Add(new GamePadButtonTrigger(GamePadButton.LeftShoulder));
		}

		private void ControlMovementAnalogue(Vector2D direction)
		{
			if (direction.Y > 0.1f)
				planeToControl.AccelerateVertically(-Time.Delta);
			else if(direction.Y < -0.1f)
				planeToControl.AccelerateVertically(Time.Delta);
			else
				planeToControl.StopVertically();
		}

		public void Dispose()
		{
			foreach (var command in commands)
				command.Dispose();
		}
	}
}