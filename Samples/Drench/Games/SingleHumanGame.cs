using DeltaEngine.Datatypes;
using Drench.Logics;

namespace Drench.Games
{
	public class SingleHumanGame : Game
	{
		public SingleHumanGame(int width, int height)
			: base(new SinglePlayerLogic(width, height))
		{
			upperText.Text = "Try to complete the grid in the lowest number of turns!";
		}

		protected override bool ProcessDesiredMove(int x, int y)
		{
			Color color = logic.Board.GetColor(x, y);
			bool isValid = logic.GetPlayerValidMoves(0).Contains(color);
			if (isValid)
				logic.MakeMove(color);
			upperText.Text = isValid ? GetTurnsText() : GetTurnsText() + " - Invalid Move!";
			return isValid;
		}

		private string GetTurnsText()
		{
			int turns = logic.GetPlayerTurnsTaken(0);
			var text = logic.GetPlayerTurnsTaken(0) == 1 ? "turn" : "turns";
			return turns + " " + text + " taken";
		}

		protected override void GameOver()
		{
			upperText.Text = "Game Over! " + "Finished in " + GetTurnsText();
		}
	}
}