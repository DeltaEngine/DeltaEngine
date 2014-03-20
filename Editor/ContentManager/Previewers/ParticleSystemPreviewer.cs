using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering3D.Particles;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class ParticleSystemPreviewer : ContentPreview
	{
		protected override void Init() {}

		protected override void Preview(string contentName)
		{
			var particleSystemData = ContentLoader.Load<ParticleSystemData>(contentName);
			var system = new ParticleSystem(particleSystemData);
			system.Position = new Vector3D(0.5f, 0.5f, 0);
		}
	}
}