using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class RemoteInputMessagesTests
	{
		[Test]
		public void CreateKeyboardMessage()
		{
			var emptyMessage = new KeyboardMessage();
			Assert.IsNull(emptyMessage.PressedKeys);
			var message = new KeyboardMessage(new Key[0]);
			Assert.AreEqual(0, message.PressedKeys.Length);
		}

		[Test]
		public void CreateTouchMessage()
		{
			var message = new TouchMessage(new Vector2D[0], new bool[0]);
			Assert.AreEqual(0, message.Positions.Length);
			Assert.AreEqual(0, message.PressedTouches.Length);
		}
	}
}