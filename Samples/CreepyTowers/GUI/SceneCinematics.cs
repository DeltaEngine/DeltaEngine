using CreepyTowers.Content;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;

namespace CreepyTowers.GUI
{
	public abstract class SceneCinematics : Menu
	{
		protected SceneCinematics()
		{
			MessageCount = 0;
			CreateScene();
			new Command(GameCommands.MouseLeftButtonClick.ToString(), () =>
			{
				if (IsShown)
					DisplayNextMessage();
			});
		}

		protected override sealed void CreateScene()
		{
			Scene = ContentLoader.Load<Scene>("SceneCinematics");
			InitializeSceneControls();
			Hide();
		}

		private void InitializeSceneControls()
		{
			AvatarImage = (Picture)GetSceneControl(Cinematics.AvatarImage.ToString());
			AvatarText = (Label)GetSceneControl(Cinematics.AvatarTalk.ToString());
			SuperVillainImage = (Picture)GetSceneControl(Cinematics.SuperVillainImage.ToString());
			SuperVillainText = (Label)GetSceneControl(Cinematics.SuperVillainTalk.ToString());
			InstructionsText = (Label)GetSceneControl(Cinematics.Instructions.ToString());
		}

		public Picture AvatarImage { get; protected set; }
		public Picture SuperVillainImage { get; protected set; }
		public Label AvatarText { get; protected set; }
		public Label SuperVillainText { get; protected set; }
		public Label InstructionsText { get; protected set; }
		public string[] Messages { get; protected set; }
		public int MessageCount { get; set; }

		public virtual void UpdateCinematic()
		{
			SuperVillainText.Text = "";
			AvatarText.Text = "";
		}

		public abstract void DisplayNextMessage();
	}
}