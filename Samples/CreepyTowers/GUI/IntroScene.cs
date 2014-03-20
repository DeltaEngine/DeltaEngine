using System.Collections.Generic;
using CreepyTowers.Content;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering2D;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;

namespace CreepyTowers.GUI
{
	public class IntroScene : Menu
	{
		public IntroScene()
		{
			if (WasAlreadyDisplayed)
				return;
			currentPanel = 0;
			comicStripMaterials = new List<Material>(5);
			InitializeMaterialsForComicStrips();
			CreateScene();
		}

		public bool WasAlreadyDisplayed { get; set; }

		private int currentPanel;
		private readonly List<Material> comicStripMaterials;
		private const float FadeDuration = 1.0f;

		private void InitializeMaterialsForComicStrips()
		{
			comicStripMaterials.Add(
				ContentLoader.Load<Material>(Content.IntroScene.ComicStripsStoryboardPanel1Mat.ToString()));
			comicStripMaterials.Add(
				ContentLoader.Load<Material>(Content.IntroScene.ComicStripsStoryboardPanel2Mat.ToString()));
			comicStripMaterials.Add(
				ContentLoader.Load<Material>(Content.IntroScene.ComicStripsStoryboardPanel3Mat.ToString()));
			comicStripMaterials.Add(
				ContentLoader.Load<Material>(Content.IntroScene.ComicStripsStoryboardPanel4Mat.ToString()));
			comicStripMaterials.Add(
				ContentLoader.Load<Material>(Content.IntroScene.ComicStripsStoryboardPanel5Mat.ToString()));
		}

		protected override sealed void CreateScene()
		{
			Scene = ContentLoader.Load<Scene>(GameMenus.SceneIntro.ToString());
			Hide();
			SetupComicStripImage();
			SetupForwardFlipButtonAndAttachEvent();
			SetupBackFlipButtonAndAttachEvent();
			SetupSkipButtonAndAttachEvent();
		}

		private void SetupComicStripImage()
		{
			comicStripImage = (Sprite)GetSceneControl(Content.IntroScene.StoryboardPanel.ToString());
			comicStripImage.Add(new FadeEffect.TransitionData { FadeActive = false });
			comicStripImage.Start<FadeEffect>();
		}

		private Sprite comicStripImage;

		private void SetupForwardFlipButtonAndAttachEvent()
		{
			forwardButton =
				(InteractiveButton)GetSceneControl(Content.IntroScene.ButtonIntroFlipRight.ToString());
			forwardButton.Clicked += MoveIntroSceneForward;
		}

		private InteractiveButton forwardButton;

		private void MoveIntroSceneForward()
		{
			PlayClickedSound();
			if (currentPanel == comicStripMaterials.Count - 1)
			{
				forwardButton.IsVisible = false;
				LoadAvatarSelectionMenu();
				return;
			}
			currentPanel++;
			FadeOutAndBack();
			ToggleButtonStatesMovingForward();
		}

		private static void PlayClickedSound()
		{
			var clickSound = ContentLoader.Load<Sound>(GameSounds.MenuPageFlip.ToString());
			clickSound.Play();
		}

		private void FadeOutAndBack()
		{
			var data = new FadeEffect.TransitionData
			{
				Duration = FadeDuration,
				Colour = new RangeGraph<Color>(Color.White, Color.TransparentBlack),
				ReplaceMaterialReverting = comicStripMaterials[currentPanel],
				FadeActive = true
			};
			comicStripImage.Set(data);
		}

		private void ToggleButtonStatesMovingForward()
		{
			if (currentPanel > 0 && currentPanel < comicStripMaterials.Count - 1)
				backButton.IsVisible = true;
		}

		private void SetupBackFlipButtonAndAttachEvent()
		{
			backButton =
				(InteractiveButton)GetSceneControl(Content.IntroScene.ButtonIntroFlipLeft.ToString());
			backButton.IsVisible = false;
			backButton.Clicked += MoveIntroSceneBackward;
		}

		private InteractiveButton backButton;

		private void MoveIntroSceneBackward()
		{
			PlayClickedSound();
			if (currentPanel < 1)
				return;
			currentPanel--;
			FadeOutAndBack();
			ToggleButtonStatesMovingBackwards();
		}

		private void ToggleButtonStatesMovingBackwards()
		{
			if (currentPanel == comicStripMaterials.Count - 2)
			{
				forwardButton.IsVisible = false;
				backButton.IsVisible = true;
			}
			else if (currentPanel == 0)
			{
				forwardButton.IsVisible = true;
				backButton.IsVisible = false;
			}
			forwardButton.IsVisible = true;
		}

		private void SetupSkipButtonAndAttachEvent()
		{
			skipButton =
				(InteractiveButton)GetSceneControl(Content.IntroScene.ButtonIntroSkip.ToString());
			skipButton.Clicked += LoadAvatarSelectionMenu;
		}

		private InteractiveButton skipButton;

		private static void LoadAvatarSelectionMenu()
		{
			PlaySkipButtonClickSound();
			MenuController.Current.HideMenu(GameMenus.SceneIntro);
			MenuController.Current.ShowMenu(GameMenus.SceneAvatarSelection);
		}

		private static void PlaySkipButtonClickSound()
		{
			var clickSound = ContentLoader.Load<Sound>(GameSounds.MenuButtonClick.ToString());
			if (clickSound != null)
				clickSound.Play();
		}

		public override void Reset()
		{
			if (WasAlreadyDisplayed)
				return;

			WasAlreadyDisplayed = true;
		}
	}
}