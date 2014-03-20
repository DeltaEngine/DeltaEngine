using System.Collections.Generic;
using DeltaEngine.Entities;
using DeltaEngine.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Tests.Entities
{
	internal class CreatingEntitiesTests
	{
		[SetUp]
		public void CreateSystem()
		{
			entities = new MockEntitiesRunner(typeof(EntityCreator), typeof(EntitiesRunnerTests.IncrementCounter),
				typeof(EntityCreatorInDraw));
		}

		private MockEntitiesRunner entities;

		[Test]
		public void CreateEntitiesInUpdateBehavior()
		{
			new MockEntity().Start<EntityCreator>();
			Assert.AreEqual(1, EntitiesRunner.Current.NumberOfEntities);
			entities.RunEntities();
			Assert.AreEqual(2, EntitiesRunner.Current.NumberOfEntities);
			entities.RunEntities();
			Assert.AreEqual(3, EntitiesRunner.Current.NumberOfEntities);
		}

		public class EntityCreator : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				new MockEntity().Start<EntitiesRunnerTests.IncrementCounter>().Add(0);
			}
		}

		[Test]
		public void CreateEntitiesInDrawBehaviorShouldFail()
		{
			new MockDrawableEntity().OnDraw<EntityCreatorInDraw>();
			Assert.AreEqual(1, EntitiesRunner.Current.NumberOfEntities);
			entities.RunEntities();
		}

		public class EntityCreatorInDraw : DrawBehavior
		{
			public void Draw(List<DrawableEntity> visibleEntities)
			{
				Assert.Throws<EntitiesRunner.YouAreNotAllowedToSetAnEntityComponentInsideTheDrawLoop>(
					() => new MockEntity().Start<EntitiesRunnerTests.IncrementCounter>().Add(0));
			}
		}
	}
}