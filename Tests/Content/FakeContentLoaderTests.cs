using System.Diagnostics;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Content.Xml;
using NUnit.Framework;

namespace DeltaEngine.Tests.Content
{
	public class FakeContentLoaderTests
	{
		[SetUp]
		public void CreateContentLoader()
		{
			ContentLoader.Use<FakeContentLoader>();
		}

		[TearDown]
		public void DisposeContentLoader()
		{
			ContentLoader.DisposeIfInitialized();
		}

		[Test]
		public void ContentLoadWithNullStream()
		{
			ContentLoader.Load<DynamicXmlMockContent>("XmlContentWithNoPath");
		}

		[Test]
		public void ContentLoadWithWrongFilePath()
		{
			Assert.Throws<ContentLoader.ContentFileDoesNotExist>(
				() => ContentLoader.Load<XmlContent>("ContentWithWrongPath"));
		}

		[Test]
		public void ThrowExceptionIfSecondContentLoaderInstanceIsUsed()
		{
			ContentLoader.Exists("abc");
			Assert.Throws<ContentLoader.ContentLoaderAlreadyExistsItIsOnlyAllowedToSetBeforeTheAppStarts>(
				ContentLoader.Use<FakeContentLoader>);
		}

		[Test]
		public void LoadDefaultDataIfAllowed()
		{
			ContentLoader.Load<DynamicXmlMockContent>("UnavailableDynamicContent");
		}

		private class DynamicXmlMockContent : ContentData
		{
			public DynamicXmlMockContent(string contentName)
				: base(contentName) {}

			protected override void DisposeData() {}
			protected override void LoadData(Stream fileData) {}
			protected override bool AllowCreationIfContentNotFound
			{
				get { return true; }
			}
		}

		[Test]
		public void CreateDataIfAllowedAndNoContentFoundAndDispose()
		{
			var dummy = ContentLoader.Load<DynamicXmlMockContent>("Dummy");
			Assert.IsTrue(dummy.InternalAllowCreationIfContentNotFound);
			Assert.IsFalse(dummy.IsDisposed);
			dummy.Dispose();
			Assert.IsTrue(dummy.IsDisposed);
		}
	}
}