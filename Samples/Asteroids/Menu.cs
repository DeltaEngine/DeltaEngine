using System;
using System.Globalization;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;

namespace Asteroids
{
	internal class Menu : Scene
	{
		public Menu()
		{
			var mainMenu = ContentLoader.Load<Scene>("MainMenu");
			//ncrunch: no coverage start
			foreach (var control in mainMenu.Controls)
			{
				Add(control);
				if(!control.GetType().IsSubclassOf(typeof(Button)))
					continue;
				var button = control as Button;
				if (button.Name == "StartGame")
				{
					button.Clicked += TryInvokeGameStart;
				}
				else if (button.Name =="HowToPlay")
				{
					button.Clicked += ShowHowToPlaySubMenu;
				}
				else if (button.Name =="HighScores")
				{
					button.Clicked += ShowHighScoresSubMenu;
				}
				else if (button.Name =="QuitGame")
				{
					button.Clicked += TryInvokeQuit;
				}
			}
		}

		private void TryInvokeGameStart()
		{
			if (InitGame != null)
				InitGame();
		} 

		public event Action InitGame;

		private void ShowHowToPlaySubMenu()
		{
			if (howToPlay == null)
				howToPlay = ContentLoader.Load<Scene>("HowToPlayMenu");
			foreach (var control in howToPlay.Controls)
			{
				if (!control.GetType().IsSubclassOf(typeof(Button)))
					continue;
				var button = control as Button;
				if (button.Name == "Back")
				{
					button.Clicked += () =>
					{
						howToPlay.Hide();
						Show();
					};
					break;
				}
			}
			howToPlay.Show();
			Hide();
		}

		private Scene howToPlay;

		private void ShowHighScoresSubMenu()
		{
			if (highscore == null)
			{
				highscore = ContentLoader.Load<Scene>("HighScoresMenu");
				foreach (var control in highscore.Controls)
				{
					if (!control.GetType().IsSubclassOf(typeof(Button)))
						continue;
					var button = control as Button;
					if (button.Name == "Back")
					{
						button.Clicked += () =>
						{
							highscore.Hide();
							Show();
						};
						break;
					}
				}
				scoreboard = new FontText(Font.Default, "",
					Rectangle.FromCenter(new Vector2D(0.5f, 0.45f), new Size(0.3f, 0.1f)));
				highscore.Add(scoreboard);
			}
			scoreboard.Text = ScoreboardText;
			highscore.Show();
			Hide();
		}

		private Scene highscore;
		private FontText scoreboard;

		private void TryInvokeQuit()
		{
			if (QuitGame != null)
				QuitGame();
		}

		public event Action QuitGame;

		public void UpdateHighscoreDisplay(int[] highscores)
		{
			ScoreboardText = "\n";
			for (int i = 0; i < highscores.Length; i++)
				ScoreboardText += "\n" + (i + 1).ToString(CultureInfo.InvariantCulture) + ": " +
					highscores[i];
		}
		//ncrunch: no coverage end

		public string ScoreboardText { get; private set; }
	}
}