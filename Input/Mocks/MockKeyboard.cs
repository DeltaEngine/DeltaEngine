namespace DeltaEngine.Input.Mocks
{
	/// <summary>
	/// Mock keyboard for unit testing.
	/// </summary>
	public sealed class MockKeyboard : Keyboard
	{
		public MockKeyboard()
		{
			IsAvailable = true;
		}

		public override void Dispose()
		{
			IsAvailable = false;
		}

		public void SetKeyboardState(Key key, State state)
		{
			keyboardStates[(int)key] = state;
			if (state == State.Pressing)
				newlyPressedKeys.Add(key);
		}

		protected override void UpdateKeyStates() {} //ncrunch: no coverage

		protected override bool IsCapsLocked
		{
			get { return false; }
		}
	}
}