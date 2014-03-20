using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class TextExtensionsTests
	{
		[Test]
		public void GetCountAndWordInPluralIfNeeded()
		{
			Assert.AreEqual("1 item", "item".GetCountAndWordInPluralIfNeeded(1));
			Assert.AreEqual("2 items", "item".GetCountAndWordInPluralIfNeeded(2));
			Assert.AreEqual("0 items", "item".GetCountAndWordInPluralIfNeeded(0));
		}
	}
}