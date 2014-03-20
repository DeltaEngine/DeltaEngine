using DeltaEngine.Core;
using DeltaEngine.Platforms;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace Breakout.Tests
{
	public class GameTests : TestWithMocksOrVisually
	{
		[Test]
		public void Draw()
		{
			Resolve<Paddle>();
			Resolve<RelativeScreenSpace>();
			new Game(Resolve<Window>());
		}

		[Test, CloseAfterFirstFrame]
		public void RemoveBallIfGameIsOver()
		{
			var score = Resolve<Score>();
			bool isGameOver = false;
			score.GameOver += () => isGameOver = true;
			score.LifeLost();
			score.LifeLost();
			score.LifeLost();
			Assert.IsTrue(isGameOver);
		}

		[Test, CloseAfterFirstFrame]
		public void KillingAllBricksShouldAdvanceToNextLevel()
		{
			bool isGameOver = false;
			var level = Resolve<Level>();
			var score = Resolve<Score>();
			Score remScore = score;
			remScore.GameOver += () => isGameOver = true;
			Assert.AreEqual(1, score.Level);
			DisposeAllBricks(level);
			Assert.AreEqual(0, level.BricksLeft);
			Assert.AreEqual(1, remScore.Level);
			Assert.IsFalse(isGameOver);
		}

		private static void DisposeAllBricks(Level level)
		{
			for (float x = 0; x < 1.0f; x += 0.1f)
				for (float y = 0; y < 1.0f; y += 0.1f)
					if (level.GetBrickAt(x, y) != null)
						level.GetBrickAt(x, y).IsVisible = false;
		}
	}
}