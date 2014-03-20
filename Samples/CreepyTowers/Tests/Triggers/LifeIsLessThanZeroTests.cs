using CreepyTowers.Triggers;
using DeltaEngine.Entities;
using DeltaEngine.GameLogic;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using NUnit.Framework;

namespace CreepyTowers.Tests.Triggers
{
	public class LifeIsLessThanZeroTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void TestLifeIsLessThanZero()
		{
			new LifeIsLessThanZero("0");
			GameTrigger.OnGameOver();
			var list = EntitiesRunner.Current.GetEntitiesOfType<FontText>();
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("Game Over", list[0].Text);
		}
	}
}
