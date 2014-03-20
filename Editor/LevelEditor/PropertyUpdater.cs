using DeltaEngine.Datatypes;
using DeltaEngine.GameLogic;

namespace DeltaEngine.Editor.LevelEditor
{
	public class PropertyUpdater
	{
		public PropertyUpdater(LevelEditorViewModel viewModel)
		{
			this.viewModel = viewModel;
		}

		private readonly LevelEditorViewModel viewModel;

		public void ResetLevelEditor(bool is3D)
		{
			ResetLevelEditorPane();
			ClearLevel();
			viewModel.contentListUpdater.GetBackgroundImages();
			viewModel.contentListUpdater.GetLevelObjects(is3D);
			viewModel.contentListUpdater.GetLevels();
			viewModel.ContentName = "MyNewLevel";
		}

		private void ResetLevelEditorPane()
		{
			viewModel.Level.Size = new Size(12);
			viewModel.CustomSize = viewModel.Level.Size.ToString();
			viewModel.SelectedTileType = LevelTileType.Nothing;
			viewModel.ClearWaves();
		}

		public void ClearWaves()
		{
			viewModel.SpawnTypeList = "";
			viewModel.WaveName = "";
			viewModel.SpawnInterval = 0;
			viewModel.WaitTime = 0;
			viewModel.MaxTime = 0;
			viewModel.WaveList.Clear();
			viewModel.WaveNameList.Clear();
			viewModel.Level.Waves.Clear();
			viewModel.IsAddWaveEnabled = viewModel.SpawnTypeList != "";
			viewModel.IsRemoveWaveEnabled = false;
		}

		public void ClearLevel()
		{
			for (int i = 0; i < viewModel.Level.MapData.Length; i++)
				viewModel.Level.MapData[i] = LevelTileType.Nothing;
			viewModel.renderer.RecreateTiles();
		}

		public void UpdateLists(bool is3D)
		{
			var currentLevelName = viewModel.ContentName;
			viewModel.contentListUpdater.GetBackgroundImages();
			viewModel.contentListUpdater.GetLevelObjects(is3D);
			viewModel.contentListUpdater.GetLevels();
			viewModel.ContentName = currentLevelName;
		}
	}
}