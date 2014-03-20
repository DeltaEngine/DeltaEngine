using System;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Trigger implementation for Keyboard events.
	/// </summary>
	public class KeyTrigger : InputTrigger, PositionTrigger, ZoomTrigger
	{
		public KeyTrigger(Key key, State state = State.Pressing)
		{
			Key = key;
			State = state;
		}

		public Key Key { get; internal set; }
		public State State { get; internal set; }
		public Vector2D Position { get; set; }
		public float ZoomAmount { get; set; }

		public KeyTrigger(string keyAndState)
		{
			var parameters = keyAndState.SplitAndTrim(new[] { ' ' });
			if (parameters.Length == 0)
				throw new CannotCreateKeyTriggerWithoutKey();
			Key = parameters[0].Convert<Key>();
			State = parameters.Length > 1 ? parameters[1].Convert<State>() : State.Pressing;
			Position = Key == Key.CursorLeft || Key == Key.A ? Vector2D.ScreenLeft :
				Key == Key.CursorRight || Key == Key.D ? Vector2D.ScreenRight :
				Key == Key.CursorUp || Key == Key.W ? Vector2D.ScreenUp :
				Key == Key.CursorDown || Key == Key.S ? Vector2D.ScreenDown : Vector2D.Zero;
		}

		public class CannotCreateKeyTriggerWithoutKey : Exception {}

		protected override void StartInputDevice()
		{
			Start<Keyboard>();
		}

		internal void HandleWithKeyboard(Keyboard keyboard)
		{
			if (keyboard.GetKeyState(Key) == State)
			{
				ZoomAmount = Key == Key.PageUp ? 0.1f : Key == Key.PageDown ? -0.1f : 0;
				Invoke();
			}
			else
				ZoomAmount = 0;
		}
	}
}