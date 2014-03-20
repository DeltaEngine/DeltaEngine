using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.GameLogic.Tests
{
	public class TimeTriggerTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void TestTimeTrigger()
		{
			var actor = new MockActor(Vector3D.Zero, 0.0f);
			var data = new TimeTrigger.Data(Color.Red, Color.Green, 1.0f);
			actor.Add(data);
			actor.Start<TimeTrigger>();
			actor.Add(Color.Black);
			Assert.AreEqual(Color.Black, actor.Get<Color>());
			Assert.AreEqual(Color.Red, data.FirstColor);
			Assert.AreEqual(Color.Green, data.SecondColor);
			Assert.AreEqual(1.0f, data.Interval);
			AdvanceTimeAndUpdateEntities(1.1f);
			Assert.AreEqual(Color.Red, actor.Get<Color>());
		}
	}
}