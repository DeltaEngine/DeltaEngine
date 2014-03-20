using System;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Trigger implementation for Thumb Sticks and Triggers on controllers.
	/// </summary>
	public class GamePadAnalogTrigger : InputTrigger, PositionTrigger
	{
		public GamePadAnalogTrigger(GamePadAnalog gamePadAnalog)
		{
			Analog = gamePadAnalog;
		}

		public GamePadAnalog Analog { get; private set; }
		public Vector2D Position { get; set; }

		public GamePadAnalogTrigger(string gamePadAnalog)
		{
			if (String.IsNullOrEmpty(gamePadAnalog))
				throw new CannotCreateGamePadStickTriggerWithoutGamePadStick();
			Analog = gamePadAnalog.Convert<GamePadAnalog>();
		}

		public class CannotCreateGamePadStickTriggerWithoutGamePadStick : Exception {}

		protected override void StartInputDevice()
		{
			Start<GamePad>();
		}
	}
}