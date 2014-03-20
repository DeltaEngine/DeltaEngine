using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.ScreenSpaces;
using DeltaNinja.Entities;
using DeltaNinja.Pages;
using DeltaNinja.Support;
using DeltaNinja.UI;

namespace $safeprojectname$
{
	class Game : Entity
	{
		public Game(ScreenSpace screen, Window window)
		{
			this.screen = screen;
			this.window = window;
			window.Title = "Delta Ninja - A Delta Engine sample";
			window.ViewportPixelSize = new Size(1280, 720);
			InitBackground();
			InitInput();
			InitPages();
			home.Show();
		}

		private readonly ScreenSpace screen;
		private readonly Window window;
		private HomePage home;
		private Match playing;
		private GameOverPage gameOver;
		
		private void InitBackground()
		{
			new Background(screen);
		}

		private void InitInput()
		{
			new Command(Exit).Add(new KeyTrigger(Key.Escape));
			new Command(SwitchWindowMode).Add(new KeyTrigger(Key.F));		
		}

		private void SwitchWindowMode()
		{
			if (window.IsFullscreen)
				window.SetWindowed();
			else
				window.SetFullscreen(new Size(1920, 1080));
		}

		private void InitPages()
		{
			home = new HomePage(screen);			
			home.ButtonClicked += OnButtonClicked;
			playing = new Match(screen, new NumberFactory(), new LogoFactory(screen));
			playing.GameEnded += (sender, e) => ShowGameOver(e);
			gameOver = new GameOverPage(screen);
			gameOver.Hide();
			gameOver.ButtonClicked += OnButtonClicked;
		}

		protected void OnButtonClicked(MenuButton code)
		{
			switch (code)
			{
				case MenuButton.Home:
					ShowHome();
					break;
				case MenuButton.NewGame:
				case MenuButton.Retry:
					StartNewGame();
					break;
				case MenuButton.Exit:
					Exit();
					break;
			}
		}

		private void StartNewGame()
		{
			home.Hide();
			gameOver.Hide();
			playing.Start();
		}

		private void ShowHome()
		{
			playing.HideScore();
			gameOver.Hide();
			home.Show();
		}

		private void ShowGameOver(GameOverEventArgs e)
		{
			if (e == null)
				ShowHome();
			else
				gameOver.Show();
		}

		private void Exit()
		{
			window.Dispose();
		}
	}
}