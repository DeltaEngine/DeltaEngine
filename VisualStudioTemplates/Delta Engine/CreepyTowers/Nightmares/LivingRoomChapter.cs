using CreepyTowers.Content;
using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Levels;
using DeltaEngine.Datatypes;
using DeltaEngine.GameLogic;
using Nightmare1 = CreepyTowers.GUI.Nightmare1;

namespace $safeprojectname$.Nightmares
{
	public class LivingRoomChapter : Chapter
	{
		public LivingRoomChapter()
			: base(LevelMaps.N1C7LivingRoomLevelInfo)
		{
			AddWaves();
			Current = this;
		}

		protected override void UpdateCamera()
		{
			GameLevel.Camera.Position = new Vector3D(9.0f, -10.0f, GameCamera.CameraHeight);
			GameLevel.Camera.MinZoom = 1 / 40.0f;
			GameLevel.Camera.MaxZoom = 1 / 10.0f;
		}

		protected override void InitializePlayer()
		{
			var player = Player.Current;
			player.Gold = 4000;
			player.MaxLives = 5;
		}

		protected override void InitializeLevel()
		{
			base.InitializeLevel();
			Level.Current.MapData = LivingRoomLevelInfo.MapInfo(GameLevel.Size);
			GameLevel.InitializeGameGraph();
		}

		private void AddWaves()
		{
			GameLevel.AddCreepWaveToWaveGenerator(new CreepWave(2.0f, 3.0f, "Wood"));
			GameLevel.AddCreepWaveToWaveGenerator(new CreepWave(2.0f, 3.0f, "Plastic"));
			GameLevel.AddCreepWaveToWaveGenerator(new CreepWave(2.0f, 3.0f, "Iron"));
		}

		protected override void Completed()
		{
			Nightmare1.ChapterToUnlock = 0;
		}
	}
}