namespace Breakout.Tests
{
	public class EmptyLevel : Level
	{
		public EmptyLevel(Score score)
			: base(score)
		{
			for (int x = 0; x < rows; x++)
				for (int y = 0; y < columns; y++)
					bricks[x, y].IsVisible = false;
		}
	}
}