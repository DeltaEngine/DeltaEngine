using DeltaEngine.Platforms;
using NUnit.Framework;

namespace Blocks.Tests
{
	/// <summary>
	/// Unit test for FruitBlocksContent
	/// </summary>
	public class FruitBlocksContentTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void Constructor()
		{
			var content = new FruitBlocksContent();
			Assert.AreEqual("FruitBlocks_", content.Prefix);
			Assert.IsTrue(content.AreFiveBrickBlocksAllowed);
			Assert.IsTrue(content.DoBricksSplitInHalfWhenRowFull);
		}
	}
}