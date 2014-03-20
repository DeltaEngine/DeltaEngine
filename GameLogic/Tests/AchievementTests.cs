using NUnit.Framework;

namespace DeltaEngine.GameLogic.Tests
{
	public class AchievementTests
	{
		[Test]
		public void TestAchievement()
		{
			var achievement = new Achievement(Name, Description);
			Assert.AreEqual(Name, achievement.Name);
			Assert.AreEqual(Description, achievement.Description);
			Assert.IsFalse(achievement.IsAchieved);
		}

		private const string Name = "TestAchievement";
		private const string Description = "That is a test achievement";
	}
}
