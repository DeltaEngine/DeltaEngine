using System.IO;
using System.Reflection;
using DeltaEngine.Content.Mocks;
using DeltaEngine.Core;
using NUnit.Framework;

namespace DeltaEngine.Content.Tests
{
	public class BinaryDataWithContentLoaderTests
	{
		[Test]
		public void TestLoadContentType()
		{
			ContentLoader.Use<MockContentLoader>();
			const string ContentName = "SomeXml";
			var instance = new ObjectWithContent(ContentLoader.Load<MockXmlContent>(ContentName));
			var stream = BinaryDataExtensions.SaveDataIntoMemoryStream(instance);
			var loadedInstance =
				BinaryDataExtensions.LoadDataWithKnownTypeFromMemoryStream<ObjectWithContent>(stream);
			Assert.AreEqual(instance.xmlContent, loadedInstance.xmlContent);
			ContentLoader.DisposeIfInitialized();
		}

		private class ObjectWithContent
		{
			private ObjectWithContent() {}

			public ObjectWithContent(MockXmlContent xmlContent)
			{
				this.xmlContent = xmlContent;
			}

			public readonly MockXmlContent xmlContent;
		}

		[Test]
		public void LoadContentWithoutNameShouldThrowUnableToLoadContentDataWithoutName()
		{
			var stream = new MemoryStream();
			var writer = new BinaryWriter(stream);
			writer.Write(true);
			writer.Write(string.Empty);
			ContentLoader.Use<MockContentLoader>();
			stream.Position = 0;
			var reader = new BinaryReader(stream);
			var version = Assembly.GetExecutingAssembly().GetName().Version;
			Assert.That(
				() => BinaryDataLoader.CreateAndLoad(typeof(MockXmlContent), reader, version),
				Throws.Exception.With.InnerException.TypeOf
					<BinaryDataLoader.UnableToLoadContentDataWithoutName>());
			ContentLoader.DisposeIfInitialized();
		}
	}
}