using System.Collections.Generic;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D.Cameras;
using DeltaEngine.Rendering3D.Shapes;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace DeltaEngine.Rendering3D.Particles.Tests
{
	[Ignore]
	public class Particle3DEmitterTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateCamera()
		{
			window = Resolve<Window>();
			window.BackgroundColor = Color.DarkGray;
			Camera.Current = new LookAtCamera(Resolve<Device>(), Resolve<Window>());
			camera = (LookAtCamera)Camera.Current;
			camera.Position = Vector3D.One;
			logo = new Material(ShaderFlags.ColoredTextured, "DeltaEngineLogo");
			spark = new Material(ShaderFlags.ColoredTextured, "ParticleSpark");
			fire = new Material(ShaderFlags.ColoredTextured, "ParticleFire");
			water = new Material(ShaderFlags.ColoredTextured, "EffectsWaterRanged");
			grid = new Grid3D(new Size(10));
		}

		private Window window;
		private LookAtCamera camera;
		private Material logo;
		private Material spark;
		private Material fire;
		private Material water;
		private Grid3D grid;

		private static ParticleEmitterData GetEmitterData(Material material, int maxParticles = 64,
			float lifeTime = 1.0f)
		{
			return new ParticleEmitterData
			{
				MaximumNumberOfParticles = maxParticles,
				SpawnInterval = lifeTime / maxParticles,
				LifeTime = lifeTime,
				Size =
					new RangeGraph<Size>(
						new List<Size>(new[] { new Size(0.1f), new Size(0.2f), new Size(0.1f) })),
				Color =
					new RangeGraph<Color>(
						new List<Color>(new[] { Color.Red, Color.Orange, Color.TransparentBlack })),
				Acceleration =
					new RangeGraph<Vector3D>(
						new List<Vector3D>(new[]
						{ Vector3D.UnitZ, new Vector3D(0.25f, 0.25f, 0.5f), new Vector3D(-0.25f, -0.25f, -4.0f) })),
				StartVelocity =
					new RangeGraph<Vector3D>(new Vector3D(-0.5f, -0.5f, 0.1f), new Vector3D(0.5f, 0.5f, 0.1f)),
				ParticleMaterial = material,
			};
		}

		[Test]
		public void SpawnOneBurst()
		{
			camera.Position = Vector3D.One / 2;
			var emitterData = GetEmitterData(spark, 512);
			emitterData.SpawnInterval = 0.0f;
			var emitter = new ParticleEmitter(emitterData, Vector3D.Zero);
			new Command(() => emitter.Spawn(64)).Add(new KeyTrigger(Key.Space));
		}

		[Test, CloseAfterFirstFrame]
		public void DisposingTwiceDoesNotError()
		{
			var emitter = new ParticleEmitter(GetEmitterData(spark, 512), Vector3D.Zero);
			emitter.DisposeAfterSeconds(1);
			emitter.DisposeAfterSeconds(1);
		}

		[Test]
		public void SpawnOneTimedBurst()
		{
			var emitterData = GetEmitterData(spark, 512);
			emitterData.SpawnInterval = 0.0007f;
			var emitter = new ParticleEmitter(emitterData, Vector3D.Zero);
			emitter.DisposeAfterSeconds(emitterData.LifeTime);
		}

		[Test]
		public void SmokeAndWind()
		{
			window.BackgroundColor = new Color(40, 64, 20);
			var defaultForce = new RangeGraph<Vector3D>(Vector3D.Zero);
			var windForce = new RangeGraph<Vector3D>(new Vector3D(-0.5f, -0.01f, 0.0f),
				new Vector3D(-1.0f, 0.01f, 0.0f));
			var emitterData = GetEmitterData(spark, 256, 2.0f);
			emitterData.Color = new RangeGraph<Color>(Color.White, Color.Transparent(Color.DarkGray));
			emitterData.Size = new RangeGraph<Size>(new Size(0.05f), new Size(0.2f));
			emitterData.Acceleration = defaultForce;
			emitterData.StartVelocity = new RangeGraph<Vector3D>(new Vector3D(0.0f, 0.0f, 0.35f),
				new Vector3D(0.1f, 0.1f, 0.1f));
			var emitter = new ParticleEmitter(emitterData, Vector3D.Zero);
			new Command(() => emitter.EmitterData.Acceleration = windForce).Add(new KeyTrigger(Key.Space));
			new Command(() => emitter.EmitterData.Acceleration = defaultForce).Add(
				new KeyTrigger(Key.Space, State.Releasing));
		}

		[Test]
		public void Fire()
		{
			var emitterData = GetEmitterData(fire, 512, 2.0f);
			emitterData.Color = new RangeGraph<Color>(new Color(16, 16, 16), new Color(255, 64, 64, 0));
			emitterData.Acceleration = new RangeGraph<Vector3D>(Vector3D.Zero, Vector3D.UnitZ * 0.1f);
			emitterData.Size = new RangeGraph<Size>(new Size(0.2f), new Size(0.1f));
			emitterData.StartVelocity = new RangeGraph<Vector3D>(new Vector3D(0.0f, 0.0f, 0.3f),
				new Vector3D(0.1f, 0.1f, 0.1f));
			new ParticleEmitter(emitterData, new Vector3D(0.1f, 0.0f, 0.0f));
			new ParticleEmitter(emitterData, new Vector3D(0.0f, 0.1f, 0.0f));
		}

		[Test, CloseAfterFirstFrame]
		public void SetForce()
		{
			var emitter = new ParticleEmitter(GetEmitterData(spark, 512), Vector3D.Zero);
			var force = new RangeGraph<Vector3D>(Vector3D.One);
			emitter.EmitterData.Acceleration = force;
			Assert.AreEqual(force, emitter.EmitterData.Acceleration);
		}

		[Test, CloseAfterFirstFrame]
		public void TooManyParticlesThrowsError()
		{
			var emitterData = GetEmitterData(spark, ParticleEmitter.MaxParticles + 1);
			var emitter = new ParticleEmitter(emitterData, Vector3D.Zero);
			Assert.Throws<ParticleEmitter.MaximumNumberOfParticlesExceeded>(() => emitter.Spawn());
			emitter.IsActive = false;
		}

		[Test, CloseAfterFirstFrame]
		public void ParticlesUpdatingPosition()
		{
			Randomizer.Use(new FixedRandom());
			var emitter = new ParticleEmitter(GetEmitterData(logo), Vector3D.Zero);
			AdvanceTimeAndUpdateEntities(0.1f);
			Assert.AreNotEqual(emitter.Position, emitter.particles[0].Position);
			Assert.IsTrue(
				emitter.particles[0].Position.IsNearlyEqual(new Vector3D(-0.03333334f, -0.03333334f,
					0.0058f)));
		}

		[Test, CloseAfterFirstFrame]
		public void ParticlesTrackingEmitterUpdatingPosition()
		{
			Randomizer.Use(new FixedRandom());
			var emitterData = GetEmitterData(logo);
			emitterData.DoParticlesTrackEmitter = true;
			var emitter = new ParticleEmitter(emitterData, Vector3D.Zero);
			AdvanceTimeAndUpdateEntities(0.1f);
			emitter.Position = Vector3D.One;
			AdvanceTimeAndUpdateEntities(0.1f);
			Assert.IsTrue(
				emitter.particles[0].Position.IsNearlyEqual(new Vector3D(0.90025f, 0.90025f, 1.0175f)));
		}

		[Test]
		public void ParticleTracksEmitterAcrossScreenFor4Seconds()
		{
			var emitter = new ParticleEmitter(CreateTrackingParticleData(), Vector3D.Zero);
			emitter.Start<MoveAcrossScreen>();
			emitter.DisposeAfterSeconds(4);
		}

		private ParticleEmitterData CreateTrackingParticleData()
		{
			return new ParticleEmitterData
			{
				MaximumNumberOfParticles = 1,
				SpawnInterval = 0.001f,
				Size = new RangeGraph<Size>(new Size(0.1f)),
				Color = new RangeGraph<Color>(Color.White),
				ParticleMaterial = logo,
				DoParticlesTrackEmitter = true
			};
		}

		private class MoveAcrossScreen : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (ParticleEmitter emitter in entities)
					emitter.Position += new Vector3D(Time.Delta / 2, 0.0f, 0.0f);
			}
		}

		[Test]
		public void ProjectileMovesAcrossScreenEmittingFire()
		{
			var start = -3 * Vector3D.UnitX;
			var emitter = new ParticleEmitter(CreateTrackingParticleData(), start);
			var emitter2 = new ParticleEmitter(CreateFireExhaustParticleData(), start);
			emitter.Start<MoveAcrossScreen>();
			emitter2.Start<MoveAcrossScreen>();
		}

		private ParticleEmitterData CreateFireExhaustParticleData()
		{
			return new ParticleEmitterData
			{
				MaximumNumberOfParticles = 200,
				SpawnInterval = 0.01f,
				LifeTime = 2.0f,
				Size =
					new RangeGraph<Size>(
						new List<Size>(new[] { new Size(0.1f), new Size(0.2f), new Size(0.1f) })),
				Color =
					new RangeGraph<Color>(
						new List<Color>(new[] { Color.Red, Color.Orange, Color.TransparentBlack })),
				Acceleration = new RangeGraph<Vector3D>(-Vector3D.UnitZ / 5),
				StartVelocity = new RangeGraph<Vector3D>(-Vector3D.UnitX / 2, Vector3D.One / 20),
				ParticleMaterial = fire
			};
		}

		[Test]
		public void FireOneBullet()
		{
			var emitterData = GetEmitterData(water, 512, 2.0f);
			emitterData.SpawnInterval = 0.0f;
			emitterData.Acceleration = new RangeGraph<Vector3D>(Vector3D.Zero, Vector3D.Zero);
			emitterData.Color = new RangeGraph<Color>(new Color(255, 255, 255), new Color(255, 255, 255));
			emitterData.Size = new RangeGraph<Size>(new Size(0.5f), new Size(0.5f));
			var emitter = new ParticleEmitter(emitterData, Vector3D.Zero);
			var enemy = new MockEnemy(new Vector3D(0, -3, 0), Size.Half, spark);
			new Command(() => //ncrunch: no coverage start
			{
				emitter.EmitterData.BillboardMode = BillboardMode.Ground;
				emitter.EmitterData.StartVelocity.Start = enemy.Position * 4.0f + enemy.direction * 0.5f;
				emitter.EmitterData.StartVelocity.End = Vector3D.Zero;
				emitter.Spawn();
			}).Add(new KeyTrigger(Key.Space));
			//ncrunch: no coverage end
		}

		[Test, CloseAfterFirstFrame]
		public void LoadParticleEmitterDataToGetConsistentValues()
		{
			var data = ContentLoader.Load<ParticleEmitterData>("FireEmitter");
			Assert.IsNotNull(data.ParticleMaterial.DiffuseMap);
			Assert.AreEqual(256, data.MaximumNumberOfParticles);
			Assert.AreEqual(5.0f, data.LifeTime);
			Assert.AreEqual(0.1f, data.SpawnInterval);
			Assert.AreEqual(Color.Red, data.Color.Start);
			Assert.AreEqual(Color.Green, data.Color.End);
		}

		[TestCase(BillboardMode.FrontAxis), TestCase(BillboardMode.CameraFacing),
		TestCase(BillboardMode.Ground), TestCase(BillboardMode.UpAxis),
		TestCase(BillboardMode.RightAxis), Test]
		public void SetDifferentBillBoardModes(BillboardMode mode)
		{
			var emitterData = GetEmitterData(logo);
			emitterData.BillboardMode = mode;
			emitterData.DoParticlesTrackEmitter = true;
			new ParticleEmitter(emitterData, Vector3D.Zero);
			Assert.DoesNotThrow(() => AdvanceTimeAndUpdateEntities());
		}

		[Test]
		public void SetDifferentBlendMode()
		{
			var emitterData = GetEmitterData(logo);
			emitterData.ParticleMaterial.DiffuseMap.BlendMode = BlendMode.Additive;
			emitterData.DoParticlesTrackEmitter = true;
			new ParticleEmitter(emitterData, Vector3D.Zero);
			AdvanceTimeAndUpdateEntities();
		}

		[Test]
		public void SwitchMaterialsOfParticles()
		{
			var emitterData = GetEmitterData(logo);
			emitterData.ParticleMaterial.DiffuseMap.BlendMode = BlendMode.Additive;
			var emitter = new ParticleEmitter(emitterData, Vector3D.Zero);
			AdvanceTimeAndUpdateEntities();
			emitter.particles[0].Material = new Material(ShaderFlags.ColoredTextured, "ParticleSpark");
			emitterData.ParticleMaterial.DiffuseMap.BlendMode = BlendMode.Additive;
			AdvanceTimeAndUpdateEntities();
		}

		[Test]
		public void EmittersFromSameDataAreStillIndependentlyChangeable()
		{
			var emitterData = GetEmitterData(logo);
			var emitterChanging = new ParticleEmitter(emitterData, Vector3D.Zero);
			var emitterStayingSame = new ParticleEmitter(emitterData, Vector3D.UnitX);
			emitterChanging.EmitterData.SpawnInterval += 0.2f;
			emitterChanging.EmitterData.Acceleration = new RangeGraph<Vector3D>(Vector3D.One);
			emitterChanging.EmitterData.Color = new RangeGraph<Color>(Color.Green, Color.Yellow);
			Assert.AreNotEqual(emitterStayingSame.EmitterData.SpawnInterval,
				emitterChanging.EmitterData.SpawnInterval);
			Assert.AreNotEqual(emitterStayingSame.EmitterData.Acceleration.Values,
				emitterChanging.EmitterData.Acceleration.Values);
			Assert.AreNotEqual(emitterStayingSame.EmitterData.Color.Values,
				emitterChanging.EmitterData.Color.Values);
		}

		[Test]
		public void CircleEmitterGivesCylinderWithDifferingZ()
		{
			var emitterData = GetEmitterData(logo, 256, 3);
			emitterData.PositionType = ParticleEmitterPositionType.CircleAroundCenter;
			emitterData.StartPosition = new RangeGraph<Vector3D>(new Vector3D(-0.1f, -0.1f, 0.0f),
				new Vector3D(0.1f, 0.1f, 0.0f));
			var emitter = new ParticleEmitter(emitterData, Vector3D.Zero);
			emitter.Position = -0.2f * Vector3D.UnitZ;
			emitter.Rotation = Quaternion.FromAxisAngle(Vector3D.UnitY, -10);
		}

		[Test]
		public void EscapeEmitterGivesCylinderWithDifferingZ()
		{
			var emitterData = GetEmitterData(logo);
			emitterData.PositionType = ParticleEmitterPositionType.CircleEscaping;
			emitterData.StartPosition = new RangeGraph<Vector3D>(new Vector3D(-0.5f, -0.5f, 0.0f),
				new Vector3D(0.5f, 0.5f, 0.5f));
			var emitter = new ParticleEmitter(emitterData, Vector3D.Zero);
			emitter.Position = -0.2f * Vector3D.UnitZ;
			emitter.Rotation = Quaternion.FromAxisAngle(Vector3D.UnitY, -10);
		}

		[Test]
		public void MultipleEmittersDifferentMaterials()
		{
			new ParticleEmitter(GetEmitterData(logo), Vector3D.Zero);
			new ParticleEmitter(GetEmitterData(spark), Vector3D.UnitX);
		}

		[Test]
		public void TrackRotationOfEmitterVisually()
		{
			var emitterData = CreateTrackingParticleData();
			emitterData.SpawnInterval = 0;
			emitterData.BillboardMode = BillboardMode.Ground;
			emitterData.LifeTime = 1.0f;
			var emitter = new ParticleEmitter(emitterData, Vector3D.Zero);
			new Command(Command.Click, 
				() => emitter.Rotation *= Quaternion.FromAxisAngle(Vector3D.UnitY, 30.0f)); //ncrunch: no coverage
			new Command(Command.MiddleClick, () => emitter.Spawn(1));
		}

		[Test, CloseAfterFirstFrame]
		public void TrackRotationOfEmitter()
		{
			var emitterData = CreateTrackingParticleData();
			emitterData.SpawnInterval = 0;
			emitterData.BillboardMode = BillboardMode.Ground;
			emitterData.LifeTime = 0.0f;
			var emitter = new ParticleEmitter(emitterData, Vector3D.Zero);
			emitter.Rotation = Quaternion.FromAxisAngle(Vector3D.UnitZ, 60);
			AdvanceTimeAndUpdateEntities();
			emitter.Spawn(1);
			Assert.AreEqual(emitter.Rotation.CalculateAxisAngle(), emitter.particles[0].Rotation);
			emitter.Rotation = Quaternion.FromAxisAngle(Vector3D.UnitZ, 90);
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(emitter.Rotation.CalculateAxisAngle(), emitter.particles[0].Rotation);
		}

		[Test]
		public void SpawnParticlesInCertainPosition()
		{
			var emitter = new ParticleEmitter(GetEmitterData(logo), Vector3D.Zero);
			emitter.SpawnAtRelativePosition(Vector3D.One, 1);
			Assert.AreEqual(Vector3D.One, emitter.particles[0].Position);
			emitter.SpawnAtRelativePosition(Vector3D.UnitX);
			Assert.AreEqual(Vector3D.UnitX, emitter.particles[1].Position);
		}

		[Test]
		public void SpawnParticlesWithCertainRotation()
		{
			var emitter = new ParticleEmitter(GetEmitterData(logo), Vector3D.Zero);
			emitter.SpawnAtRelativePositionAndRotation(Vector3D.One, 0.5f, 1);
			Assert.AreEqual(Vector3D.One, emitter.particles[0].Position);
			emitter.SpawnAtRelativePositionAndRotation(Vector3D.UnitX, 0.5f);
			Assert.AreEqual(Vector3D.UnitX, emitter.particles[1].Position);
		}

		[Test]
		public void DrawLensFlareEffect()
		{
			grid.IsActive = false;
			freeCamera = Camera.Use<FreeCamera>();
			freeCamera.Position = Vector3D.UnitZ;
			CreateParticleEmitters();
			new Command(MoveLight).Add(new MouseMovementTrigger());
		}

		private FreeCamera freeCamera;

		private void CreateParticleEmitters()
		{
			var bigGlowMaterial = new Material(ShaderFlags.ColoredTextured, "HardGlow");
			bigGlowMaterial.DefaultColor = new Color(150, 150, 200);
			bigGlow = new ParticleEmitter(GetLensEffectData(bigGlowMaterial, 0.5f), Vector3D.Zero);
			var streaksMaterial = new Material(ShaderFlags.ColoredTextured, "Streaks");
			streaksMaterial.DefaultColor = new Color(150, 150, 200);
			streaks = new ParticleEmitter(GetLensEffectData(streaksMaterial, 0.25f), Vector3D.Zero);
			var softGlowMaterial = new Material(ShaderFlags.ColoredTextured, "SoftGlow");
			softGlowMaterial.DefaultColor = new Color(200, 200, 255, 128);
			softGlow = new ParticleEmitter(GetLensEffectData(softGlowMaterial, 0.1f), Vector3D.Zero);
		}

		private ParticleEmitter bigGlow;
		private ParticleEmitter streaks;
		private ParticleEmitter softGlow;

		private static ParticleEmitterData GetLensEffectData(Material material, float scale)
		{
			return new ParticleEmitterData
			{
				PositionType = ParticleEmitterPositionType.Point,
				DoParticlesTrackEmitter = true,
				ParticleMaterial = material,
				MaximumNumberOfParticles = 1,
				SpawnInterval = 0.001f,
				Size = new RangeGraph<Size>(new Size(scale))
			};
		}

		private void MoveLight(Vector2D mousePosition)
		{
			var position = new Vector2D(mousePosition.X - 0.5f, 0.5f - mousePosition.Y);
			bigGlow.Position = position;
			streaks.Position = position;
			softGlow.Position = position * 0.75f;
		}
	}
}