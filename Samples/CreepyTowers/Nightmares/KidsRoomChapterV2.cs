using CreepyTowers.Content;
using CreepyTowers.Enemy.Creeps;
using CreepyTowers.GUI;
using CreepyTowers.Levels;
using DeltaEngine.Entities;
using Nightmare1 = CreepyTowers.GUI.Nightmare1;

namespace CreepyTowers.Nightmares
{
	public class KidsRoomChapterV2 : Chapter
	{
		public KidsRoomChapterV2()
			: base(LevelMaps.N1C2ChildsRoomLevelInfo)
		{
			ShowAvatarVillainInteraction();
			AddWaves();
			Current = this;
		}

		private void ShowAvatarVillainInteraction()
		{
			avatarSuperVillainScene =
				(AvatarVillianCinematics)MenuController.Current.GetMenu(GameMenus.SceneAvatarSuperVillain);
			MenuController.Current.ShowMenu(GameMenus.SceneAvatarSuperVillain);
			Time.IsPaused = true;
		}

		private AvatarVillianCinematics avatarSuperVillainScene;

		protected override void UpdateCamera()
		{
			GameLevel.Camera.ResetPositionToDefault();
			GameLevel.Camera.MinZoom = 1 / 25.0f;
			GameLevel.Camera.MaxZoom = 1 / 10.0f;
		}

		protected override void InitializePlayer()
		{
			var player = Player.Current;
			player.Gold = 4000;
			player.MaxLives = 5;
		}

		private void AddWaves()
		{
			GameLevel.AddCreepWaveToWaveGenerator(new CreepWave(2.0f, 3.0f, "Sand"));
			GameLevel.AddCreepWaveToWaveGenerator(new CreepWave(2.0f, 3.0f, "Glass"));
		}

		protected override void Completed()
		{
			Nightmare1.ChapterToUnlock = 3;
		}

		public override void Restart()
		{
			base.Restart();
			avatarSuperVillainScene.MessageCount = 0;
			ShowAvatarVillainInteraction();
		}

		public override void Dispose() {}
	}
}