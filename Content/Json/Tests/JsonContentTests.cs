using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Content.Json.Tests
{
	public class JsonContentTests : TestWithMocksOrVisually
	{
		// ncrunch: no coverage start
		[Test, Category("Slow"), Ignore]
		public void LoadJsonContentFromFile()
		{
			var json = ContentLoader.Load<JsonContent>("Level");
			Assert.False(json.IsDisposed);
			Assert.AreEqual(9, json.Data.NumberOfNodes);
			Assert.AreEqual(48, json.Data.Get<int>("height"));
		}
	}
}