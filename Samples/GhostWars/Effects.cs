using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering3D.Particles;

namespace GhostWars
{
	/// <summary>
	/// Collection of methods to create effects used in the GhostWars game.
	/// </summary>
	public static class Effects
	{
		public static Sprite CreateArrow(Vector2D start, Vector2D target)
		{
			var material = new Material(ShaderFlags.Position2DColoredTextured, "Arrow");
			var newSprite = new Sprite(material, CalculateArrowDrawArea(material, start, target));
			newSprite.Rotation = target.RotationTo(start);
			return newSprite;
		}

		private static Rectangle CalculateArrowDrawArea(Material material, Vector2D start, Vector2D target)
		{
			start += Vector2D.Normalize(start.DirectionTo(target)) * 0.033f;
			target -= Vector2D.Normalize(start.DirectionTo(target)) * 0.033f;
			var distance = start.DistanceTo(target);
			var size = new Size(distance, distance / material.DiffuseMap.PixelSize.AspectRatio);
			return Rectangle.FromCenter((start + target) / 2, size * GameLogic.ArrowSize);
		}

		public static ParticleEmitter CreateDeathEffect(Vector2D position)
		{
			var material = new Material(ShaderFlags.Position2DColoredTextured, "DeathSkull");
			var deathEffect = new ParticleEmitterData
			{
				ParticleMaterial = material,
				MaximumNumberOfParticles = 1,
				SpawnInterval = 0,
				Size = new RangeGraph<Size>(new Size(GameLogic.DeathSkullSize),
					new Size(GameLogic.DeathSkullSize-0.005f)),
				Acceleration = new RangeGraph<Vector3D>(new Vector2D(0, -0.04f), new Vector2D(0, -0.04f)),
				LifeTime = 2f,
				StartVelocity = new RangeGraph<Vector3D>(Vector2D.Zero, new Vector2D(0.01f, 0.01f))
			};
			return new ParticleEmitter(deathEffect, position);
		}

		public static ParticleEmitter CreateHitEffect(Vector2D position)
		{
			var material = new Material(ShaderFlags.Position2DColoredTextured, "Hit");
			var deathEffect = new ParticleEmitterData
			{
				ParticleMaterial = material,
				MaximumNumberOfParticles = 1,
				SpawnInterval = 0,
				Size = new RangeGraph<Size>(new Size(0.06f), new Size(0.09f)),
				LifeTime = 0.5f
			};
			return new ParticleEmitter(deathEffect, position);
		}

		public static ParticleEmitter CreateSparkleEffect(Team team, Vector2D position, int sparkles)
		{
			//creates too many particles, needs to be handled better and allow changing numbers
			return null;
		}
	}
}