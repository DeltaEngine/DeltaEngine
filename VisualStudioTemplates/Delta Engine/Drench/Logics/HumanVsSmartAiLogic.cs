using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Datatypes;

namespace $safeprojectname$.Logics
{
	public class HumanVsSmartAiLogic : HumanVsAiLogic
	{
		public HumanVsSmartAiLogic(int width, int height)
			: base(width, height) {}

		protected override void MakeAiMove()
		{
			List<Color> validMoves = GetPlayerValidMoves(1);
			LastPlayerPassed = validMoves.Count == 0;
			if (validMoves.Count > 0)
				Board.SetColor(homeSquares[ActivePlayer], GetHighestScoringMove(validMoves));
		}

		private Color GetHighestScoringMove(List<Color> validMoves)
		{
			var scores = validMoves.Select(GetScoreForMove).ToList();
			for (int i = 0; i < scores.Count; i++)
				if (scores[i] == scores.Max())
					return validMoves[i];
			return validMoves[0]; //ncrunch: no coverage
		}

		private int GetScoreForMove(Color color)
		{
			Board clone = Board.Clone();
			clone.SetColor(homeSquares[1], color);
			return clone.GetConnectedColorsCount(homeSquares[1]);
		}
	}
}