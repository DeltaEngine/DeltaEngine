using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class KeyboardMessagesTests
	{
		[SetUp]
		public void CreateKeyboardMessageWithParameter()
		{
			keyboardMessage = new KeyboardMessage(new[] { Key.Enter });
		}

		private KeyboardMessage keyboardMessage;

		[Test]
		public void CanCreateKeyboardMessageWithoutParameter()
		{
			keyboardMessage = new KeyboardMessage();
			Assert.NotNull(keyboardMessage);
		}

		[Test]
		public void PressedKeyShouldBeEnter()
		{
			Assert.AreEqual(new[] { Key.Enter }, keyboardMessage.PressedKeys);
		}
	}
}