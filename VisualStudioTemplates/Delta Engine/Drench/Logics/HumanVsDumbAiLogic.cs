using System.Collections.Generic;
using DeltaEngine.Datatypes;

namespace $safeprojectname$.Logics
{
	public class HumanVsDumbAiLogic : HumanVsAiLogic
	{
		public HumanVsDumbAiLogic(int width, int height)
			: base(width, height) {}

		protected override void MakeAiMove()
		{
			List<Color> validMoves = GetPlayerValidMoves(1);
			LastPlayerPassed = validMoves.Count == 0;
			if (validMoves.Count > 0)
				Board.SetColor(homeSquares[ActivePlayer], validMoves[0]);
		}
	}
}