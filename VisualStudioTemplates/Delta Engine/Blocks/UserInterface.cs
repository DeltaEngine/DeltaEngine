using System;

namespace $safeprojectname$
{
	/// <summary>
	/// Swaps from landscape to portrait mode as the window aspect changes
	/// </summary>
	public class UserInterface : IDisposable
	{
		public UserInterface(BlocksContent content)
		{
			userInterfaceLandscape = new UserInterfaceLandscape(content);
			userInterfacePortrait = new UserInterfacePortrait(content);
			ShowUserInterfaceLandscape();
		}

		private readonly UserInterfaceLandscape userInterfaceLandscape;
		private readonly UserInterfacePortrait userInterfacePortrait;

		public void ShowUserInterfaceLandscape()
		{
			userInterfacePortrait.Hide();
			userInterfaceLandscape.ResizeInterface();
			userInterfaceLandscape.Show();
		}

		public void ShowUserInterfacePortrait()
		{
			userInterfaceLandscape.Hide();
			userInterfaceLandscape.ResizeInterface();
			userInterfacePortrait.Show();
		}

		public void AddToScore(int points)
		{
			Score += points;
			userInterfacePortrait.Text.Text = "Score: " + Score;
			userInterfaceLandscape.Text.Text = "Score: " + Score;
		}

		public void Lose()
		{
			userInterfacePortrait.Text.Text = "Final Score: " + Score;
			userInterfaceLandscape.Text.Text = "Final Score: " + Score;
			Score = 0;
		}

		public int Score { get; set; }

		public void Dispose()
		{
			userInterfaceLandscape.Hide();
			userInterfacePortrait.Hide();
		}
	}
}