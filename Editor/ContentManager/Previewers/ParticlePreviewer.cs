using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering3D.Particles;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class ParticlePreviewer : ContentPreview
	{
		protected override void Init() {}

		protected override void Preview(string contentName)
		{
			var particleEmitterData = ContentLoader.Load<ParticleEmitterData>(contentName);
			currentDisplayParticle2D = new ParticleEmitter(particleEmitterData,
				new Vector3D(0.5f, 0.5f, 0));
		}

		public ParticleEmitter currentDisplayParticle2D;
	}
}