using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace Asteroids.Tests
{
	internal class GameTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void GameOverResultsInSameStateEvenMultipleCalls()
		{
			CreateAndStartGame();
			game.GameOver();
			game.GameOver();
			Assert.AreEqual(GameState.GameOver, game.GameState);
			Assert.IsFalse(game.InteractionLogic.Player.IsActive);
		}

		private void CreateAndStartGame()
		{
			game = new Game(Resolve<Window>());
			game.StartGame();
		}

		private Game game;

		[Test, CloseAfterFirstFrame]
		public void RestartGameGivesRunningGameAgain()
		{
			CreateAndStartGame();
			game.RestartGame();
			Assert.AreEqual(GameState.Playing, game.GameState);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeViewPortSizeInGame()
		{
			var window = Resolve<Window>();
			if(window.GetType() != typeof(MockWindow))
				return; //ncrunch: no coverage
			game = new Game(window);
			game.StartGame();
			window.ViewportPixelSize = new Size(800,600);
		}

		[Test, CloseAfterFirstFrame]
		public void IncreaseScoreToScoreBoard()
		{
			CreateAndStartGame();
			game.InteractionLogic.IncrementScore(1);
			Assert.AreEqual("1", game.HudInterface.ScoreDisplay.Text);
		}

		[Test, CloseAfterFirstFrame]
		public void ExtractHighscoresFromString()
		{
			CreateAndStartGame();
			game.GetHighscoresFromString("");
			game.GetHighscoresFromString("100,30,20");
		}
	}
}