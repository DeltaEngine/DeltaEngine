using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Rendering3D.Particles.Tests
{
	internal class EmitterDataLoadingTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void LoadCommonGradeData()
		{
			PrepareSomeData();
			var loadedData = new ParticleEmitterData();
			loadedData.LoadFromFile(dataStream);
			Assert.AreEqual(emitterData.SpawnInterval, loadedData.SpawnInterval);
			Assert.AreEqual(emitterData.LifeTime, loadedData.LifeTime);
			Assert.AreEqual(emitterData.MaximumNumberOfParticles, loadedData.MaximumNumberOfParticles);
			Assert.AreEqual(emitterData.StartVelocity.Values, loadedData.StartVelocity.Values);
			Assert.AreEqual(emitterData.Acceleration.Values, loadedData.Acceleration.Values);
			Assert.AreEqual(emitterData.Size.Values, loadedData.Size.Values);
			Assert.AreEqual(emitterData.StartRotation.Values, loadedData.StartRotation.Values);
			Assert.AreEqual(emitterData.RotationSpeed.Values, loadedData.RotationSpeed.Values);
			Assert.AreEqual(emitterData.Color.Values, loadedData.Color.Values);
			Assert.AreEqual(emitterData.PositionType, loadedData.PositionType);
			Assert.AreEqual(emitterData.StartPosition.Values, loadedData.StartPosition.Values);
			Assert.AreEqual(emitterData.ParticlesPerSpawn.Values, loadedData.ParticlesPerSpawn.Values);
			Assert.AreEqual(emitterData.DoParticlesTrackEmitter, loadedData.DoParticlesTrackEmitter);
			Assert.AreEqual(emitterData.BillboardMode, loadedData.BillboardMode);
		}

		private void PrepareSomeData()
		{
			emitterData = new ParticleEmitterData();
			emitterData.LifeTime = 1.2f;
			emitterData.SpawnInterval = 0.2f;
			emitterData.BillboardMode = BillboardMode.Ground;
			emitterData.MaximumNumberOfParticles = 128;
			emitterData.ParticleMaterial = new Material(ShaderFlags.Position2DColoredTextured,
				"DeltaEngineLogo");
			emitterData.ParticlesPerSpawn = new RangeGraph<ValueRange>(new ValueRange(3.0f, 3.0f),
				new ValueRange(3.0f, 3.0f));
			emitterData.Color = new RangeGraph<Color>(new[] { Color.Black, Color.DarkGreen, Color.Gold });
			emitterData.StartVelocity =
				new RangeGraph<Vector3D>(new[] { -Vector3D.UnitX, Vector3D.UnitX, Vector3D.Zero });
			SaveDataToStream();
		}

		private ParticleEmitterData emitterData;

		private void SaveDataToStream()
		{
			dataBytes = BinaryDataExtensions.ToByteArrayWithTypeInformation(emitterData);
			dataStream = new MemoryStream(dataBytes);
		}

		private byte[] dataBytes;
		private MemoryStream dataStream;

		[Test, CloseAfterFirstFrame]
		public void LoadDefaultData()
		{
			PrepareSimplestPossibleData();
			var loadedData = new ParticleEmitterData();
			loadedData.LoadFromFile(dataStream);
		}

		private void PrepareSimplestPossibleData()
		{
			emitterData = new ParticleEmitterData();
			emitterData.ParticleMaterial = new Material(ShaderFlags.Position2DColoredTextured,
				"DeltaEngineLogo");
			SaveDataToStream();
		}

		[Test, CloseAfterFirstFrame]
		public void LoadingFromEmptyStreamThrows()
		{
			dataStream = new MemoryStream();
			var loadedData = new ParticleEmitterData();
			Assert.Throws<ParticleEmitterData.EmptyEmitterDataFileGiven>(
				() => { loadedData.LoadFromFile(dataStream); });
		}
	}
}