using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering3D.Particles
{
	public class Particle3DRenderer : DrawBehavior
	{
		public Particle3DRenderer(BatchRenderer3D renderer, Device device)
		{
			this.renderer = renderer;
			this.device = device;
		}

		private readonly BatchRenderer3D renderer;
		private readonly Device device;

		public void Draw(List<DrawableEntity> visibleEntities)
		{
			inverseView = device.CameraInvertedViewMatrix;
			foreach (ParticleEmitter entity in visibleEntities)
				if (entity.NumberOfActiveParticles > 0)
					AddVerticesToBatch(entity);
		}

		private void AddVerticesToBatch(ParticleEmitter emitter)
		{
			var particles = emitter.GetInterpolatedArray<Particle>(emitter.NumberOfActiveParticles);
			var material = emitter.EmitterData.ParticleMaterial;
			var length = particles.Length;
			for (int i = 0; i < length; i++)
			{
				if (!particles[i].IsActive)
					continue; //ncrunch: no coverage
				var batch = (Batch3D)renderer.FindOrCreateBatch(material, material.DiffuseMap.BlendMode);
				UpdateTransformMatrix(particles[i], emitter.EmitterData.BillboardMode);
				var particleVertices = particles[i].GetVertices(particles[i].Size, particles[i].Color);
				for (int v = 0; v < particleVertices.Length; v++)
					particleVertices[v].Position = lastParticleTransform * particleVertices[v].Position;
				batch.AddIndices();
				batch.verticesColorUV[batch.verticesIndex++] = particleVertices[0];
				batch.verticesColorUV[batch.verticesIndex++] = particleVertices[1];
				batch.verticesColorUV[batch.verticesIndex++] = particleVertices[2];
				batch.verticesColorUV[batch.verticesIndex++] = particleVertices[3];
			}
		}

		private Matrix inverseView;

		private Vector3D lastCameraUp;
		private Vector3D lastFacingDirection;


		private Matrix lastParticleTransform;

		private void UpdateTransformMatrix(Particle particle, BillboardMode mode)
		{
			lastFacingDirection = inverseView.Translation - particle.Position;
			lastCameraUp = inverseView.Up;
			AlignBillboardDirection(mode);
			lastCameraUp.Normalize();
			lastFacingDirection.Normalize();
			Vector3D right = Vector3D.Cross(lastCameraUp, lastFacingDirection);
			Vector3D up = Vector3D.Cross(lastFacingDirection, right);
			lastParticleTransform = Matrix.Identity;
			lastParticleTransform.Right = -right;
			lastParticleTransform.Up = lastFacingDirection;
			lastParticleTransform.Forward = up;
			lastParticleTransform *=
				Matrix.CreateRotationZYX(lastParticleTransform.Forward.X, lastParticleTransform.Forward.Y,
				lastParticleTransform.Forward.Z);
			lastParticleTransform.Translation = particle.Position;
		}

		//ncrunch: no coverage start
		private void AlignBillboardDirection(BillboardMode mode)
		{
			if ((mode & BillboardMode.UpAxis) != 0)
				AlignToZAxis();
			else if ((mode & BillboardMode.FrontAxis) != 0)
				AlignToYAxis();
			else if ((mode & BillboardMode.RightAxis) != 0)
				AlignToXAxis();
			else if ((mode & BillboardMode.Ground) != 0)
				AlignToXYPlane();
		}

		private void AlignToZAxis()
		{
			lastCameraUp = Vector3D.UnitZ;
			lastFacingDirection.Z = 0;
		}

		private void AlignToYAxis()
		{
			lastCameraUp = Vector3D.UnitY;
			lastFacingDirection.Y = 0;
		}

		private void AlignToXAxis()
		{
			lastCameraUp = Vector3D.UnitX;
			lastFacingDirection.X = 0.0f;
		}

		private void AlignToXYPlane()
		{
			lastCameraUp = -Vector3D.UnitY;
			lastFacingDirection = Vector3D.UnitZ;
		}
	}
}