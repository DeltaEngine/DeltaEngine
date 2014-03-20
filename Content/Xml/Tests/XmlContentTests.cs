using DeltaEngine.Content.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Content.Xml.Tests
{
	/// <summary>
	/// Confirms XmlContent can load from file
	/// </summary>
	public class XmlContentTests
	{
		[SetUp]
		public void CreateMockContentLoader()
		{
			ContentLoader.Use<MockContentLoader>();
		}

		[TearDown]
		public void DisposeMockContentLoader()
		{
			ContentLoader.DisposeIfInitialized();
		}

		[Test]
		public void LoadXmlContentFromMock()
		{
			var xmlContent = ContentLoader.Load<XmlContent>("Test");
			Assert.False(xmlContent.IsDisposed);
			Assert.AreEqual("Root", xmlContent.Data.Name);
			Assert.AreEqual(1, xmlContent.Data.Children.Count);
			Assert.AreEqual("Hi", xmlContent.Data.Children[0].Name);
			xmlContent.Dispose();
		}

		[Test]
		public void LoadXmlContentFromNonExistingFile()
		{
			var xmlContent = ContentLoader.Load<XmlContent>("NonExisting");
			Assert.AreEqual("NonExisting", xmlContent.Data.Name);
		}

		//ncrunch: no coverage start
		[Test, Category("Slow"), Ignore]
		public void LoadXmlContentFromFile()
		{
			var xmlContent = ContentLoader.Load<XmlContent>("Test");
			Assert.False(xmlContent.IsDisposed);
			Assert.AreEqual("Root", xmlContent.Data.Name);
			Assert.AreEqual(1, xmlContent.Data.Children.Count);
			Assert.AreEqual("Hi", xmlContent.Data.Children[0].Name);
		}
	}
}