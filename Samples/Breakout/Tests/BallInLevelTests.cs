using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace Breakout.Tests
{
	public class BallInLevelTests : TestWithMocksOrVisually
	{
		[Test]
		public void Draw()
		{
			Resolve<BallInLevel>();
		}

		[Test, CloseAfterFirstFrame]
		public void FireBall()
		{
			Resolve<BallInLevel>();
			var ball = Resolve<Ball>();
			Assert.IsTrue(ball.IsVisible);
			AdvanceTimeAndUpdateEntities(0.01f);
			var initialBallPosition = new Vector2D(0.5f, 0.86f);
			Assert.AreEqual(initialBallPosition, ball.Position);
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			Resolve<MockKeyboard>().SetKeyboardState(Key.Space, State.Pressing);
			AdvanceTimeAndUpdateEntities(1.0f);
			Assert.AreNotEqual(initialBallPosition, ball.Position);
		}

		[Test, CloseAfterFirstFrame]
		public void ResetBallLosesLive()
		{
			var score = Resolve<Score>();
			bool lost = false;
			score.GameOver += () => lost = true;
			var ball = Resolve<BallInLevel>();
			ball.ResetBall();
			ball.ResetBall();
			ball.ResetBall();
			Assert.IsTrue(lost);
		}

		[Test, CloseAfterFirstFrame]
		public void BallUpdateStartsNewLevel()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			Resolve<BallInLevel>();
			var level = Resolve<Level>();
			level.GetBrickAt(0.25f, 0.125f).IsVisible = false;
			level.GetBrickAt(0.75f, 0.125f).IsVisible = false;
			level.GetBrickAt(0.25f, 0.375f).IsVisible = false;
			level.GetBrickAt(0.75f, 0.375f).IsVisible = false;
			Assert.AreEqual(0, level.BricksLeft);
			Resolve<MockKeyboard>().SetKeyboardState(Key.Space, State.Pressing);
			AdvanceTimeAndUpdateEntities();
			Assert.AreNotEqual(0, level.BricksLeft);
		}

		[Test]
		public void PlayGameWithGravity()
		{
			Resolve<Paddle>();
			Resolve<BallWithGravity>();
		}

		private class BallWithGravity : BallInLevel
		{
			public BallWithGravity(Paddle paddle, Level level)
				: base(paddle, level) {}

			//ncrunch: no coverage start
			protected override void UpdateInFlight(float timeDelta)
			{
				var gravity = new Vector2D(0.0f, 9.81f);
				velocity += gravity * 0.15f * timeDelta;
				base.UpdateInFlight(timeDelta);
			}
		}
	}
}