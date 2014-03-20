using DeltaEngine.Datatypes;

namespace $safeprojectname$.Logics
{
	public class SinglePlayerLogic : Logic
	{
		public SinglePlayerLogic(int width, int height)
			: base(width, height, new[] { Vector2D.Zero }) {}

		public override void MakeMove(Color color)
		{
			turns[0]++;
			if (IsGameOver)
				throw new CannotMakeMoveWhenGameIsOver();
			Board.SetColor(homeSquares[0], color);
			CheckForGameOver();
		}

		public override void Pass() {}
	}
}