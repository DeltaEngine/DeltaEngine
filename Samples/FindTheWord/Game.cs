namespace FindTheWord
{
	public class Game
	{
		public Game(StartupScreen startScreen, GameScreen gameScreen)
		{
			this.startScreen = startScreen;
			startScreen.GameStarted += OnStartupScreenGameStarted;
			this.gameScreen = gameScreen;
			gameScreen.Hide();
		}

		private readonly StartupScreen startScreen;
		private readonly GameScreen gameScreen;

		private void OnStartupScreenGameStarted()
		{
			startScreen.FadeOut();
			gameScreen.FadeIn();
		}
	}
}