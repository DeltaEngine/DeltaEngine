using DeltaEngine.Platforms;
using NUnit.Framework;

namespace FindTheWord.Tests
{
	public class GameTests : TestWithMocksOrVisually
	{
		[Test, Ignore]
		public void InitializeGame()
		{
			var startupScreen = Resolve<StartupScreen>();
			var gameScreen = Resolve<GameScreen>();
			new Game(startupScreen, gameScreen);
			startupScreen.StartGame();
			Assert.IsTrue(startupScreen.IsVisible);
		}
	}
}