using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace Breakout.Tests
{
	public class LevelTests : TestWithMocksOrVisually
	{
		[Test]
		public void Draw()
		{
			Resolve<Level>();
		}

		[Test]
		public void ForceResolutionChange()
		{
			Resolve<Level>();
			Resolve<Window>().ViewportPixelSize = new Size(400, 600);
		}

		[Test]
		public void DrawLevelTwo()
		{
			var level = Resolve<Level>();
			level.InitializeNextLevel();
		}

		[Test]
		public void DrawLevelThree()
		{
			var level = Resolve<Level>();
			level.InitializeNextLevel();
			level.InitializeNextLevel();
		}

		[Test]
		public void DrawLevelFour()
		{
			var level = Resolve<Level>();
			level.InitializeNextLevel();
			level.InitializeNextLevel();
			level.InitializeNextLevel();
		}

		[Test]
		public void DrawLevelFive()
		{
			var level = Resolve<Level>();
			level.InitializeNextLevel();
			level.InitializeNextLevel();
			level.InitializeNextLevel();
			level.InitializeNextLevel();
		}

		[Test]
		public void SwitchLevels()
		{
			var level = Resolve<Level>();
			var score = Resolve<Score>();
			for (int levelNum = 1; levelNum <= 5; levelNum++)
			{
				Assert.AreEqual(levelNum, score.Level);
				level.InitializeNextLevel();
			}
		}

		[Test, CloseAfterFirstFrame]
		public void GetBrickAtScreenPosition()
		{
			var level = Resolve<Level>();
			Assert.Null(level.GetBrickAt(0f, 0.6f));
			Assert.Null(level.GetBrickAt(1f, 0f));
			Assert.NotNull(level.GetBrickAt(0.25f, 0.25f));
		}

		[Test, CloseAfterFirstFrame]
		public void CheckEmptyLevel()
		{
			Resolve<Paddle>();
			var level = Resolve<EmptyLevel>();
			Assert.IsNull(level.GetBrickAt(0.25f, 0.25f));
			Assert.IsNull(level.GetBrickAt(0.5f, 0.25f));
			Assert.IsNull(level.GetBrickAt(0.75f, 0.35f));
			Assert.AreEqual(0, level.BricksLeft);
		}

		[Test, CloseAfterFirstFrame]
		public void RemoveBrick()
		{
			var level = Resolve<Level>();
			Assert.AreEqual(4, level.BricksLeft);
			var brick = level.GetBrickAt(0.25f, 0.25f);
			Assert.IsTrue(brick.IsVisible);
			brick.IsVisible = false;
			Assert.AreEqual(3, level.BricksLeft);
			Assert.IsNull(level.GetBrickAt(0.25f, 0.25f));
		}

		[Test, CloseAfterFirstFrame]
		public void UpdateGameWithBallReleased()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			var remBall = Resolve<TestBall>();
			Resolve<Level>();
			Resolve<MockKeyboard>().SetKeyboardState(Key.Space, State.Pressing);
			AdvanceTimeAndUpdateEntities(0.1f);
			if (remBall != null)
				Assert.IsFalse(remBall.IsCurrentlyOnPaddle);
		}

		[Test, CloseAfterFirstFrame]
		public void LifeLostLeadsToGameOver()
		{
			bool lost = false;
			var score = new Score();
			score.GameOver += () => lost = true;
			var level = new Level(score);
			level.LifeLost();
			level.LifeLost();
			level.LifeLost();
			Assert.True(lost);
		}

		[Test, CloseAfterFirstFrame]
		public void BricksLeft()
		{
			var level = Resolve<Level>();
			level.GetBrickAt(0.25f, 0.125f).IsVisible = false;
			level.GetBrickAt(0.75f, 0.125f).IsVisible = false;
			level.GetBrickAt(0.25f, 0.375f).IsVisible = false;
			level.GetBrickAt(0.75f, 0.375f).IsVisible = false;
			Assert.AreEqual(0, level.BricksLeft);
		}
	}
}