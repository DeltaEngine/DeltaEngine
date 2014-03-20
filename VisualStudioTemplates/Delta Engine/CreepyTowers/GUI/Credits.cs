using CreepyTowers.Content;
using DeltaEngine.Content;
using DeltaEngine.Multimedia;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;

namespace $safeprojectname$.GUI
{
	public class Credits : Menu
	{
		public Credits()
		{
			CreateScene();
		}

		protected override sealed void CreateScene()
		{
			Scene = ContentLoader.Load<Scene>(GameMenus.SceneCredits.ToString());
			Hide();
			AttachBackButtonEvent();
		}

		private void AttachBackButtonEvent()
		{
			backButton =
				(InteractiveButton)GetSceneControl(Content.Credits.ButtonBack.ToString());
			backButton.Clicked += ShowMainMenu;
		}

		private InteractiveButton backButton;

		private static void ShowMainMenu()
		{
			PlayClickedSound();
			MenuController.Current.HideMenu(GameMenus.SceneCredits);
			MenuController.Current.ShowMenu(GameMenus.SceneMainMenu);
		}

		private static void PlayClickedSound()
		{
			var sound = ContentLoader.Load<Sound>(GameSounds.MenuButtonClick.ToString());
			sound.Play();
		}

		public override void Reset() {}
	}
}