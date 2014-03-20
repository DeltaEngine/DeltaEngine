using System.Collections.Generic;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D;

namespace DeltaNinja.Entities
{
	internal class TrajectoryEntityHandler : UpdateBehavior
	{
		public override void Update(IEnumerable<Entity> entities)
		{
			foreach (Entity2D entity in entities)
			{
				var sprite = entity as MovingSprite;
				if (sprite == null)
					return;
				if (sprite.IsPaused)
					return;
				MoveEntity(sprite);
			}
		}

		private static void MoveEntity(MovingSprite sprite)
		{
			var physics = sprite.Get<SimplePhysics.Data>();
			physics.Velocity += physics.Gravity * Time.Delta;
			sprite.Center += physics.Velocity * Time.Delta;
			sprite.Rotation += physics.RotationSpeed * Time.Delta;
			physics.Elapsed += Time.Delta;
			if (physics.Duration > 0.0f && physics.Elapsed >= physics.Duration)
				sprite.IsActive = false;
		}
	}
}