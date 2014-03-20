using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering3D.Particles;

namespace DeltaEngine.Editor.LevelEditor
{
	public class ParticleLevelObject : DrawableEntity
	{
		public ParticleLevelObject(Vector2D position, string selectedLevelObject)
		{
			this.position = position;
			this.selectedLevelObject = selectedLevelObject;
			Create();
		}

		private Vector2D position;
		private readonly string selectedLevelObject;
		public string Name { get; private set; }

		private void Create()
		{
			Name = selectedLevelObject;
			levelObject =
				new ParticleEmitter(ContentLoader.Load<ParticleEmitterData>(selectedLevelObject),
					new Vector3D(position.X, position.Y, 0.2f));
		}

		private ParticleEmitter levelObject;

		public override void Dispose()
		{
			levelObject.Dispose();
			base.Dispose();
		}
	}
}