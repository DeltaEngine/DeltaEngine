namespace DeltaEngine.Input
{
	/// <summary>
	/// All states a mouse button, keyboard key, touch, gamepad or gesture may have.
	/// </summary>
	public enum State
	{
		/// <summary>
		/// Default state for any button, key or gesture, no input is happening, no event will be fired
		/// </summary>
		Released,
		/// <summary>
		/// This state will be set when the key or button is released again, which can happen in the
		/// very same frame as the Pressing event. Use this event for button clicks and gestures.
		/// </summary>
		Releasing,
		/// <summary>
		/// A button or key was just pressed, this will be true for the first tick when the button or
		/// key was just pressed. Not used for most gestures, which only care about the Releasing state
		/// </summary>
		Pressing,
		/// <summary>
		/// This state will be true for every tick the button or key is being pressed (except for the
		/// initial tick when Pressing is used, to check for all pressing states use PressingOrPressed).
		/// Events of this type are also fired each frame.
		/// </summary>
		Pressed
	}
}