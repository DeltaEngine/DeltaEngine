using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering3D.Particles
{
	public class ParticleEmitter : DrawableEntity, Updateable
	{
		public ParticleEmitter(ParticleEmitterData emitterData, Vector3D spawnPosition)
		{
			if (emitterData.ParticleMaterial == null)
				throw new UnableToCreateWithoutMaterial(); //ncrunch: no coverage
			EmitterData = ParticleEmitterData.CopyFrom(emitterData);
			ElapsedSinceLastSpawn = emitterData.SpawnInterval;
			Position = spawnPosition;
			Rotation = Quaternion.Identity;
			lastFramePosition = spawnPosition;
			lastFrameRotation = Quaternion.Identity;
			CreateImageAnimationMaterials();
			StartRendererBasedOnShaderFormat();
		}

		public class UnableToCreateWithoutMaterial : Exception { }

		public ParticleEmitterData EmitterData { get; private set; }
		public float ElapsedSinceLastSpawn { get; set; }
		public Vector3D Position { get; set; }
		public Quaternion Rotation { get; set; }
		private Vector3D lastFramePosition;
		private Quaternion lastFrameRotation;

		private void CreateImageAnimationMaterials()
		{
			ImageAnimation animation = EmitterData.ParticleMaterial.Animation;
			if (animation == null)
				return;
			//ncrunch: no coverage start
			animationMaterials = new Material[animation.Frames.Length];
			for (int i = 0; i < animation.Frames.Length; i++)
				animationMaterials[i] = new Material(EmitterData.ParticleMaterial.Shader,
					animation.Frames[i]);
			//ncrunch: no coverage end
		}

		private Material[] animationMaterials;

		private void StartRendererBasedOnShaderFormat()
		{
			if ((EmitterData.ParticleMaterial.Shader as ShaderWithFormat).Format.Is3D)
				OnDraw<Particle3DRenderer>();
			else
				OnDraw<Particle2DRenderer>();
		}

		public void Update()
		{
			UpdateAndLimitNumberOfActiveParticles();
			UpdateAnimation();
			SpawnNewParticles();
		}

		private void UpdateAndLimitNumberOfActiveParticles()
		{
			if (EmitterData.PositionType == ParticleEmitterPositionType.CircleAroundCenter)
				UpdateParticlesTracingCircularOutline(); //ncrunch: no coverage
			else if (EmitterData.PositionType == ParticleEmitterPositionType.CircleEscaping)
				UpdateParticlesRadialEscape(); //ncrunch: no coverage
			else
				UpdateParticlesBasic();
		}

		public Particle[] particles;

		//ncrunch: no coverage start
		private void UpdateParticlesTracingCircularOutline()
		{
			int lastIndex =
				UpdateParticlesAndGetLastActive(
					index => particles[index].UpdateRoundingParticleIfStillActive(EmitterData, Position));
			SetLastFrameAttributes(lastIndex);
		} //ncrunch: no coverage end

		private int UpdateParticlesAndGetLastActive(Func<int, bool> updateMethod)
		{
			int lastIndex = -1;
			for (int index = 0; index < NumberOfActiveParticles; index++)
				if (updateMethod(index))
				{
					lastIndex = index;
					UpdateParticleProperties(index);
				}
			return lastIndex;
		}

		private void SetLastFrameAttributes(int lastIndex)
		{
			NumberOfActiveParticles = lastIndex + 1;
			lastFramePosition = Position;
			lastFrameRotation = Rotation;
		}

		//ncrunch: no coverage start
		private void UpdateParticlesRadialEscape()
		{
			int lastIndex =
				UpdateParticlesAndGetLastActive(
					index => particles[index].UpdateEscapingParticleIfStillActive(EmitterData, Position));
			SetLastFrameAttributes(lastIndex);
		} //ncrunch: no coverage end

		private void UpdateParticlesBasic()
		{
			int lastIndex = UpdateParticlesAndGetLastActive(UpdateParticle);
			SetLastFrameAttributes(lastIndex);
		}

		public int NumberOfActiveParticles { get; protected set; }

		private bool UpdateParticle(int index)
		{
			return particles[index].UpdateIfStillActive(EmitterData);
		}

		private void UpdateParticleProperties(int index)
		{
			var interpolation = particles[index].ElapsedTime / EmitterData.LifeTime;
			particles[index].Color = EmitterData.Color.GetInterpolatedValue(interpolation);
			particles[index].Size = EmitterData.Size.GetInterpolatedValue(interpolation);
			var acceleration = EmitterData.Acceleration.GetInterpolatedValue(interpolation);
			particles[index].Acceleration = acceleration.GetVector2D();
			if (!EmitterData.DoParticlesTrackEmitter)
				return;
			//ncrunch: no coverage start
			particles[index].Position += Position - lastFramePosition;
			particles[index].Rotation +=
				Math.Abs(Rotation.CalculateAxisAngle() - lastFrameRotation.CalculateAxisAngle());
			//ncrunch: no coverage end
		}

		//ncrunch: no coverage start
		private void UpdateAnimation()
		{
			if (EmitterData.ParticleMaterial.Animation != null)
				UpdateAnimationForParticles();
			if (EmitterData.ParticleMaterial.SpriteSheet != null)
				UpdateSpriteSheetAnimationForParticles();
		}

		private void UpdateAnimationForParticles()
		{
			var animation = EmitterData.ParticleMaterial.Animation;
			var duration = EmitterData.ParticleMaterial.Duration;
			for (int index = 0; index < NumberOfActiveParticles; index++)
			{
				particles[index].CurrentFrame =
					(int)(animation.Frames.Length * particles[index].ElapsedTime / duration) %
						animation.Frames.Length;
				particles[index].Material = animationMaterials[particles[index].CurrentFrame];
			}
		}

		private void UpdateSpriteSheetAnimationForParticles()
		{
			var sheet = EmitterData.ParticleMaterial.SpriteSheet;
			var duration = EmitterData.ParticleMaterial.Duration;
			for (int index = 0; index < NumberOfActiveParticles; index++)
			{
				particles[index].CurrentFrame =
					(int)(sheet.UVs.Count * particles[index].ElapsedTime / duration) % sheet.UVs.Count;
				particles[index].CurrentUV = sheet.UVs[particles[index].CurrentFrame];
			}
		} //ncrunch: no coverage end

		private void SpawnNewParticles()
		{
			ElapsedSinceLastSpawn += Time.Delta;
			if (EmitterData.SpawnInterval <= 0.0f || IsAnyEmitterDataNull())
				return;
			while (ElapsedSinceLastSpawn >= EmitterData.SpawnInterval)
				DoIntervalSpawn();
		}

		private bool IsAnyEmitterDataNull()
		{
			return EmitterData.Acceleration == null || EmitterData.Size == null ||
				EmitterData.StartVelocity == null || EmitterData.StartPosition == null ||
				EmitterData.ParticleMaterial.DiffuseMap == null;
		}

		private void DoIntervalSpawn()
		{
			ElapsedSinceLastSpawn -= EmitterData.SpawnInterval;
			var numberOfParticles = (int)EmitterData.ParticlesPerSpawn.Start.GetRandomValue();
			Spawn(numberOfParticles);
		}

		public void Spawn(int numberOfParticles = 0)
		{
			if (numberOfParticles == 0)
				SpawnSomeParticlesAt(GetParticleSpawnPosition(),
					(int)EmitterData.ParticlesPerSpawn.Start.Start);
			else
				SpawnSomeParticlesAt(GetParticleSpawnPosition(), numberOfParticles);
		}

		private void SpawnSomeParticlesAt(Vector3D position, int numberOfParticles)
		{
			CheckArrayAndInitFreeSpot();
			for (int i = 0; i < numberOfParticles; i++)
				SpawnOneParticle(position, EmitterData.StartRotation.Start.GetRandomValue());
		}

		private void CheckArrayAndInitFreeSpot()
		{
			CreateParticleArrayIfNecessary();
			lastFreeSpot = -1;
		}

		private void SpawnSomeParticlesDefiningRotation(Vector3D position, int numberOfParticles,
			float rotation)
		{
			CheckArrayAndInitFreeSpot();
			for (int i = 0; i < numberOfParticles; i++)
				SpawnOneParticle(position, rotation);
		}

		private int lastFreeSpot;

		private void CreateParticleArrayIfNecessary()
		{
			if (particles != null && particles.Length == EmitterData.MaximumNumberOfParticles)
				return;
			VerifyNumberOfParticlesDoesNotExceedMaximumAllowed();
			particles = new Particle[EmitterData.MaximumNumberOfParticles];
			Set(particles);
		}

		protected void VerifyNumberOfParticlesDoesNotExceedMaximumAllowed()
		{
			if (EmitterData.MaximumNumberOfParticles > MaxParticles)
				//ncrunch: no coverage start
				throw new MaximumNumberOfParticlesExceeded(EmitterData.MaximumNumberOfParticles,
					MaxParticles);
		}

		public const int MaxParticles = 1024;

		public class MaximumNumberOfParticlesExceeded : Exception
		{
			public MaximumNumberOfParticlesExceeded(int specified, int maxAllowed)
				: base("Specified=" + specified + ", Maximum allowed=" + maxAllowed) {}
		} //ncrunch: no coverage end

		private void SpawnOneParticle(Vector3D position, float rotation)
		{
			int freeSpot = FindFreeSpot();
			if (freeSpot < 0)
				return;
			particles[freeSpot].IsActive = true;
			particles[freeSpot].ElapsedTime = 0;
			particles[freeSpot].Position = position;
			SetStartVelocityOfParticle(freeSpot);
			particles[freeSpot].Acceleration = EmitterData.Acceleration.Start;
			particles[freeSpot].Size = EmitterData.Size.Start;
			particles[freeSpot].Color = EmitterData.Color.Start;
			particles[freeSpot].CurrentUV = EmitterData.ParticleMaterial.SpriteSheet == null
				? Rectangle.One : EmitterData.ParticleMaterial.SpriteSheet.UVs[0];
			particles[freeSpot].Rotation = rotation;
			particles[freeSpot].Material = EmitterData.ParticleMaterial;
			if (EmitterData.DoParticlesTrackEmitter)
				particles[freeSpot].Rotation += Rotation.CalculateAxisAngle();
		}

		private void SetStartVelocityOfParticle(int newParticleIndex)
		{
			if (Rotation.Equals(Quaternion.Identity))
				particles[newParticleIndex].SetStartVelocityRandomizedFromRange(
					EmitterData.StartVelocity.Start, EmitterData.StartVelocity.End);
			else
				particles[newParticleIndex].SetStartVelocityRandomizedFromRange(
					EmitterData.StartVelocity.Start.Transform(Rotation),
					EmitterData.StartVelocity.End.Transform(Rotation));
		}

		private int FindFreeSpot()
		{
			for (int index = lastFreeSpot + 1; index < NumberOfActiveParticles; index++)
				if (!particles[index].IsActive)
					return lastFreeSpot = index; //ncrunch: no coverage
			if (NumberOfActiveParticles < EmitterData.MaximumNumberOfParticles)
			{
				lastFreeSpot = NumberOfActiveParticles;
				NumberOfActiveParticles++;
				return lastFreeSpot;
			}
			lastFreeSpot = NumberOfActiveParticles;
			return -1;
		}

		//ncrunch: no coverage start
		protected virtual Vector3D GetParticleSpawnPosition()
		{
			switch (EmitterData.PositionType)
			{
			case ParticleEmitterPositionType.Point:
				return GetSpawnPositionPoint();
			case ParticleEmitterPositionType.Line:
				return GetSpawnPositionLine();
			case ParticleEmitterPositionType.Box:
				return GetSpawnPositionBox();
			case ParticleEmitterPositionType.Sphere:
				return GetSpawnPositionSphere();
			case ParticleEmitterPositionType.SphereBorder:
				return GetSpawnPositionSphereBorder();
			case ParticleEmitterPositionType.CircleAroundCenter:
			case ParticleEmitterPositionType.CircleEscaping:
				return GetSpawnPositionCircleOutline();
			default:
				return GetSpawnPositionPoint();
			}
		}

		protected Vector3D GetSpawnPositionPoint()
		{
			return (Rotation.Equals(Quaternion.Identity))
				? Position + EmitterData.StartPosition.Start
				: Position + EmitterData.StartPosition.Start.Transform(Rotation);
		}

		protected Vector3D GetSpawnPositionLine()
		{
			return (Rotation.Equals(Quaternion.Identity))
				? Position + EmitterData.StartPosition.GetRandomValue()
				: Position + EmitterData.StartPosition.GetRandomValue().Transform(Rotation);
		}

		protected Vector3D GetSpawnPositionBox()
		{
			var insideTheBox = GetEmitterStartPositionLerped();
			return (Rotation.Equals(Quaternion.Identity))
				? Position + insideTheBox : Position + insideTheBox.Transform(Rotation);
		}

		private Vector3D GetEmitterStartPositionLerped()
		{
			return
				new Vector3D(
					EmitterData.StartPosition.Start.X.Lerp(EmitterData.StartPosition.End.X,
						Randomizer.Current.Get()),
					EmitterData.StartPosition.Start.Y.Lerp(EmitterData.StartPosition.End.Y,
						Randomizer.Current.Get()),
					EmitterData.StartPosition.Start.Z.Lerp(EmitterData.StartPosition.End.Z,
						Randomizer.Current.Get()));
		}

		protected Vector3D GetSpawnPositionSphere()
		{
			var insideSphere = GetEmitterStartPositionLerped();
			insideSphere.Normalize();
			insideSphere *=
				0.0f.Lerp(EmitterData.StartPosition.Start.Distance(EmitterData.StartPosition.End) * 0.5f,
					Randomizer.Current.Get());
			return Position + insideSphere;
		}

		protected Vector3D GetSpawnPositionSphereBorder()
		{
			var onSphereOutline = GetEmitterStartPositionLerped();
			onSphereOutline.Normalize();
			onSphereOutline *= EmitterData.StartPosition.Start.Distance(EmitterData.StartPosition.End) *
				0.5f;
			return Position + onSphereOutline;
		}

		private Vector3D GetSpawnPositionCircleOutline()
		{
			var startPosition = EmitterData.StartPosition;
			var onCircleOutline =
				new Vector3D(startPosition.Start.X + Randomizer.Current.Get(-1.0f) * startPosition.End.X,
					startPosition.Start.Y + Randomizer.Current.Get(-1.0f) * startPosition.End.Y, 0.0f);
			onCircleOutline.Normalize();
			var diameter = Math.Max(startPosition.Start.Length, startPosition.End.Length);
			onCircleOutline *= diameter * 0.5f;
			onCircleOutline = (Rotation.Equals(Quaternion.Identity))
				? Position + onCircleOutline : Position + onCircleOutline.Transform(Rotation);
			onCircleOutline.Z = startPosition.Start.Z.Lerp(startPosition.End.Z, Randomizer.Current.Get());
			return onCircleOutline;
		}

		public void SpawnAndDispose(int numberOfParticles = 1)
		{
			Spawn(numberOfParticles);
			DisposeAfterSeconds(EmitterData.LifeTime);
		}

		public void SpawnAtRelativePosition(Vector3D position, int numberOfParticles = 0)
		{
			if (numberOfParticles == 0)
				SpawnSomeParticlesAt(Position + position, (int)EmitterData.ParticlesPerSpawn.Start.Start);
			else
				SpawnSomeParticlesAt(Position + position, numberOfParticles);
		}

		public void SpawnAtRelativePositionAndRotation(Vector3D position, float rotation,
			int numberOfParticles = 0)
		{
			if (numberOfParticles == 0)
				SpawnSomeParticlesDefiningRotation(Position + position,
					(int)EmitterData.ParticlesPerSpawn.Start.Start, rotation);
			else
				SpawnSomeParticlesDefiningRotation(Position + position, numberOfParticles, rotation);
		}

		public void DisposeAfterSeconds(float remainingSeconds)
		{
			if (IsDisposing)
				return;
			Add(new Vector2D(remainingSeconds, 0));
			Start<SelfDestructTimer>();
			IsDisposing = true;
		}

		internal bool IsDisposing { get; private set; }

		internal class SelfDestructTimer : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (var entity in entities)
				{
					var duration = entity.Get<Vector2D>();
					duration.Y += Time.Delta;
					if (duration.Y > duration.X)
						entity.Dispose();
					entity.Set(duration);
				}
			}
		}
	}
}