using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace Breakout.Tests
{
	public class PaddleTests : TestWithMocksOrVisually
	{
		[Test]
		public void Draw()
		{
			var ball = Resolve<TestBall>();
			var paddle = Resolve<Paddle>();
			Assert.AreEqual(0.5f, paddle.Position.X);
			Assert.IsTrue(ball.IsCurrentlyOnPaddle);
			Assert.AreEqual(0.5f, ball.Position.X);
		}

		[Test]
		public void ControlPaddleVirtuallyWithKeyboard()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			Resolve<TestBall>();
			var paddle = Resolve<Paddle>();
			var keyboard = Resolve<MockKeyboard>();
			keyboard.SetKeyboardState(Key.CursorLeft, State.Pressed);
			AssertPaddleMovesLeftCorrectly(paddle);
			keyboard.SetKeyboardState(Key.CursorLeft, State.Released);
			keyboard.SetKeyboardState(Key.CursorRight, State.Pressed);
			AssertPaddleMovesRightCorrectly(paddle);
		}

		private void AssertPaddleMovesLeftCorrectly(Paddle paddle)
		{
			Assert.AreEqual(0.5f, paddle.Position.X);
			AdvanceTimeAndUpdateEntities(0.1f);
			Assert.Less(paddle.Position.X, 0.5f);
			Assert.Greater(paddle.Position.Y, 0.75f);
		}

		private void AssertPaddleMovesRightCorrectly(Paddle paddle)
		{
			AdvanceTimeAndUpdateEntities(0.2f);
			Assert.Greater(paddle.Position.X, 0.5f);
		}

		[Test, CloseAfterFirstFrame]
		public void ControlPaddleVirtuallyWithGamePad()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			var paddle = Resolve<Paddle>();
			var gamePad = Resolve<MockGamePad>();
			gamePad.SetGamePadState(GamePadButton.Left, State.Pressed);
			AssertPaddleMovesLeftCorrectly(paddle);
			gamePad.SetGamePadState(GamePadButton.Left, State.Released);
			gamePad.SetGamePadState(GamePadButton.Right, State.Pressed);
			AssertPaddleMovesRightCorrectly(paddle);
		}

		[Test, CloseAfterFirstFrame]
		public void ControlPaddleVirtuallyWithMouse()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			var paddle = Resolve<Paddle>();
			var mouse = Resolve<MockMouse>();
			mouse.SetNativePosition(new Vector2D(0.45f, 0.76f));
			mouse.SetButtonState(MouseButton.Left, State.Pressed);
			AssertPaddleMovesLeftCorrectly(paddle);
			mouse.SetNativePosition(new Vector2D(0.55f, 0.76f));
			mouse.SetButtonState(MouseButton.Left, State.Released);
			mouse.SetButtonState(MouseButton.Left, State.Pressed);
			AssertPaddleMovesRightCorrectly(paddle);
		}

		[Test, CloseAfterFirstFrame]
		public void IsBallReleasedAfterSpacePressed()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			var ball = Resolve<TestBall>();
			Resolve<Paddle>();
			PressSpaceOneSecond();
			Assert.IsFalse(ball.IsCurrentlyOnPaddle);
			Assert.AreNotEqual(0.5f, ball.Position.X);
		}

		private void PressSpaceOneSecond()
		{
			Resolve<MockKeyboard>().SetKeyboardState(Key.Space, State.Pressing);
			AdvanceTimeAndUpdateEntities(1);
		}
	}
}