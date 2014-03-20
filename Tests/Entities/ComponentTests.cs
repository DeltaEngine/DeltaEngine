using System.Collections.Generic;
using DeltaEngine.Entities;
using DeltaEngine.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Tests.Entities
{
	public class ComponentTests
	{
		[Test]
		public void CannontCreateEntityWithoutRunner()
		{
			if (EntitiesRunner.Current != null)
				EntitiesRunner.Current.Dispose();
			Assert.Throws<Entity.UnableToCreateEntityWithoutInitializedResolverAndEntitiesRunner>(
				() => new MockEntity());
		}

		[Test]
		public void CreateEntityWithRotationComponent()
		{
			var entities = new MockEntitiesRunner(typeof(Rotate));
			var entity = new MockEntity().Add(0.5f).Start<Rotate>();
			Assert.AreEqual(0.5f, entity.Get<float>());
			entities.RunEntities();
			Assert.AreEqual(0.55f, entity.Get<float>());
		}

		public class Rotate : UpdateBehavior
		{
			public Rotate()
				: base(Priority.First) {}

			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (var entity in entities)
					entity.Set(entity.Get<float>() + Time.Delta);
			}
		}

		[Test]
		public void CanCheckEntityHandlersPriority()
		{
			EntitiesRunner entities = new MockEntitiesRunner(typeof(Rotate));
			Assert.AreEqual(Priority.First, entities.GetUpdateBehavior<Rotate>().priority);
		}
	}
}