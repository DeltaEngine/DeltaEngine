using System.Collections.Generic;
using DeltaEngine.Entities;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	internal class BehaviorTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void PrepareEntity()
		{
			entity = new MockEntityNoUpdateable();
		}

		private MockEntityNoUpdateable entity;

		[Test, CloseAfterFirstFrame]
		public void DeactivateAndActivateAgainExecutingTwice()
		{
			StartBehaviorAndAdvance();
			DeactivateEntityAndAdvance();
			ReactivateEntityAndAdvance();
			Assert.GreaterOrEqual(entity.Get<UpdateCounter>().Count, 2);
		}

		private void StartBehaviorAndAdvance()
		{
			entity.Start<CounterIncrement>();
			AdvanceTimeAndUpdateEntities();
		}

		private void StopBehaviorAndAdvance()
		{
			entity.Stop<CounterIncrement>();
			AdvanceTimeAndUpdateEntities();
		}

		private void DeactivateEntityAndAdvance()
		{
			entity.IsActive = false;
			AdvanceTimeAndUpdateEntities();
		}

		private void ReactivateEntityAndAdvance()
		{
			entity.IsActive = true;
			AdvanceTimeAndUpdateEntities();
		}

		[Test, CloseAfterFirstFrame]
		public void StopAndDeactivateToActivateAndStartUpdates()
		{
			StartBehaviorAndAdvance();
			StopBehaviorAndAdvance();
			DeactivateEntityAndAdvance();
			ReactivateEntityAndAdvance();
			StartBehaviorAndAdvance();
			Assert.AreEqual(2, entity.Get<UpdateCounter>().Count);
		}

		private class MockEntityNoUpdateable : Entity
		{
			public MockEntityNoUpdateable()
			{
				Add(new UpdateCounter { Count = 0 });
			}
		}

		private struct UpdateCounter
		{
			public int Count { get; set; }
		}

		private class CounterIncrement : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (Entity entity in entities)
				{
					var counter = entity.Get<UpdateCounter>();
					counter.Count ++;
					entity.Set(counter);
				}
			}
		}

		//ncrunch: no coverage start
		[Test, CloseAfterFirstFrame, Ignore]
		public void StopAndRestartBehaviorExecutingTwice()
		{
			StartBehaviorAndAdvance();
			StopBehaviorAndAdvance();
			StartBehaviorAndAdvance();
			Assert.AreEqual(2, entity.Get<UpdateCounter>().Count);
		}
	}
}