using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class TouchMovementTriggerTests : TestWithMocksOrVisually
	{
		[Test]
		public void CreateWithString()
		{
			Assert.DoesNotThrow(() => new TouchMovementTrigger(""));
			Assert.Throws<TouchMovementTrigger.TouchMovementTriggerHasNoParameters>(
				() => new TouchMovementTrigger("a"));
		}
	}
}