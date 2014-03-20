using CreepyTowers.Content;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Multimedia;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;

namespace $safeprojectname$.GUI
{
	public class MainMenu : Menu
	{
		public MainMenu(Window window)
		{
			this.window = window;
			CreateScene();
		}

		private readonly Window window;

		private void ExitGame()
		{
			PlayClickedSound();
			Scene.Dispose();
			window.CloseAfterFrame();
		}

		protected override sealed void CreateScene()
		{
			Scene = ContentLoader.Load<Scene>(GameMenus.SceneMainMenu.ToString());
			Hide();
			LoadMusic();
			SetupButtonsAndAttachButtonEvents();
		}

		private void LoadMusic()
		{
			if (!ContentLoader.Exists(GameMusic.GameMusic.ToString(), ContentType.Music))
				return;
			music = ContentLoader.Load<Music>(GameMusic.GameMusic.ToString());
			music.Loop = true;
		}

		private Music music;

		private void SetupButtonsAndAttachButtonEvents()
		{
			AttachPlayButtonEvent();
			AttachSettingsButtonEvent();
			AttachCreditsButtonEvent();
			AttachQuitButtonEvent();
		}

		private void AttachPlayButtonEvent()
		{
			playButton = (InteractiveButton)GetSceneControl(Content.MainMenu.ButtonPlay.ToString());
			if (playButton != null)
				playButton.Clicked += SelectRelevantMenu;
		}

		private static void SelectRelevantMenu()
		{
			var introScene = (IntroScene)MenuController.Current.GetMenu(GameMenus.SceneIntro);
			ToggleMenus(introScene.WasAlreadyDisplayed
				? GameMenus.SceneAvatarSelection : GameMenus.SceneIntro);
		}

		private InteractiveButton playButton;

		private static void ToggleMenus(GameMenus menuToShow)
		{
			PlayClickedSound();
			MenuController.Current.HideMenu(GameMenus.SceneMainMenu);
			MenuController.Current.ShowMenu(menuToShow);
		}

		private static void PlayClickedSound()
		{
			var clickSound = ContentLoader.Load<Sound>(GameSounds.MenuButtonClick.ToString());
			if (clickSound != null)
				clickSound.Play();
		}

		private void AttachSettingsButtonEvent()
		{
			settingsButton =
				(InteractiveButton)GetSceneControl(Content.MainMenu.ButtonSettings.ToString());
			if (settingsButton != null)
				settingsButton.Clicked += DisplaySettings;
		}

		private InteractiveButton settingsButton;

		private static void DisplaySettings()
		{
			PlayClickedSound();
			MenuController.Current.HideMenu(GameMenus.SceneMainMenu);
			MenuController.Current.ShowMenu(GameMenus.SceneSettingsMenu);
		}

		private void AttachCreditsButtonEvent()
		{
			creditsButton =
				(InteractiveButton)GetSceneControl(Content.MainMenu.ButtonCredits.ToString());
			if (creditsButton != null)
				creditsButton.Clicked += DisplayCredits;
		}

		private InteractiveButton creditsButton;

		private static void DisplayCredits()
		{
			PlayClickedSound();
			MenuController.Current.HideMenu(GameMenus.SceneMainMenu);
			MenuController.Current.ShowMenu(GameMenus.SceneCredits);
		}

		private void AttachQuitButtonEvent()
		{
			quitButton = (InteractiveButton)GetSceneControl(Content.MainMenu.ButtonQuit.ToString());
			if (quitButton != null)
				quitButton.Clicked += ExitGame;
		}

		private InteractiveButton quitButton;

		public override void Reset()
		{
			if (music == null)
				return;
			if (!music.IsPlaying())
				music.Play();
		}
	}
}