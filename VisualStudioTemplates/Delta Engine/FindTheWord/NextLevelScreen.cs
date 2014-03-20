using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;
using DeltaEngine.Scenes.Controls;

namespace $safeprojectname$
{
	public class NextLevelScreen
	{
		public NextLevelScreen()
		{
			InitializeBackground();
		}

		private void InitializeBackground()
		{
			background = new Sprite("NextLevel", GetDrawArea());
			background.IsVisible = false;
			background.RenderLayer = 10;
		}

		private static Rectangle GetDrawArea()
		{
			return new Rectangle(0, 0.1875f, 1, 0.625f);
		}

		public void ShowAndWaitForInput()
		{
			background.IsVisible = true;
			InitializeButton();
		}

		private void InitializeButton()
		{
			advanceButton = new GameButton("NextLevelButton",
				Rectangle.FromCenter(0.5f, 0.6f, 0.2f, 0.1f));
			advanceButton.RenderLayer = 11;
			advanceButton.Clicked += HideAndStartNextLevel;
		}

		public void HideAndStartNextLevel()
		{
			background.IsVisible = false;
			advanceButton.IsActive = false;
			if (StartNextLevel != null)
				StartNextLevel();
		}

		public event Action StartNextLevel;
		private Sprite background;
		private Button advanceButton;
	}
}