using DeltaEngine.Platforms;
using NUnit.Framework;

namespace GameOfDeath.Tests
{
	internal class UserInterfaceTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateInitialUserInterface()
		{
			userInterface = new UserInterface();
		}

		private UserInterface userInterface;

		[Test]
		public void SettingKillsSetsTextOfCounter()
		{
			userInterface.Kills = 10;
			Assert.AreEqual(10, userInterface.Kills);
			Assert.AreEqual("10", userInterface.KillText.Text);
		}
	}
}