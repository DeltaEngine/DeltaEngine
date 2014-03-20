using System;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Used for UI controls that can respond to keyboard input
	/// </summary>
	public interface KeyboardControllable
	{
		void UpdateTextFromKeyboardInput(Func<string, string> handleInput);
	}
}