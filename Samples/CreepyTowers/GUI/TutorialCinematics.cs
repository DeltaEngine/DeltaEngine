using CreepyTowers.Content;
using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Levels;
using DeltaEngine.Entities;
using DeltaEngine.GameLogic;

namespace CreepyTowers.GUI
{
	public class TutorialCinematics : SceneCinematics
	{
		public TutorialCinematics()
		{
			Messages = DialoguesXml.KidsRoomMessages();
			UpdateCinematic();
		}

		public override sealed void UpdateCinematic()
		{
			base.UpdateCinematic();
			SuperVillainImage.IsVisible = false;
			SuperVillainText.IsVisible = false;
		}

		public override void DisplayNextMessage()
		{
			if (Messages.Length == 0)
			{
				MenuController.Current.ShowMenu(GameMenus.SceneGameHud);
				return;
			}

			InstructionsText.Text = "";

			if (MessageCount == 2)
			{
				HideTutorialShowGame();
				return;
			}

			if (MessageCount == 4 || MessageCount == Messages.Length)
			{
				StartCreepWaves();
				return;
			}

			UpdateAvatarText();
		}

		private static void HideTutorialShowGame()
		{
			MenuController.Current.HideMenu(GameMenus.SceneTutorial);
			MenuController.Current.ShowMenu(GameMenus.SceneGameHud);
			Time.IsPaused = false;
		}

		private void StartCreepWaves()
		{
			HideTutorialShowGame();
			var level = (GameLevel)Level.Current;
			level.WaveGenerator.Start<WaveGenerator.WaveCreation>();
		}

		public void UpdateAvatarText()
		{
			if (MessageCount >= Messages.Length)
				return;
			AvatarText.Text = Messages[MessageCount];
			MessageCount++;
		}

		public override void Reset()
		{
			UpdateCinematic();
			if (MessageCount > 0)
				return;
			AvatarText.Text = Messages[MessageCount];
			MessageCount++;
		}
	}
}