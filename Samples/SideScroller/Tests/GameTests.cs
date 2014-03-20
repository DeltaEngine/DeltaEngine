using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using DeltaEngine.Scenes.Controls;
using NUnit.Framework;

namespace SideScroller.Tests
{
	public class GameTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateNewGame()
		{
			game = new Game(Resolve<Window>());
		}

		private Game game;

		[Test]
		public void StartTheGame()
		{
			var startButton = (InteractiveButton)game.mainMenu.Controls[IndexOfStartButton];
			startButton.Clicked.Invoke();
			Assert.IsNotNull(game.player);
		}

		private const int IndexOfStartButton = 1;

		[Test]
		public void ExitGame()
		{
			var exitButton = (InteractiveButton)game.mainMenu.Controls[IndexOfExitButton];
			exitButton.Clicked.Invoke();
		}

		private const int IndexOfExitButton = 2;

		[Test]
		public void CreateEnemiesAFterPointInTime()
		{
			game.StartGame();
			AdvanceTimeAndUpdateEntities(TimeToSpawnNewEnemy + 0.1f);
			Assert.AreEqual(1, EntitiesRunner.Current.GetEntitiesOfType<EnemyPlane>().Count);
		}

		private const float TimeToSpawnNewEnemy = 2;
	}
}