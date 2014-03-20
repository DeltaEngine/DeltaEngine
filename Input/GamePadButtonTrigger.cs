using System;
using DeltaEngine.Commands;
using DeltaEngine.Extensions;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Trigger implementation for GamePad Button events.
	/// </summary>
	public class GamePadButtonTrigger : InputTrigger
	{
		public GamePadButtonTrigger(GamePadButton button, State state = State.Pressing)
		{
			Button = button;
			State = state;
		}

		public GamePadButton Button { get; internal set; }
		public State State { get; internal set; }

		public GamePadButtonTrigger(string buttonAndState)
		{
			var parameters = buttonAndState.SplitAndTrim(new[] { ' ' });
			if (parameters.Length == 0)
				throw new CannotCreateGamePadTriggerWithoutKey();
			Button = parameters[0].Convert<GamePadButton>();
			State = parameters.Length > 1 ? parameters[1].Convert<State>() : State.Pressing;
			Start<GamePad>();
		}

		public class CannotCreateGamePadTriggerWithoutKey : Exception {}

		protected override void StartInputDevice()
		{
			Start<GamePad>();
		}
	}
}