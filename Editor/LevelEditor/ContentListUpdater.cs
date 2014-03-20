using DeltaEngine.Content;
using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.LevelEditor
{
	public class ContentListUpdater
	{
		public ContentListUpdater(LevelEditorViewModel viewModel, Service service)
		{
			this.viewModel = viewModel;
			this.service = service;
		}

		private readonly LevelEditorViewModel viewModel;
		private readonly Service service;

		public void GetBackgroundImages()
		{
			viewModel.BackgroundImages.Clear();
			foreach (var image in service.GetAllContentNamesByType(ContentType.Image))
				if (!image.EndsWith("Font"))
					viewModel.BackgroundImages.Add(image);
		}

		public void GetLevelObjects(bool is3D)
		{
			viewModel.LevelObjects.Clear();
			viewModel.LevelObjects.Add("None");
			if (is3D)
			{
				GetModels();
				GetParticles();
			}
			GetMaterials();
		}

		private void GetParticles()
		{
			foreach (var particle in service.GetAllContentNamesByType(ContentType.ParticleEmitter))
				viewModel.LevelObjects.Add(particle);
		}

		private void GetModels()
		{
			viewModel.ModelList.Clear();
			foreach (var model in service.GetAllContentNamesByType(ContentType.Model))
			{
				viewModel.ModelList.Add(model);
				viewModel.LevelObjects.Add(model);
			}
		}

		private void GetMaterials()
		{
			foreach (var material in service.GetAllContentNamesByType(ContentType.Material))
				viewModel.LevelObjects.Add(material);
		}

		public void GetLevels()
		{
			viewModel.LevelList.Clear();
			foreach (var level in service.GetAllContentNamesByType(ContentType.Level))
				viewModel.LevelList.Add(level);
		}
	}
}