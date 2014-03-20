using System.Collections.Generic;
using DeltaEngine.Entities;
using DeltaEngine.Rendering3D;

namespace DeltaEngine.Physics3D
{
	public class AffixToPhysics3D : UpdateBehavior
	{
		public AffixToPhysics3D() : base(Priority.Last) { }

		public override void Update(IEnumerable<Entity> entities)
		{
			foreach (Entity3D entity in entities)
				UpdatePositionAndOrientation(entity, entity.Get<PhysicsBody>());
		}

		private static void UpdatePositionAndOrientation(Entity3D entity, PhysicsBody physicsBody)
		{
			entity.Position = physicsBody.Position;
			entity.Orientation = physicsBody.GetOrientation();
		}
	}
}
