using DeltaEngine.Datatypes;

namespace Drench.Logics
{
	public abstract class TwoPlayerLogic : Logic
	{
		protected TwoPlayerLogic(int width, int height)
			: base(width, height, new[] { Vector2D.Zero, new Vector2D(width - 1, height - 1) }) {}

		protected TwoPlayerLogic(Board.Data boardData)
			: base(boardData, new[] { Vector2D.Zero, new Vector2D(boardData.Width - 1, boardData.Height - 1) }
				) {}
	}
}