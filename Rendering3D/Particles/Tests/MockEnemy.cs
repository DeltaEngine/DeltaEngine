using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;

namespace DeltaEngine.Rendering3D.Particles.Tests
{
	public class MockEnemy : Billboard
	{
		public MockEnemy(Vector3D position, Size size, Material material)
			: base(position, size, material)
		{
			direction = Vector3D.UnitX;
			Start<EnemyUpdater>();
		}

		public Vector3D direction;

		private class EnemyUpdater : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (var entity in entities)
					if (entity is MockEnemy)
						UpdatePosition(entity);
			}

			private static void UpdatePosition(Entity entity)
			{
				var enemy = entity as MockEnemy;
				enemy.Position += enemy.direction * Time.Delta;
				if (enemy.Position.X.Abs() > 5)
					enemy.direction *= -1; //ncrunch: no coverage
			}
		}
	}
}
