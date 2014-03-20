using CreepyTowers.Content;
using DeltaEngine.Content;
using DeltaEngine.Multimedia;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;

namespace $safeprojectname$.GUI
{
	public class Settings : Menu
	{
		public Settings()
		{
			CreateScene();
		}

		protected override sealed void CreateScene()
		{
			Scene = ContentLoader.Load<Scene>(GameMenus.SceneSettingsMenu.ToString());
			Hide();
			AttachBackButtonEvent();
			SetMusicVolume();
			SetSoundVolume();
		}

		private void AttachBackButtonEvent()
		{
			backButton = (InteractiveButton)GetSceneControl(Content.Settings.ButtonBack.ToString());
			backButton.Clicked += ShowMainMenu;
		}

		private InteractiveButton backButton;

		private static void ShowMainMenu()
		{
			PlayClickedSound();
			MenuController.Current.HideMenu(GameMenus.SceneSettingsMenu);
			MenuController.Current.ShowMenu(GameMenus.SceneMainMenu);
		}

		private static void PlayClickedSound()
		{
			var sound = ContentLoader.Load<Sound>(GameSounds.MenuButtonClick.ToString());
			sound.Play();
		}

		private void SetMusicVolume()
		{
			musicSlider = (Slider)GetSceneControl(Content.Settings.MusicVolumeSlider.ToString());
			InitializeMusicSlider();
			SetMusicSliderValue();
			musicSlider.ValueChanged += value =>
			{
				var musicVol = value / (float)musicSlider.MaxValue;
				Game.SoundDevice.MusicVolume = musicVol;
				SetMusicSliderValue();
			};
		}

		private void InitializeMusicSlider()
		{
			musicSlider.MinValue = 0;
			musicSlider.MaxValue = 100;
		}

		private void SetMusicSliderValue()
		{
			musicSlider.Value =
				(int)(DeltaEngine.Core.Settings.Current.MusicVolume * musicSlider.MaxValue);
		}

		private Slider musicSlider;

		private void SetSoundVolume()
		{
			soundSlider = (Slider)GetSceneControl(Content.Settings.SoundVolumeSlider.ToString());
			InitializeSoundSliderValues();
			SetSoundSliderValue();
			soundSlider.ValueChanged += value =>
			{
				DeltaEngine.Core.Settings.Current.SoundVolume = value / (float)soundSlider.MaxValue;
				PlayDummySound();
				SetSoundSliderValue();
			};
		}

		private void InitializeSoundSliderValues()
		{
			soundSlider.MinValue = 0;
			soundSlider.MaxValue = 10;
		}

		private void SetSoundSliderValue()
		{
			soundSlider.Value =
				(int)(DeltaEngine.Core.Settings.Current.SoundVolume * soundSlider.MaxValue);
		}

		private static void PlayDummySound()
		{
			var sound = ContentLoader.Load<Sound>(GameSounds.MenuButtonClick.ToString());
			if (!sound.IsAnyInstancePlaying)
				sound.Play();
		}

		private Slider soundSlider;

		public override void Reset() {}
	}
}