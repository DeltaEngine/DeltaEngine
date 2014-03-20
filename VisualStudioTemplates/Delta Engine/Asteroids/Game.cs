using System.Globalization;
using DeltaEngine.Core;
using DeltaEngine.Extensions;
using DeltaEngine.Scenes;

namespace $safeprojectname$
{
	/// <summary>
	/// Game Logic and initialization for Asteroids
	/// </summary>
	public class Game : Scene
	{
		public Game(Window window)
		{
			highScores = new int[10];
			TryLoadingHighscores();
			SetUpBackground();
			mainMenu = new Menu();
			mainMenu.InitGame += StartGame;
			mainMenu.QuitGame += window.CloseAfterFrame;
			InteractionLogic = new InteractionLogic();
			mainMenu.UpdateHighscoreDisplay(highScores);
		}

		private int[] highScores;
		private readonly Menu mainMenu;

		private void TryLoadingHighscores()
		{
			/*currently unused
			var highscorePath = GetHighscorePath();
			if (!File.Exists(highscorePath))
				return; //ncrunch: no coverage, can't use files in mocks
			using (var stream = File.OpenRead(highscorePath))
			{
				var reader = new StreamReader(stream);
				GetHighscoresFromString(reader.ReadToEnd());
			}
			 */
		}

		/*unused
		private static string GetHighscorePath()
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
				"DeltaEngine", "Asteroids", "Highscores");
		}
		 */

		public void GetHighscoresFromString(string highscoreString)
		{
			if (string.IsNullOrEmpty(highscoreString))
				return;
			var partitions = highscoreString.SplitAndTrim(new[] { ',', ' ' });
			highScores = new int[10];
			for (int i = 0; i < highScores.Length; i++)
				try
				{
					highScores[i] = int.Parse(partitions[i]);
				}
				catch
				{
					highScores[i] = 0;
				}
		}

		public void StartGame()
		{
			mainMenu.Hide();
			Show();
			controls = new Controls(this);
			score = 0;
			GameState = GameState.Playing;
			InteractionLogic.BeginGame();
			SetUpEvents();
			controls.SetControlsToState(GameState);
			HudInterface = new HudInterface();
		}

		private void SetUpEvents()
		{
			InteractionLogic.GameOver += GameOver;
			InteractionLogic.IncreaseScore += increase =>
			{
				score += increase;
				HudInterface.SetScoreText(score);
			};
		}

		private Controls controls;
		private int score;
		public InteractionLogic InteractionLogic { get; private set; }
		public GameState GameState;
		public HudInterface HudInterface { get; private set; }

		private void SetUpBackground()
		{
			SetQuadraticBackground("AsteroidsBackground");
		}

		public void GameOver()
		{
			if (GameState == GameState.GameOver)
				return;
			RefreshHighScores();
			InteractionLogic.PauseUpdate();
			InteractionLogic.Player.Dispose();
			GameState = GameState.GameOver;
			controls.SetControlsToState(GameState);
			HudInterface.SetGameOverText();
		}

		public void RestartGame()
		{
			InteractionLogic.Restart();
			score = 0;
			HudInterface.SetScoreText(score);
			HudInterface.SetInGameMode();
			GameState = GameState.Playing;
			controls.SetControlsToState(GameState);
		}

		private void RefreshHighScores()
		{
			AddLastScoreToHighscoreIfQualified();
			mainMenu.UpdateHighscoreDisplay(highScores);
			SaveHighScore();
		}

		private void SaveHighScore()
		{
			/*currently unused
			var highscoreFilePath = GetHighscorePath();
			PathExtensions.CreateDirectoryIfNotExists(Path.GetDirectoryName(highscoreFilePath));
			using (FileStream highscoreFile = File.Create(highscoreFilePath))
			{
				var writer = new StreamWriter(highscoreFile);
				writer.Write(CreateHighscoreString());
				writer.Flush();
			}
			 */
		}

		//ncrunch: no coverage start
		private string CreateHighscoreString()
		{
			var stringOfScores = highScores[0].ToString(CultureInfo.InvariantCulture);
			for (int i = 1; i < highScores.Length; i++)
				stringOfScores += ", " + highScores[i].ToString(CultureInfo.InvariantCulture);
			return stringOfScores;
		}

		public void BackToMenu()
		{
			Hide();
			InteractionLogic.Dispose();
			controls.SetControlsToState(GameState.MainMenu);
			HudInterface.Dispose();
			mainMenu.Show();
		}

		private void AddLastScoreToHighscoreIfQualified()
		{
			if (score <= highScores[highScores.Length - 1])
				return;
			if (score > highScores[0])
			{
				highScores[0] = score;
				return;
			}
			for (int i = 0; i < highScores.Length - 2; i++)
				if (highScores[i] > score && score > highScores[i + 1])
					InsertNewScoreAt(i + 1);
		}

		private void InsertNewScoreAt(int index)
		{
			var scoreBuffer = highScores;
			highScores = new int[10];
			for (int i = 0; i < 10; i++)
				if (i == index)
					highScores[i] = score;
				else if (i > index)
					highScores[i] = scoreBuffer[i - 1];
				else
					highScores[i] = scoreBuffer[i];
		}
	}
}