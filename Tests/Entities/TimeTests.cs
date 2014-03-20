using DeltaEngine.Entities;
using DeltaEngine.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Tests.Entities
{
	public class TimeTests
	{
		[Test]
		public void CheckEveryIsFalseIfItDidNotCrossTheIntervalThisFrame()
		{
			Time.Total = 0.4f;
			Time.Delta = 0.2f;
			Assert.IsFalse(Time.CheckEvery(0.5f));
		}

		[Test]
		public void CheckEveryIsTrueIfItDidCrossTheIntervalThisFrame()
		{
			Time.Total = 0.6f;
			Time.Delta = 0.2f;
			Assert.IsTrue(Time.CheckEvery(0.5f));
		}

		[Test]
		public void PauseTimeShouldNotUpdateAnyEntityAnymore()
		{
			var entities = new MockEntitiesRunner(typeof(EntitiesRunnerTests.IncrementCounter));
			var entity = new MockEntity().Add(0).Start<EntitiesRunnerTests.IncrementCounter>();
			Assert.AreEqual(0, entity.Get<int>());
			entities.RunEntities();
			Assert.AreEqual(1, entity.Get<int>());
			Time.SpeedFactor = 0;
			entities.RunEntities();
			Assert.AreEqual(1, entity.Get<int>());
			Time.SpeedFactor = 1;
		}
	}
}