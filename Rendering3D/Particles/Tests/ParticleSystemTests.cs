using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Rendering3D.Particles.Tests
{
	internal class ParticleSystemTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateParticleSystem()
		{
			particleSystem = new ParticleSystem();
		}

		private ParticleSystem particleSystem;

		[Test, CloseAfterFirstFrame]
		public void NewSystemHasListInitialized()
		{
			Assert.IsNotNull(particleSystem.AttachedEmitters);
		}

		[Test, CloseAfterFirstFrame]
		public void AttachingEmitterSetsPositionAndRotation()
		{
			particleSystem.Position = Vector3D.UnitY;
			particleSystem.Orientation = Quaternion.FromAxisAngle(Vector3D.UnitZ, 45);
			var emitter = CreateAndAttachEmitter(Vector3D.UnitX);
			particleSystem.FireBurstOfAllEmitters();
			particleSystem.FireBurstAtRelativePosition(Vector3D.One);
			particleSystem.FireBurstAtRelativePositionAndRotation(Vector3D.One, 0.5f);
			Assert.AreEqual(particleSystem.Position, emitter.Position);
			Assert.AreEqual(particleSystem.Orientation, emitter.Rotation);
			Assert.IsTrue(particleSystem.IsActive);
			particleSystem.IsActive = false;
			Assert.IsFalse(particleSystem.IsActive);
		}

		private ParticleEmitter CreateAndAttachEmitter(Vector3D emitterPosition)
		{
			var textureData = new ImageCreationData(new Size(32));
			var material = new Material(ContentLoader.Create<Shader>(
				new ShaderCreationData(ShaderFlags.ColoredTextured)),
				ContentLoader.Create<Image>(textureData));
			var emitterData = new ParticleEmitterData { ParticleMaterial = material };
			var emitter = new ParticleEmitter(emitterData, emitterPosition);
			particleSystem.AttachEmitter(emitter);
			return emitter;
		}

		[Test, CloseAfterFirstFrame]
		public void DisposeEmitterDeactivates()
		{
			var emitterAlpha = CreateAndAttachEmitter(Vector3D.Zero);
			var emitterBeta = CreateAndAttachEmitter(Vector3D.UnitY);
			particleSystem.DisposeEmitter(1);
			particleSystem.DisposeEmitter(emitterAlpha);
			particleSystem.DisposeEmitter(emitterAlpha);
			Assert.IsFalse(emitterAlpha.IsActive);
			Assert.IsFalse(emitterBeta.IsActive);
			Assert.AreEqual(0, particleSystem.AttachedEmitters.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void RemoveEmitterJustDeattaches()
		{
			var emitterAlpha = CreateAndAttachEmitter(Vector3D.Zero);
			var emitterBeta = CreateAndAttachEmitter(Vector3D.UnitY);
			particleSystem.RemoveEmitter(0);
			particleSystem.RemoveEmitter(emitterBeta);
			Assert.AreEqual(0, particleSystem.AttachedEmitters.Count);
			Assert.IsTrue(emitterAlpha.IsActive);
			Assert.IsTrue(emitterBeta.IsActive);
		}

		[Test, CloseAfterFirstFrame]
		public void DisposingSystemDisposesAllEmitters()
		{
			var emitterAlpha = CreateAndAttachEmitter(Vector3D.Zero);
			var emitterBeta = CreateAndAttachEmitter(Vector3D.UnitY);
			particleSystem.DisposeSystem();
			Assert.IsFalse(emitterAlpha.IsActive);
			Assert.IsFalse(emitterBeta.IsActive);
		}

		[Test, CloseAfterFirstFrame]
		public void SettingPositionAndRotationOfSystemAlsoSetsForEmitters()
		{
			particleSystem.Position = Vector3D.One;
			var emitter = CreateAndAttachEmitter(Vector3D.Zero);
			particleSystem.Position = Vector3D.UnitY;
			particleSystem.Orientation = Quaternion.FromAxisAngle(Vector3D.UnitZ, 50);
			Assert.AreEqual(particleSystem.Position, emitter.Position);
			Assert.AreEqual(particleSystem.Orientation, emitter.Rotation);
		}

		[Test, CloseAfterFirstFrame]
		public void SystemCanBeCreatedFromDataOfEmitterNames()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage start
			var particleSystemData = new ParticleSystemData
			{
				emitterNames = new List<string>(new[] { "PointEmitter3D", "PointEmitter3D" })
			};
			var createdParticleSystem = new ParticleSystem(particleSystemData);
			Assert.AreEqual(particleSystemData.emitterNames.Count,
				createdParticleSystem.AttachedEmitters.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void LoadParticleSystemDataAsContent()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage start
			var particleSystemData = ContentLoader.Load<ParticleSystemData>("ParticleSystem");
			Assert.Greater(particleSystemData.emitterNames.Count, 0);
		}
	}
}