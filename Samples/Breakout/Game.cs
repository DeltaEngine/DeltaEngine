using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Scenes;
using DeltaEngine.ScreenSpaces;

namespace Breakout
{
	/// <summary>
	/// Renders the background, ball, level and score; Also handles starting new levels
	/// </summary>
	public class Game : Scene
	{
		public Game(Window window)
		{
			screenSpace = new Camera2DScreenSpace(window);
			this.window = window;
			menu = new MainMenu();
			menu.InitGame += InitGame;
			menu.QuitGame += window.CloseAfterFrame;
			window.ViewportPixelSize = Settings.Current.Resolution;
			soundTrack = ContentLoader.Load<Music>("BreakoutMusic");
			soundTrack.Loop = true;
			soundTrack.Play();
			MainMenu.SettingsChanged += UpdateMusicVolume;
			screenSpace.Zoom = 1 / window.ViewportPixelSize.AspectRatio;
			window.ViewportSizeChanged += SizeChanged;
			SetViewportBackground("Background");
		}

		private readonly MainMenu menu;
		private readonly Music soundTrack;
		private readonly Camera2DScreenSpace screenSpace;

		//ncrunch: no coverage start
		private void SizeChanged(Size size)
		{
			screenSpace.Zoom = (size.AspectRatio > 1) ? 1 / size.AspectRatio : size.AspectRatio;
		}

		private void UpdateMusicVolume()
		{
			soundTrack.Stop();
			soundTrack.Play();
		}
		//ncrunch: no coverage end

		private void InitGame()
		{
			Show();
			if (menu != null)
				menu.Hide();
			if (restartCommand != null && restartCommand.IsActive)
				restartCommand.Dispose(); //ncrunch: no coverage
			if (backToMenuCommand != null && backToMenuCommand.IsActive)
				backToMenuCommand.Dispose(); //ncrunch: no coverage
			if (gameOverMessage != null)
				gameOverMessage.Dispose(); //ncrunch: no coverage
			score = new Score();
			currentLevel = new Level(score);
			ball = new BallInLevel(new Paddle(), currentLevel);
			new UI(window, this);
			//ncrunch: no coverage start
			score.GameOver += () =>
			{
				RemoveOldObjects();
				gameOverMessage = new FontText(Font.Default, "That's it.\nGame Over!\n\nPress \"Q\" to " +
					"go back to the Main Menu.", Rectangle.One);
				restartCommand = new Command(InitGame).Add(new KeyTrigger(Key.Space)).
					Add(new MouseButtonTrigger()).Add(new TouchTapTrigger());
				backToMenuCommand = new Command(BackToMainMenu).Add(new KeyTrigger(Key.Q));
			};
			//ncrunch: no coverage end
			Score = score;
		}

		private Level currentLevel;

		//ncrunch: no coverage start
		private void RemoveOldObjects()
		{
			ball.Dispose();
			currentLevel.Dispose();
		}

		private BallInLevel ball;
		private Score score;
		private readonly Window window;
		private Command restartCommand;
		private Command backToMenuCommand;
		private FontText gameOverMessage;

		public Score Score { get; private set; }

		private void BackToMainMenu()
		{
			if (gameOverMessage != null)
				gameOverMessage.Dispose();
			if (backToMenuCommand != null)
				backToMenuCommand.Dispose();
			if (restartCommand != null)
				restartCommand.Dispose();
			Hide();
			menu.Show();
		}
	}
}