using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Input.Windows.Tests
{
	class WindowsGamepadTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateWindowsGamepad()
		{
			if (StackTraceExtensions.StartedFromNUnitConsoleButNotFromNCrunch)
				Assert.Ignore(); //ncrunch: no coverage
			Resolve<GamePad>().Dispose();
			gamePad = new WindowsGamePad();
		}

		private WindowsGamePad gamePad;

		[Test, CloseAfterFirstFrame]
		public void UpdateGamePad()
		{
			var buttonTrigger = new GamePadButtonTrigger(GamePadButton.A);
			var joyStickTrigger = new GamePadAnalogTrigger(GamePadAnalog.LeftTrigger);
			gamePad.Update(new Trigger[]{buttonTrigger, joyStickTrigger});
		}

		[Test, CloseAfterFirstFrame]
		public void GetIdleStates()
		{
			Assert.AreEqual(State.Released, gamePad.GetButtonState(GamePadButton.A));
			Assert.AreEqual(Vector2D.Zero, gamePad.GetLeftThumbStick());
			Assert.AreEqual(Vector2D.Zero, gamePad.GetRightThumbStick());
			Assert.AreEqual(0.0f, gamePad.GetLeftTrigger());
			Assert.AreEqual(0.0f, gamePad.GetRightTrigger());
		}

		[Test, CloseAfterFirstFrame]
		public void TestUpdateGamePad()
		{
			gamePad = new MockWindowsGamePad(GamePadNumber.Two);
			var buttonTrigger = new GamePadButtonTrigger(GamePadButton.A);
			var joyStickTrigger = new GamePadAnalogTrigger(GamePadAnalog.LeftTrigger);
			gamePad.Update(new Trigger[] { buttonTrigger, joyStickTrigger });
			Assert.AreEqual(0.0f, gamePad.GetLeftTrigger());
			Assert.IsTrue(gamePad.IsAvailable);
			gamePad.Dispose();
		}

		[Test, CloseAfterFirstFrame]
		public void CheckGamePadNumber()
		{
			var mockGamePad = new MockWindowsGamePad(GamePadNumber.Three);
			var buttonTrigger = new GamePadButtonTrigger(GamePadButton.A);
			var joyStickTrigger = new GamePadAnalogTrigger(GamePadAnalog.LeftTrigger);
			mockGamePad.Update(new Trigger[] { buttonTrigger, joyStickTrigger });
			Assert.AreEqual(GamePadNumber.Three, mockGamePad.GetGamePadNumber());
			mockGamePad = new MockWindowsGamePad(GamePadNumber.Four);
			mockGamePad.Update(new Trigger[] { buttonTrigger, joyStickTrigger });
			Assert.AreEqual(GamePadNumber.Four, mockGamePad.GetGamePadNumber());
		}

		private class MockWindowsGamePad : WindowsGamePad
		{
			public MockWindowsGamePad(GamePadNumber number)
				: base(number) {}

			public override bool IsAvailable
			{
				get { return true; }
			}

			public GamePadNumber GetGamePadNumber()
			{
				return Number;
			}
		}
	}
}