using System;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering2D
{
	/// <summary>
	/// Simplifies applying simple physics to an entity
	/// </summary>
	public static class Entity2DExtensions
	{
		public static void StartRotating(this Entity2D entity, float rotationSpeed)
		{
			AddSimplePhysicsDataIfNeeded(entity);
			entity.Get<SimplePhysics.Data>().RotationSpeed = rotationSpeed;
			entity.Start<SimplePhysics.Rotate>();
		}

		private static void AddSimplePhysicsDataIfNeeded(Entity2D entity)
		{
			if (!entity.Contains<SimplePhysics.Data>())
				entity.Add(new SimplePhysics.Data());
		}

		public static void StartMoving(this Entity2D entity, Vector2D velocity)
		{
			AddSimplePhysicsDataIfNeeded(entity);
			entity.Get<SimplePhysics.Data>().Velocity = velocity;
			entity.Start<SimplePhysics.Move>();
		}

		public static void StartFalling(this Entity2D entity, Vector2D velocity, Vector2D gravity)
		{
			AddSimplePhysicsDataIfNeeded(entity);
			entity.Get<SimplePhysics.Data>().Velocity = velocity;
			entity.Get<SimplePhysics.Data>().Gravity = gravity;
			entity.Start<SimplePhysics.Move>();
		}

		public static void StartMovingUV(this Sprite sprite, Vector2D velocity)
		{
			AddSimplePhysicsDataIfNeeded(sprite);
			sprite.Get<SimplePhysics.Data>().UVVelocity = velocity;
			sprite.Start<SimplePhysics.MoveUV>();
		}

		public static void StartBouncingOffScreenEdges(this Entity2D entity, Vector2D velocity,
			Action bounced)
		{
			AddSimplePhysicsDataIfNeeded(entity);
			entity.Get<SimplePhysics.Data>().Velocity = velocity;
			entity.Get<SimplePhysics.Data>().Bounced = bounced;
			entity.Start<SimplePhysics.Move>();
			entity.Start<SimplePhysics.BounceIfAtScreenEdge>();
		}
	}
}