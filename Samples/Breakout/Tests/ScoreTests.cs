using DeltaEngine.Platforms;
using NUnit.Framework;

namespace Breakout.Tests
{
	public class ScoreTests : TestWithMocksOrVisually
	{
		[SetUp, CloseAfterFirstFrame]
		public void Init()
		{
			score = Resolve<Score>();
		}

		private Score score;

		[Test]
		public void IncreasePoints()
		{
			Assert.IsTrue(score.ToString().Contains("Score: 0"), score.ToString());
			score.IncreasePoints();
		}

		[Test]
		public void NextLevelWithoutInitialization()
		{
			bool isGameOver = false;
			score.GameOver += () => isGameOver = true;
			Assert.AreEqual(1, score.Level);
			score.NextLevel();
			Assert.AreEqual(2, score.Level);
			Assert.IsFalse(isGameOver);
		}

		[Test]
		public void NextLevelWithLevelInitialization()
		{
			Resolve<Level>();
			bool isGameOver = false;
			score.GameOver += () => isGameOver = true;
			Assert.AreEqual(1, score.Level);
			score.NextLevel();
			Assert.AreEqual(2, score.Level);
			Assert.IsFalse(isGameOver);
		}

		[Test]
		public void LoseLivesUntilGameOver()
		{
			bool isGameOver = false;
			score.GameOver += () => isGameOver = true;
			score.LifeLost();
			score.LifeLost();
			score.LifeLost();
			Assert.IsTrue(isGameOver);
		}
	}
}