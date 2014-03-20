using System.Collections.Generic;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Rendering3D.Particles
{
	public class Particle2DRenderer : DrawBehavior
	{
		public Particle2DRenderer(BatchRenderer2D renderer)
		{
			this.renderer = renderer;
		}

		private readonly BatchRenderer2D renderer;

		public void Draw(List<DrawableEntity> visibleEntities)
		{
			foreach (ParticleEmitter entity in visibleEntities)
				if (entity.NumberOfActiveParticles > 0)
					AddVerticesToBatch(entity);
		}

		private void AddVerticesToBatch(ParticleEmitter emitter)
		{
			particles = emitter.GetInterpolatedArray<Particle>(emitter.NumberOfActiveParticles);
			var length = particles.Length;
			for (int index = 0; index < length; index++)
				AddIndicesAndVerticesForParticle(index);
		}

		private Particle[] particles;

		private void AddIndicesAndVerticesForParticle(int index)
		{
			var particle = particles[index];
			if (!particle.IsActive)
				return;
			var material = particle.Material;
			var batch = (Batch2D)renderer.FindOrCreateBatch(material, material.DiffuseMap.BlendMode);
			batch.AddIndices();
			batch.verticesColorUV[batch.verticesIndex++] = particle.GetTopLeftVertex();
			batch.verticesColorUV[batch.verticesIndex++] = particle.GetBottomLeftVertex();
			batch.verticesColorUV[batch.verticesIndex++] = particle.GetBottomRightVertex();
			batch.verticesColorUV[batch.verticesIndex++] = particle.GetTopRightVertex();
		}
	}
}
