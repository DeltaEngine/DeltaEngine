namespace DeltaEngine.Input
{
	/// <summary>
	/// Keyboard input message for remote input via networking.
	/// </summary>
	public class KeyboardMessage
	{
		public KeyboardMessage() {}

		public KeyboardMessage(Key[] pressedKeys)
		{
			PressedKeys = pressedKeys;
		}

		public Key[] PressedKeys { get; private set; }
	}
}