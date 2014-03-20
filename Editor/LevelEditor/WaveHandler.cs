using DeltaEngine.GameLogic;

namespace DeltaEngine.Editor.LevelEditor
{
	public class WaveHandler
	{
		public WaveHandler(LevelEditorViewModel viewModel)
		{
			this.viewModel = viewModel;
		}

		private readonly LevelEditorViewModel viewModel;

		public void LoadWaves()
		{
			viewModel.WaveList.Clear();
			viewModel.SpawnTypeList = "";
			viewModel.WaveName = "";
			viewModel.SpawnInterval = 0;
			viewModel.WaitTime = 0;
			viewModel.MaxTime = 0;
			viewModel.IsAddWaveEnabled = !string.IsNullOrEmpty(viewModel.SpawnTypeList);
			viewModel.IsRemoveWaveEnabled = false;
			foreach (var wave in viewModel.Level.Waves)
			{
				viewModel.WaveList.Add(wave);
				viewModel.WaveNameList.Add(wave.ShortName);
			}
		}

		public void SetWaveProperties()
		{
			var foundWave = FindWaveInList();
			if (foundWave != null)
				viewModel.SelectedWave = foundWave;
		}

		private Wave FindWaveInList()
		{
			bool hit = false;
			int i;
			for (i = 0; !hit && i < viewModel.Level.Waves.Count; i++)
				if (viewModel.Level.Waves[i].ShortName == viewModel.WaveName)
					hit = true;
			return hit ? viewModel.Level.Waves[--i] : null;
		}

		public void AddWave(float waitingTime, float spawningInterval, float maximumTime,
			string thingsToSpawn, string shortName, int maxSpawnItems)
		{
			if (string.IsNullOrEmpty(shortName))
				shortName = "Wave " + (viewModel.Level.Waves.Count + 1);
			var newWave = new Wave(waitingTime, spawningInterval, thingsToSpawn, shortName, maximumTime, maxSpawnItems);
			viewModel.Level.AddWave(newWave);
			if (string.IsNullOrEmpty(thingsToSpawn))
				return;
			viewModel.WaveList.Add(newWave);
			if (!viewModel.WaveNameList.Contains(viewModel.WaveName))
				viewModel.WaveNameList.Add(newWave.ShortName);
		}

		public void RemoveSelectedWave()
		{
			if (viewModel.WaveList.Count <= 0 || viewModel.SelectedWave == null)
				return;
			viewModel.Level.Waves.Remove(viewModel.SelectedWave);
			viewModel.WaveNameList.Remove(viewModel.SelectedWave.ShortName);
			viewModel.WaveList.Remove(viewModel.SelectedWave);
			viewModel.IsRemoveWaveEnabled = viewModel.WaveList.Count > 0;
		}
	}
}