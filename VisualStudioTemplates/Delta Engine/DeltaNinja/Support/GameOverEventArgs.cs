using System;

namespace $safeprojectname$.Support
{
	public class GameOverEventArgs : EventArgs
	{
		public GameOverEventArgs(Score score)
		{
			Score = score;
		}

		public Score Score { get; private set; }
	}
}