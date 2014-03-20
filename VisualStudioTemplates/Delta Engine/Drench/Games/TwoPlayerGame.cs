using Drench.Logics;

namespace $safeprojectname$.Games
{
	public abstract class TwoPlayerGame : Game
	{
		protected TwoPlayerGame(TwoPlayerLogic twoPlayerLogic)
			: base(twoPlayerLogic) {}

		protected override void GameOver()
		{
			if (logic.GetPlayerScore(0) > logic.GetPlayerScore(1))
				UpdateText("Game Over! Player 1 wins!", "");
			else
				UpdateText("", "Game Over! Player 2 wins!"); //ncrunch: no coverage
		}

		protected void UpdateText(string upper, string lower)
		{
			upperText.Text = PlayerOneTurnIndicator + " Player 1: " + logic.GetPlayerScore(0) + " " +
				upper + " " + PlayerOneTurnIndicator;
			lowerText.Text = PlayerTwoTurnIndicator + "Player 2: " + logic.GetPlayerScore(1) + " " +
				lower + " " + PlayerTwoTurnIndicator;
		}

		private string PlayerOneTurnIndicator
		{
			get { return logic.ActivePlayer == 0 ? "***" : ""; }
		}

		private string PlayerTwoTurnIndicator
		{
			get { return logic.ActivePlayer == 1 ? "***" : ""; }
		}
	}
}