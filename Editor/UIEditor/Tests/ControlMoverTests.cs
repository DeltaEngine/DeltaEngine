using DeltaEngine.Editor.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.UIEditor.Tests
{
	public class ControlMoverTests : TestWithMocksOrVisually
	{
		[Test]
		public void Line2DArrayShouldHaveFourElements()
		{
			var control = new ControlProcessor();
			Assert.AreEqual(0, control.Outlines.Count);
		}
	}
}