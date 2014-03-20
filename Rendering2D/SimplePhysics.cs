using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Rendering2D
{
	/// <summary>
	/// Groups various simple "physics" effects that can be applied to Entities.
	/// These do neither use actual physics functionality nor provide interaction to objects that do,
	/// but can prove to be a more efficient solution if just aiming for plain movement of sprites.
	/// </summary>
	public class SimplePhysics
	{
		/// <summary>
		/// Holds SimplePhysics related data
		/// </summary>
		public class Data
		{
			public float Elapsed { get; set; }
			public float Duration { get; set; }
			public float RotationSpeed { get; set; }
			public Vector2D Velocity { get; set; }
			public Vector2D UVVelocity { get; set; }
			public Vector2D Gravity { get; set; }
			public Action Bounced { get; set; }
		}

		/// <summary>
		/// Rotates an Entity2D every frame
		/// </summary>
		public class Rotate : UpdateBehavior
		{
			public Rotate()
				: base(Priority.First) {}

			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (Entity2D entity in entities)
					entity.Rotation += entity.Get<Data>().RotationSpeed * Time.Delta;
			}
		}

		/// <summary>
		/// Causes an Entity2D to move and fall under gravity.
		/// When the duration is complete it removes the Entity from the Entity System
		/// </summary>
		public class Move : UpdateBehavior
		{
			public Move()
				: base(Priority.First) {}

			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (Entity2D entity in entities)
					UpdatePhysics(entity, entity.Get<Data>());
			}

			private static void UpdatePhysics(Entity2D entity, Data physics)
			{
				physics.Velocity += physics.Gravity * Time.Delta;
				entity.Center += physics.Velocity * Time.Delta;
				physics.Elapsed += Time.Delta;
				if (physics.Duration > 0.0f && physics.Elapsed >= physics.Duration)
					entity.IsActive = false;
			}
		}

		/// <summary>
		/// Changes the velocity of an entity if it is at a screen edge.
		/// Is used in tandem with Move.
		/// </summary>
		public class BounceIfAtScreenEdge : UpdateBehavior
		{
			public BounceIfAtScreenEdge()
				: base(Priority.High) {}

			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (Entity2D entity in entities)
				{
					var physics = entity.Get<Data>();
					var velocity = physics.Velocity;
					physics.Velocity = physics.Velocity.ReflectIfHittingBorder(entity.DrawArea,
						ScreenSpace.Current.Viewport);
					entity.DrawArea = new Rectangle(entity.TopLeft + physics.Velocity * Time.Delta,
						entity.Size);
					if (physics.Velocity != velocity && entity.Get<Data>().Bounced != null)
						entity.Get<Data>().Bounced();
				}
			}
		}

		public class MoveUV : UpdateBehavior
		{
			public MoveUV()
				: base(Priority.First) {}

			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (Sprite sprite in entities)
					UpdatePhysics(sprite, sprite.Get<Data>().UVVelocity);
			}

			private static void UpdatePhysics(Sprite sprite, Vector2D velocity)
			{
				sprite.UV = sprite.UV.Move(velocity * Time.Delta);
			}
		}

		/// <summary>
		/// When the duration is complete it removes the Entity from the Entity System.
		/// </summary>
		public class KillAfterDurationReached : UpdateBehavior
		{
			public KillAfterDurationReached()
				: base(Priority.First) {}

			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (Entity2D entity in entities)
					UpdateDuration(entity, entity.Get<Data>());
			}

			private static void UpdateDuration(Entity2D entity, Data physics)
			{
				physics.Elapsed += Time.Delta;
				if (physics.Duration > 0.0f && physics.Elapsed >= physics.Duration)
					entity.Dispose();
			}
		}
	}
}