using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace GhostWars.Tests
{
	public class MenuTests : TestWithMocksOrVisually
	{
		//ncrunch: no coverage start
		[Test, Ignore]
		public void ShowMenu()
		{
			Resolve<Settings>().Resolution = new Size(1200, 750);
			new MainMenu(Resolve<Window>());
		}
		//ncrunch: no coverage end

		[Test]
		public void BackToMenuOnGameOver()
		{
			GiveMenuSimulatingGameLost();
			Assert.AreEqual(MainMenu.State, GameState.Menu);
		}

		private MainMenu GiveMenuSimulatingGameLost(int level = 1) 
		{
			var menu = new MainMenu(Resolve<Window>());
			menu.CurrentLevel = level;
			menu.Clear();
			menu.SetGameOverState();
			return menu;
		}

		[Test]
		public void SetGameOverAndRestartBackToCountDown()
		{
			var menu = GiveMenuSimulatingGameLost();
			menu.RestartGame();
			Assert.AreEqual(MainMenu.State, GameState.CountDown);
		}

		[Test]
		public void RestartingStartsTheSameLevelAgain()
		{
			var menu = GiveMenuSimulatingGameLost(2);
			menu.RestartGame();
			Assert.AreEqual(2, menu.CurrentLevel);
		}
	}
}