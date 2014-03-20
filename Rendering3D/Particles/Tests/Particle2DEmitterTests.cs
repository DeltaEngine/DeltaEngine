using System.Collections.Generic;
using System.IO;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace DeltaEngine.Rendering3D.Particles.Tests
{
	[Ignore]
	internal class Particle2DEmitterTests : TestWithMocksOrVisually
	{
		//ncrunch: no coverage start
		[Test, Category("Slow")]
		public void Render1000Particles()
		{
			CreateDataAndEmitter(1000, 0.01f, 10);
			emitter.Position = new Vector2D(0.5f, 0.4f);
			Rectangle fpsDrawArea = Rectangle.FromCenter(0.5f, 0.25f, 0.2f, 0.2f);
			new FpsDisplay(emitter, fpsDrawArea);
		} //ncrunch: no coverage end

		private void CreateDataAndEmitter(int maxParticles = 1, float spawnInterval = 0.01f,
			float lifeTime = 0.002f)
		{
			emitterData = CreateData(maxParticles, spawnInterval, lifeTime);
			emitter = new ParticleEmitter(emitterData, Vector2D.Half);
		}

		private ParticleEmitterData emitterData;
		private ParticleEmitter emitter;

		private static ParticleEmitterData CreateData(int maxParticles = 1,
			float spawnInterval = 0.01f, float lifeTime = 0.002f)
		{
			return new ParticleEmitterData
			{
				MaximumNumberOfParticles = maxParticles,
				SpawnInterval = spawnInterval,
				LifeTime = lifeTime,
				Size = new RangeGraph<Size>(new Size(0.02f), new Size(0.06f)),
				Color =
					new RangeGraph<Color>(
						new List<Color>(new[]
						{ Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Purple })),
				Acceleration =
					new RangeGraph<Vector3D>(new Vector3D(0, 0.1f, 0), new Vector3D(0.2f, 0.1f, 0)),
				StartVelocity =
					new RangeGraph<Vector3D>(new Vector3D(-0.05f, -0.1f, 0), new Vector3D(0.05f, -0.015f, 0)),
				StartRotation =
					new RangeGraph<ValueRange>(new ValueRange(20f, 100f), new ValueRange(60f, 300f)),
				RotationSpeed = new RangeGraph<ValueRange>(new ValueRange(0, 10), new ValueRange(55, 75)),
				ParticleMaterial = new Material(ShaderFlags.Position2DColoredTextured, "DeltaEngineLogo"),
				PositionType = ParticleEmitterPositionType.Point,
				StartPosition =
					new RangeGraph<Vector3D>(new Vector2D(-0.1f, -0.1f), new Vector2D(0.1f, 0.1f))
			};
		}

		public class FpsDisplay : FontText, Updateable
		{
			public FpsDisplay(ParticleEmitter emitter, Rectangle drawArea)
				: base(Font.Default, "", drawArea)
			{
				emitters = new []{emitter};
			}


			public FpsDisplay(ParticleEmitter[] emitters, Rectangle drawArea)
				: base(Font.Default, "", drawArea)
			{
				this.emitters = emitters;
			}

			private readonly ParticleEmitter[] emitters;

			public void Update()
			{
				int particleCount = 0;
				foreach (var particleEmitter in emitters)
				{
					particleCount += particleEmitter.NumberOfActiveParticles;
				}
				Text = "Fps = " + GlobalTime.Current.Fps + "  Count = " + particleCount;
			}
		}

		[Test]
		public void Render2000Particles()
		{
			var emitters = new[]
			{
				new ParticleEmitter(CreateData(1000, 0.01f, 10), new Vector2D(0.3f, 0.5f)),
				new ParticleEmitter(CreateData(1000, 0.01f, 10), new Vector2D(0.7f, 0.5f))
			};
			new FpsDisplay(emitters, Rectangle.FromCenter(0.5f, 0.25f, 0.2f, 0.2f));
		}

		[Test]
		public void ParticlesAreEmittedAtMousePosition()
		{
			CreateDataAndEmitter(1024, 0.01f, 5);
			new Command(position => emitter.Position = position).Add(new MouseMovementTrigger());
		}

		[Test]
		public void InactiveEmitterDoesNothing()
		{
			CreateDataAndEmitter(512, 0.01f, 5);
			emitter.Position = new Vector2D(0.5f, 0.4f);
			emitter.IsActive = false;
		}

		[Test]
		public void CreateEmitterWithJustOneParticle()
		{
			CreateDataAndEmitter(1, 0.01f, 5);
			emitter.Position = new Vector2D(0.5f, 0.7f);
		}

		[Test, CloseAfterFirstFrame]
		public void AdvanceCreatingOneParticle()
		{
			CreateDataAndEmitter();
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(1, emitter.NumberOfActiveParticles);
		}

		[Test, CloseAfterFirstFrame]
		public void AdvanceTwoParticlesPastExpiry()
		{
			CreateDataAndEmitter(2, 0.1f);
			AdvanceTimeAndUpdateEntities(0.1f);
			Assert.AreEqual(1, emitter.NumberOfActiveParticles);
		}

		[Test]
		public void MultipleEmittersShallNotInterfere()
		{
			CreateDataAndEmitter(12, 0.1f, 5);
			var data = new ParticleEmitterData
			{
				MaximumNumberOfParticles = 12,
				SpawnInterval = 0.4f,
				LifeTime = 2,
				Size = new RangeGraph<Size>(new Size(0.03f)),
				Color = new RangeGraph<Color>(Color.Gray),
				Acceleration = new RangeGraph<Vector3D>(-Vector2D.UnitY),
				StartVelocity = new RangeGraph<Vector3D>(new Vector2D(0.3f, -0.1f)),
				ParticleMaterial = new Material(ShaderFlags.Position2DColoredTextured, "DeltaEngineLogo")
			};
			new ParticleEmitter(data, Vector2D.Half);
		}

		[Test, CloseAfterFirstFrame]
		public void ParticlesUpdatingPosition()
		{
			Randomizer.Use(new FixedRandom());
			CreateDataAndEmitter();
			AdvanceTimeAndUpdateEntities(0.1f);
			Assert.AreEqual(new Vector3D(0.4f, 0.4f, 0), emitter.particles[0].Position);
		}

		[Test, CloseAfterFirstFrame]
		public void ParticlesTrackingEmitterUpdatingPosition()
		{
			Randomizer.Use(new FixedRandom());
			CreateDataAndEmitter();
			emitterData.DoParticlesTrackEmitter = true;
			AdvanceTimeAndUpdateEntities(0.1f);
			emitter.Position = Vector2D.One;
			AdvanceTimeAndUpdateEntities(0.1f);
			Assert.AreEqual(new Vector3D(0.9f, 0.9f, 0), emitter.particles[0].Position);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateParticleEmitterAddingDefaultComponents()
		{
			var emptyMaterial = new Material(ShaderFlags.Position2DColored, "");
			new ParticleEmitter(new ParticleEmitterData { ParticleMaterial = emptyMaterial },
				Vector2D.Zero);
		}

		[Test]
		public void CreateEmitterAndKeepRunningWithAnimation()
		{
			emitter = new ParticleEmitter(CreateDataAndEmitterWithAnimation("ImageAnimation"),
				Vector2D.Half) { Position = new Vector2D(0.5f, 0.7f) };
		}

		private ParticleEmitterData CreateDataAndEmitterWithAnimation(string contentName)
		{
			emitterData = new ParticleEmitterData
			{
				MaximumNumberOfParticles = 512,
				SpawnInterval = 0.1f,
				LifeTime = 5f,
				Size = new RangeGraph<Size>(new Size(0.05f), new Size(0.10f)),
				Color = new RangeGraph<Color>(Color.White),
				Acceleration = new RangeGraph<Vector3D>(new Vector2D(0, 0.1f)),
				StartVelocity =
					new RangeGraph<Vector3D>(new Vector2D(0, -0.3f), new Vector2D(0.05f, 0.01f)),
				ParticleMaterial = new Material(ShaderFlags.Position2DColoredTextured, contentName)
			};
			return emitterData;
		}

		[Test]
		public void CreateEmitterAndKeepRunningWithSpriteSheetAnimation()
		{
			emitterData = CreateDataAndEmitterWithAnimation("EarthSpriteSheet");
			emitter = new ParticleEmitter(emitterData, Vector2D.Half);
			emitter.Position = new Vector2D(0.5f, 0.7f);
			AdvanceTimeAndUpdateEntities();
			Assert.IsTrue(emitter.IsPauseable);
		}

		[Test]
		public void CreateRotatedParticles()
		{
			emitterData = CreateDataAndEmitterWithAnimation("DeltaEngineLogo");
			emitterData.StartRotation = new RangeGraph<ValueRange>(new ValueRange(45, 50),
				new ValueRange(65, 70));
			emitterData.Size = new RangeGraph<Size>(new Size(0.05f));
			emitter = new ParticleEmitter(emitterData, Vector2D.Half);
			emitter.Position = new Vector2D(0.5f, 0.7f);
		}

		[Test]
		public void CreateRotatingParticles()
		{
			emitterData = CreateDataAndEmitterWithAnimation("DeltaEngineLogo");
			emitterData.StartRotation = new RangeGraph<ValueRange>(new ValueRange(0, 5),
				new ValueRange(45, 50));
			emitterData.RotationSpeed = new RangeGraph<ValueRange>(new ValueRange(300.0f, 320.0f),
				new ValueRange(50f, 80f));
			emitterData.Size = new RangeGraph<Size>(new Size(0.05f));
			emitter = new ParticleEmitter(emitterData, Vector2D.Half);
			emitter.Position = new Vector2D(0.5f, 0.7f);
		}

		[Test, CloseAfterFirstFrame]
		public void Spawn()
		{
			CreateDataAndEmitter(512, 0.0f, 1);
			emitter.Spawn(20);
			Assert.AreEqual(20, emitter.NumberOfActiveParticles);
		}

		[Test]
		public void Spawn500OnMouseClick()
		{
			CreateDataAndEmitter(512, 0.0f, 1);
			new Command(() => emitter.Spawn(500)).Add(new MouseButtonTrigger());
		}

		[Test]
		public void SpawnAndDispose()
		{
			CreateDataAndEmitter(512, 0.01f, 0.1f);
			emitter.SpawnAndDispose(20);
			AdvanceTimeAndUpdateEntities(0.11f);
			Assert.IsFalse(emitter.IsActive);
		}

		[Test, CloseAfterFirstFrame]
		public void DisposeEmitterAfterSetTime()
		{
			emitterData = CreateDataAndEmitterWithAnimation("DeltaEngineLogo");
			emitter = new ParticleEmitter(emitterData, Vector2D.Half);
			emitter.DisposeAfterSeconds(0.1f);
			AdvanceTimeAndUpdateEntities(0.25f);
			emitter.DisposeAfterSeconds(0.1f);
			Assert.IsFalse(emitter.IsActive);
		}

		[Test, CloseAfterFirstFrame]
		public void ParticleWithNoMaterialThrowsException()
		{
			emitterData = CreateDataAndEmitterWithAnimation("DeltaEngineLogo");
			emitterData.ParticleMaterial = null;
			Assert.Throws<ParticleEmitter.UnableToCreateWithoutMaterial>(
				() => new ParticleEmitter(emitterData, new Vector2D(0.5f, 0.5f)));
		}

		[Test]
		public void SpawnSeveralParticlesPerInterval()
		{
			CreateDataAndEmitter(128, 2.2f, 2f);
			emitterData.ParticlesPerSpawn.Start = new ValueRange(64, 128);
			emitterData.StartPosition = new RangeGraph<Vector3D>(Vector3D.UnitX * -0.4f,
				Vector3D.UnitX * 0.4f);
		}

		[Test, CloseAfterFirstFrame]
		public void SaveAndLoadParticle2D()
		{
			CreateDataAndEmitter(512, 0.01f, 5);
			var stream = BinaryDataExtensions.SaveToMemoryStream(emitterData);
			var loaded = stream.CreateFromMemoryStream() as ParticleEmitterData;
			File.WriteAllBytes("Test.test", stream.ToArray());
			Assert.AreEqual(685, stream.Length);
			Assert.AreEqual(emitterData.PositionType, loaded.PositionType);
		}

		[Test]
		public void ParticlesSwirlAroundMousePosition()
		{
			emitterData = GetSwirlingParticleEmitterData();
			emitter = new ParticleEmitter(emitterData, Vector2D.Half);
			new Command(position => emitter.Position = position).Add(new MouseMovementTrigger());
		}

		private static ParticleEmitterData GetSwirlingParticleEmitterData()
		{
			return new ParticleEmitterData
			{
				MaximumNumberOfParticles = 17,
				SpawnInterval = 0.25f,
				LifeTime = 4.0f,
				Acceleration = new RangeGraph<Vector3D>(SwirlingAccelerations),
				StartVelocity = new RangeGraph<Vector3D>(new Vector3D(0.05f, 0.0f, 0.0f), Vector3D.Zero),
				StartPosition = new RangeGraph<Vector3D>(new Vector3D(0.0f, -0.04f, 0.0f)),
				Size = new RangeGraph<Size>(new Size(0.01f)),
				Color = new RangeGraph<Color>(RainbowColors),
				ParticleMaterial = new Material(ShaderFlags.Position2DColoredTextured, "ImageAnimation"),
				DoParticlesTrackEmitter = true
			};
		}

		//ncrunch: no coverage start
		private static readonly List<Vector3D> SwirlingAccelerations = new List<Vector3D>
		{
			new Vector3D(0.0f, Magnitude, 0.0f),
			new Vector3D(-Magnitude, 0.0f, 0.0f),
			new Vector3D(0.0f, -Magnitude, 0.0f),
			new Vector3D(Magnitude, 0.0f, 0.0f),
			new Vector3D(0.0f, Magnitude * 0.4f, 0.0f)
		};

		private const float Magnitude = 0.1f;

		private static readonly List<Color> RainbowColors = new List<Color>
		{
			Color.Red,
			Color.Yellow,
			Color.Green,
			Color.Blue,
			Color.Purple,
			Color.Red
		};
		//ncrunch: no coverage end

		[Test]
		public void ParticleTracksEmitterAcrossScreenFor4Seconds()
		{
			emitterData = new ParticleEmitterData
			{
				MaximumNumberOfParticles = 1,
				SpawnInterval = 0.001f,
				Size = new RangeGraph<Size>(new Size(0.1f)),
				Color = new RangeGraph<Color>(Color.White),
				ParticleMaterial = new Material(ShaderFlags.Position2DColoredTextured, "ImageAnimation"),
				DoParticlesTrackEmitter = true
			};
			emitter = new ParticleEmitter(emitterData, new Vector2D(0.2f, 0.5f));
			emitter.Start<MoveAcrossScreen>();
			emitter.DisposeAfterSeconds(4);
			AdvanceTimeAndUpdateEntities();
		}

		private class MoveAcrossScreen : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (ParticleEmitter emitter in entities)
					emitter.Position += new Vector3D(Time.Delta / 10, 0.0f, 0.0f);
			}
		}

		[Test]
		public void EmittersFromSameDataAreStillIndependentlyChangeable()
		{
			emitterData = CreateDataAndEmitterWithAnimation("DeltaEngineLogo");
			var emitterChanging = new ParticleEmitter(emitterData, Vector3D.UnitY);
			var emitterStayingSame = new ParticleEmitter(emitterData, Vector3D.UnitX);
			emitterChanging.EmitterData.SpawnInterval += 0.2f;
			emitterChanging.EmitterData.Acceleration = new RangeGraph<Vector3D>(Vector3D.One);
			emitterChanging.EmitterData.Color = new RangeGraph<Color>(Color.Green, Color.Yellow);
			emitterChanging.EmitterData.StartRotation.Start = new ValueRange(3, 4);
			Assert.AreNotSame(emitterData, emitterChanging.EmitterData);
			Assert.AreNotEqual(emitterStayingSame.EmitterData.SpawnInterval,
				emitterChanging.EmitterData.SpawnInterval);
			Assert.AreNotEqual(emitterStayingSame.EmitterData.Acceleration.Values,
				emitterChanging.EmitterData.Acceleration.Values);
			Assert.AreNotEqual(emitterStayingSame.EmitterData.Color.Values,
				emitterChanging.EmitterData.Color.Values);
			Assert.AreNotEqual(emitterStayingSame.EmitterData.StartRotation,
				emitterChanging.EmitterData.StartRotation);
		}

		[Test]
		public void CreatePointEmitter()
		{
			CreateVariableTypeEmitter(ParticleEmitterPositionType.Point);
		}

		private static void CreateVariableTypeEmitter(ParticleEmitterPositionType type)
		{
			var createdEmitterData = CreateData(100, 0.05f, 1.0f);
			createdEmitterData.PositionType = type;
			new ParticleEmitter(createdEmitterData, new Vector3D(0.5f, 0.5f, 0.5f));
		}

		[Test]
		public void CreateLineEmitter()
		{
			CreateVariableTypeEmitter(ParticleEmitterPositionType.Line);
		}

		[Test]
		public void CreateBoxEmitter()
		{
			CreateVariableTypeEmitter(ParticleEmitterPositionType.Box);
		}

		[Test]
		public void CreateSphereEmitter()
		{
			CreateVariableTypeEmitter(ParticleEmitterPositionType.Sphere);
		}

		[Test]
		public void CreateSphereBorderEmitter()
		{
			CreateVariableTypeEmitter(ParticleEmitterPositionType.SphereBorder);
		}

		[Test]
		public void CreateCircleAroundCenterEmitter()
		{
			CreateVariableTypeEmitter(ParticleEmitterPositionType.CircleAroundCenter);
			AdvanceTimeAndUpdateEntities();
		}

		[Test]
		public void CreateCircleEscapingEmitter()
		{
			CreateVariableTypeEmitter(ParticleEmitterPositionType.CircleEscaping);
			AdvanceTimeAndUpdateEntities();
		}

		[Test]
		public void CreateMeshEmitter()
		{
			CreateVariableTypeEmitter(ParticleEmitterPositionType.Mesh);
		}

		[Test, CloseAfterFirstFrame]
		public void ExceedingMaximumIsAvoided()
		{
			Assert.Throws<ParticleEmitter.MaximumNumberOfParticlesExceeded>(() =>
			{
				emitter = new ParticleEmitter(CreateData(3000), Vector3D.Zero);
				AdvanceTimeAndUpdateEntities();
			});
			emitter.IsActive = false;
		}

		[Test]
		public void RotateEmitterOnClick()
		{
			var particleEmitter = new ParticleEmitter(CreateData(100, 0.1f, 1.0f),
				new Vector3D(0.5f, 0.5f, 0.0f));
			bool rotated = false;
			new Command(Command.Click, () =>
			{//ncrunch: no coverage start
				particleEmitter.Rotation = rotated
					? Quaternion.Identity : Quaternion.FromAxisAngle(Vector3D.UnitZ, 45);
				rotated = !rotated;
			}); //ncrunch: no coverage end
		}

		[Test]
		public void SpawnAtClickedPosition()
		{
			var particleEmitter = new ParticleEmitter(CreateData(100, 0.0f, 1.0f), Vector3D.Zero);
			new Command(Command.Click, position => { particleEmitter.SpawnAtRelativePosition(position); });
		}

		[Test]
		public void TrackRotationOfEmitter()
		{
			var data = CreateData(100, 0.2f, 1.0f);
			data.DoParticlesTrackEmitter = true;
			var particleEmitter = new ParticleEmitter(data, new Vector3D(0.5f, 0.5f, 0.0f));
			new Command(Command.Click,
				() => particleEmitter.Rotation *= Quaternion.FromAxisAngle(Vector3D.UnitZ, 20.0f)); //ncrunch: no coverage
		}
	}
}