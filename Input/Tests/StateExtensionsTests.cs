using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class StateExtensionsTests
	{
		[Test]
		public void TestUpdateStates()
		{
			var state = State.Released;
			state = state.UpdateOnNativePressing(true);
			Assert.AreEqual(State.Pressing, state);
			state = state.UpdateOnNativePressing(true);
			Assert.AreEqual(State.Pressed, state);
			state = state.UpdateOnNativePressing(false);
			Assert.AreEqual(State.Releasing, state);
			state = state.UpdateOnNativePressing(false);
			Assert.AreEqual(State.Released, state);
		}
	}
}