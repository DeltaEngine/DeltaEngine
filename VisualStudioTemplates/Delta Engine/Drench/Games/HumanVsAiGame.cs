using DeltaEngine.Datatypes;
using Drench.Logics;

namespace $safeprojectname$.Games
{
	public class HumanVsAiGame : TwoPlayerGame
	{
		public HumanVsAiGame(HumanVsAiLogic humanVsAiLogic)
			: base(humanVsAiLogic)
		{
			UpdateText("", "");
		}

		protected override bool ProcessDesiredMove(int x, int y)
		{
			UpdateText("", "");
			Color color = logic.Board.GetColor(x, y);
			bool isValid = logic.GetPlayerValidMoves(0).Contains(color);
			if (isValid)
				MakeMove(color);
			else
				UpdateText("- Invalid Move!", "");
			while (!logic.HasPlayerAnyValidMoves(0) && !logic.IsGameOver)
				logic.Pass(); //ncrunch: no coverage
			return isValid;
		}

		private void MakeMove(Color color)
		{
			logic.MakeMove(color);
			if (logic.LastPlayerPassed)
				UpdateText("", "(AI passes)"); //ncrunch: no coverage
		}
	}
}