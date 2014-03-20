using System;

namespace $safeprojectname$
{
	/// <summary>
	/// Each brick can either be dead or alive, has a color and the bounding
	/// rectangle for rendering and collision detection.
	/// </summary>
	public class Score
	{
		public Score()
		{
			Level = InitialLevel;
		}

		public int Level { get; private set; }
		private const int InitialLevel = 1;

		public void NextLevel()
		{
			Level++;
			lives++;
		}

		private int lives = InitialLives;
		private const int InitialLives = 3;

		public void IncreasePoints()
		{
			destroyedBlocksInARow++;
			points += destroyedBlocksInARow + Level;
		}

		private int destroyedBlocksInARow;
		private int points;

		public void LifeLost()
		{
			lives--;
			destroyedBlocksInARow /= 2;
			if (lives == 0 && GameOver != null)
				GameOver();
		}

		public event Action GameOver;

		public override string ToString()
		{
			return "Level: " + Level + ", Score: " + points +
				(lives == 0 ? " - GAME OVER" : ", Lives: " + lives);
		}
	}
}