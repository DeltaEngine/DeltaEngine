using DeltaEngine.Platforms;
using NUnit.Framework;

namespace Blocks.Tests
{
	public class UserInterfaceTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			content = new JewelBlocksContent();
			userInterface = new UserInterface(content);
		}

		private JewelBlocksContent content;
		private UserInterface userInterface;

		[Test, CloseAfterFirstFrame]
		public void UserInterfaceShouldChangeFromLandscapeToPortrait()
		{
			userInterface.ShowUserInterfacePortrait();
		}

		[Test, CloseAfterFirstFrame]
		public void OnLoseScoreShouldBeZero()
		{
			userInterface.Lose();
			Assert.AreEqual(0, userInterface.Score);
		}

		[Test, CloseAfterFirstFrame]
		public void UserInterfaceShouldBeHidden()
		{
			userInterface.Dispose();
		}
	}
}