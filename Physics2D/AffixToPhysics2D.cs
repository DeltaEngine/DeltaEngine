using System.Collections.Generic;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Physics2D
{
	/// <summary>
	/// Updates an Entity2Ds position and rotation from the associated PhysicsBody
	/// </summary>
	public class AffixToPhysics2D : UpdateBehavior
	{
		public AffixToPhysics2D()
			: base(Priority.Last) { }

		public override void Update(IEnumerable<Entity> entities)
		{
			foreach (Entity2D entity in entities)
				UpdateCenterAndRotation(entity, entity.Get<PhysicsBody>());
		}

		private static void UpdateCenterAndRotation(Entity2D entity, PhysicsBody body)
		{
			entity.Center = body.Position;
			entity.Rotation = body.Rotation;
		}
	}
}