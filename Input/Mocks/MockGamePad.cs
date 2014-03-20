using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Input.Mocks
{
	/// <summary>
	/// Mock game pad for unit testing.
	/// </summary>
	public sealed class MockGamePad : GamePad
	{
		public MockGamePad()
			: base(GamePadNumber.Any)
		{
			IsAvailable = true;
			gamePadButtonStates = new State[GamePadButton.A.GetCount()];
		}

		public override bool IsAvailable { get; protected set; }
		private static State[] gamePadButtonStates;

		public override void Dispose() {}

		public override Vector2D GetLeftThumbStick()
		{
			return Vector2D.Zero;
		}

		public override Vector2D GetRightThumbStick()
		{
			return Vector2D.Zero;
		}

		public override float GetLeftTrigger()
		{
			return 0.0f;
		}

		public override float GetRightTrigger()
		{
			return 0.0f;
		}

		public override State GetButtonState(GamePadButton button)
		{
			return gamePadButtonStates[(int)button];
		}

		public void SetGamePadState(GamePadButton button, State state)
		{
			gamePadButtonStates[(int)button] = state;
		}

		public override void Vibrate(float strength) {}

		protected override void UpdateGamePadStates() {}
		public void SetUnavailable()
		{
			IsAvailable = false;
		}
	}
}