using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace SideScroller.Tests
{
	internal class GameControlsTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			controlledPlayer = new PlayerPlane(Vector2D.Half);
			playerControls = new PlayerControls(controlledPlayer);
			keyboard = (MockKeyboard)Resolve<Keyboard>();
			originalVelocity = controlledPlayer.Get<Velocity2D>().Velocity;
		}

		private PlayerPlane controlledPlayer;
		private PlayerControls playerControls;
		private MockKeyboard keyboard;
		private Vector2D originalVelocity;

		[TearDown]
		public void DisposePlayerControls()
		{
			if (playerControls != null)
				playerControls.Dispose();
		}

		[Test]
		public void TestAscendControls()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			keyboard.SetKeyboardState(Key.W, State.Pressed);
			AdvanceTimeAndUpdateEntities();
			Assert.Less(controlledPlayer.Get<Velocity2D>().Velocity.Y, originalVelocity.Y);
		}

		[Test]
		public void TestSinkControls()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			keyboard.SetKeyboardState(Key.S, State.Pressed);
			AdvanceTimeAndUpdateEntities();
			Assert.Greater(controlledPlayer.Get<Velocity2D>().Velocity.Y, originalVelocity.Y);
		}
	}
}