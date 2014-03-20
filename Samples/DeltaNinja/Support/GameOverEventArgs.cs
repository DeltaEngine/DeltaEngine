using System;

namespace DeltaNinja.Support
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