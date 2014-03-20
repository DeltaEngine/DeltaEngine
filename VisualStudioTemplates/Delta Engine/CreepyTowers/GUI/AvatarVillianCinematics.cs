using CreepyTowers.Content;
using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Levels;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.GameLogic;

namespace $safeprojectname$.GUI
{
	public class AvatarVillianCinematics : SceneCinematics
	{
		public AvatarVillianCinematics()
		{
			sceneDialoguesXml =
				ContentLoader.Load<DialoguesXml>(Xml.N1C3AvatarSuperVillainDialogues.ToString());
			avatarOriginalSize = AvatarImage.Size;
			superVillainOriginalSize = SuperVillainImage.Size;
		}

		private readonly DialoguesXml sceneDialoguesXml;
		private readonly Size avatarOriginalSize;
		private readonly Size superVillainOriginalSize;

		public override void DisplayNextMessage()
		{
			if (sceneDialoguesXml.messages.Count == 0)
			{
				MenuController.Current.ShowMenu(GameMenus.SceneGameHud);
				return;
			}

			if (MessageCount == sceneDialoguesXml.messages.Count)
			{
				ShowLevel();
				return;
			}

			UpdateDialogue();
			MessageCount++;
		}

		private void UpdateDialogue()
		{
			var dialogue = sceneDialoguesXml.GetMessage(MessageCount);
			switch (dialogue.character)
			{
			case "Avatar":
				HighlightAvatarAndUpdateText(dialogue.text);
				break;
			case "SuperVillain":
				HighlightSuperVillainAndUpdateText(dialogue.text);
				break;
			}
		}

		private void HighlightAvatarAndUpdateText(string message)
		{
			SuperVillainImage.DrawArea = Rectangle.FromCenter(SuperVillainImage.Center,
				superVillainOriginalSize);
			SuperVillainText.Text = "";
			AvatarImage.DrawArea = Rectangle.FromCenter(AvatarImage.Center,
				avatarOriginalSize * SizeUpFactor);
			AvatarText.Text = message;
		}

		private const float SizeUpFactor = 1.30f;

		private void HighlightSuperVillainAndUpdateText(string message)
		{
			AvatarImage.DrawArea = Rectangle.FromCenter(AvatarImage.Center,
				avatarOriginalSize);
			AvatarText.Text = "";
			SuperVillainImage.DrawArea = Rectangle.FromCenter(SuperVillainImage.Center,
				superVillainOriginalSize * SizeUpFactor);
			SuperVillainText.Text = message;
		}

		private static void ShowLevel()
		{
			HideCurrentSceneAndShowGame();
			var level = (GameLevel)Level.Current;
			level.WaveGenerator.Start<WaveGenerator.WaveCreation>();
		}

		private static void HideCurrentSceneAndShowGame()
		{
			MenuController.Current.HideMenu(GameMenus.SceneAvatarSuperVillain);
			MenuController.Current.ShowMenu(GameMenus.SceneGameHud);
			Time.IsPaused = false;
		}

		public override void Reset()
		{
			AvatarText.Text = "";
			SuperVillainText.Text = "";
			DisplayNextMessage();
		}
	}
}