using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class MouseMessageTests
	{
		[SetUp]
		public void CreateMouseMessage()
		{
			mouseMessage = new MouseMessage(Vector2D.Half, 0, new[] { MouseButton.Left });
		}

		private MouseMessage mouseMessage;

		[Test]
		public void VerifyMouseMessagePropertiesAreSet()
		{
			Assert.AreEqual(Vector2D.Half, mouseMessage.Position);
			Assert.AreEqual(0, mouseMessage.ScrollWheel);
			Assert.AreEqual(new[] { MouseButton.Left }, mouseMessage.PressedButtons);
		}
	}
}