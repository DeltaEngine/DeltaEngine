using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class TouchMessageTests
	{
		[SetUp]
		public void CreateTouchMessage()
		{
			touchMessage = new TouchMessage(new[] { Vector2D.Half }, new[] { true });
		}

		private TouchMessage touchMessage;

		[Test]
		public void VerifyTouchMessagePropertiesAreSet()
		{
			Assert.AreEqual(new []{Vector2D.Half}, touchMessage.Positions);
			Assert.AreEqual(new []{true}, touchMessage.PressedTouches);
		}
	}
}