using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;

namespace FindTheWord
{
	public class StartupScreen : Sprite
	{
		public StartupScreen()
			: base("StartScreen", GetDrawArea())
		{
			CreateStartButton();
		}

		private static Rectangle GetDrawArea()
		{
			return new Rectangle(0, 0.1875f, 1, 0.625f);
		} 

		private void CreateStartButton()
		{
			const float ButtonWidth = 268.0f / 1280.0f;
			const float ButtonHeight = 164.0f / 1280.0f;
			const float BottomRightGap = 0.025f;
			float xPos = DrawArea.Right - BottomRightGap - ButtonWidth;
			float yPos = DrawArea.Bottom - BottomRightGap - ButtonHeight;
			Rectangle startDrawArea = new Rectangle(xPos, yPos, ButtonWidth, ButtonHeight);
			StartButton = new GameButton("StartButton", startDrawArea);
			StartButton.Clicked += StartGame;
		}

		public GameButton StartButton { get; private set; }

		public void StartGame()
		{
			if (GameStarted != null)
				GameStarted();
			FadeOut();
		}

		public void FadeOut()
		{
			IsVisible = false;
			StartButton.IsVisible = false;
		}

		public event Action GameStarted;
	}
}