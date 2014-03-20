using DeltaEngine.Datatypes;

namespace $safeprojectname$.Logics
{
	public abstract class HumanVsAiLogic : TwoPlayerLogic
	{
		protected HumanVsAiLogic(int width, int height)
			: base(width, height) {}

		public override void MakeMove(Color color)
		{
			Board.SetColor(homeSquares[0], color);
			turns[0]++;
			CheckForGameOver();
			if (IsGameOver)
				return;
			ActivePlayer = 1;
			ProcessComputerMove();
		}

		private void ProcessComputerMove()
		{
			MakeAiMove();
			if (!LastPlayerPassed)
				turns[1]++;
			CheckForGameOver();
			ActivePlayer = 0;
		}

		protected abstract void MakeAiMove();

		public override void Pass()
		{
			ActivePlayer = 1;
			ProcessComputerMove();
		}
	}
}