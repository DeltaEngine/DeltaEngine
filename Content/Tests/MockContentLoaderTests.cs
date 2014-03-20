using System;
using System.IO;
using DeltaEngine.Content.Mocks;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Content.Tests
{
	public class MockContentLoaderTests
	{
		[SetUp]
		public void CreateContentLoaderInstanceAndTestContent()
		{
			Assert.Throws<ContentLoader.ContentLoaderUseWasNotCalled>(
				() => ContentLoader.Load<MockXmlContent>(TestXmlContentName));
			ContentLoader.Use<MockContentLoader>();
			testXmlContent = ContentLoader.Load<MockXmlContent>(TestXmlContentName);
		}

		[TearDown]
		public void DisposeContentLoader()
		{
			ContentLoader.DisposeIfInitialized();
		}

		private MockXmlContent testXmlContent;
		private const string TestXmlContentName = "FakeXml";

		[Test]
		public void ContentDataToString()
		{
			string contentDataString = testXmlContent.ToString();
			Assert.IsTrue(contentDataString.Contains(TestXmlContentName), contentDataString);
		}

		[Test]
		public void AllowCreationIfContentNotFound()
		{
			Assert.IsFalse(testXmlContent.InternalAllowCreationIfContentNotFound);
		}

		[Test]
		public void InternalCreateDefault()
		{
			var originalData = testXmlContent.Data;
			testXmlContent.InternalCreateDefault();
			Assert.AreEqual(TestXmlContentName, testXmlContent.Name);
			Assert.AreNotEqual(originalData, testXmlContent.Data);
		}

		[Test]
		public void CheckForContentOnStartup()
		{
			Assert.IsTrue(ContentLoader.HasValidContentForStartup());
		}

		[Test]
		public void ValidPath()
		{
			var contentLoader = new MockContentLoader();
			Assert.AreEqual(Path.Combine(contentLoader.ContentProjectPath, "ContentMetaData.xml"),
				contentLoader.GetContentMetaDataFilePath());
		}

		[Test]
		public void LoadContent()
		{
			Assert.AreEqual(TestXmlContentName, testXmlContent.Name);
			Assert.IsFalse(testXmlContent.IsDisposed);
			Assert.Greater(testXmlContent.LoadCounter, 0);
		}

		[Test]
		public void LoadGeneratedContent()
		{
			Assert.DoesNotThrow(() => ContentLoader.Load<MockXmlContent>("<GeneratedMockXml>"));
		}

		[Test]
		public void LoadWithExtensionNotAllowed()
		{
			Assert.Throws<ContentLoader.ContentNameShouldNotHaveExtension>(
				() => ContentLoader.Load<MockXmlContent>("Test.xml"));
		}

		[Test]
		public void CheckIfContentExists()
		{
			Assert.IsTrue(ContentLoader.Exists(TestXmlContentName));
			Assert.IsTrue(ContentLoader.Exists(TestXmlContentName, ContentType.Xml));
			Assert.IsFalse(ContentLoader.Exists(TestXmlContentName, ContentType.Camera));
		}

		[Test]
		public void LoadCachedContent()
		{
			var contentTwo = ContentLoader.Load<MockXmlContent>(TestXmlContentName);
			Assert.IsFalse(contentTwo.IsDisposed);
			Assert.AreEqual(testXmlContent, contentTwo);
		}

		[Test]
		public void RemoveCachedContent()
		{
			Assert.DoesNotThrow(() => ContentLoader.RemoveResource(TestXmlContentName));
		}

		[Test]
		public void ForceReload()
		{
			ContentLoader.ReloadContent(TestXmlContentName);
			Assert.Greater(testXmlContent.LoadCounter, 1);
			Assert.AreEqual(1, testXmlContent.changeCounter);
		}

		[Test]
		public void TwoContentFilesShouldNotReloadEachOther()
		{
			var content2 = ContentLoader.Load<MockXmlContent>(TestXmlContentName + 2);
			ContentLoader.ReloadContent(TestXmlContentName);
			Assert.AreEqual(1, content2.LoadCounter);
			Assert.AreEqual(2, testXmlContent.LoadCounter);
		}

		[Test]
		public void DisposeContent()
		{
			Assert.IsFalse(testXmlContent.IsDisposed);
			testXmlContent.Dispose();
			Assert.IsTrue(testXmlContent.IsDisposed);
		}

		[Test]
		public void CheckContentLocale()
		{
			string contentLocale = ContentLoader.ContentLocale;
			Assert.IsNotEmpty(contentLocale);
			ContentLoader.ContentLocale = null;
			ContentLoader.ContentLocale = contentLocale;
		}

		[Test]
		public void DisposeAndLoadAgainShouldReturnFreshInstance()
		{
			testXmlContent.Dispose();
			var freshContent = ContentLoader.Load<MockXmlContent>(TestXmlContentName);
			Assert.IsFalse(freshContent.IsDisposed);
		}

		[Test]
		public void LoadWithoutContentNameIsNotAllowed()
		{
			Assert.Throws<ContentData.ContentNameMissing>(() => new MockXmlContent(""));
		}

		[Test]
		public void ThrowExceptionIfContentNameDoesNotMatchToContentType()
		{
			Assert.Throws<ContentLoader.CachedResourceExistsButIsOfTheWrongType>(
				() => ContentLoader.Load<MockImage>(TestXmlContentName));
		}

		[Test]
		public void ThrowExceptionOnLoadWithWrongMetaData()
		{
			const ContentType WrongMetaDataType = ContentType.Video;
			Assert.AreNotEqual(testXmlContent.MetaData.Type, WrongMetaDataType);
			testXmlContent.MetaData.Type = WrongMetaDataType;
			Func<ContentData, Stream> fakeGetDataStreamMethod = data => Stream.Null;
#if DEBUG
			Assert.Throws<ContentData.DoesNotMatchMetaDataType>(
				() => testXmlContent.InternalLoad(fakeGetDataStreamMethod));
#endif
		}

		[Test]
		public void LoadAnimationContentViaCreationData()
		{
			var image = ContentLoader.Load<MockImage>("MockImage");
			var animationData = new SpriteSheetAnimationCreationData(image, 1.0f, new Size(1, 1));
			var animation = ContentLoader.Create<SpriteSheetAnimation>(animationData);
			Assert.AreEqual(animationData.Image, animation.Image);
			Assert.AreEqual(animationData.DefaultDuration, animation.DefaultDuration);
			Assert.AreEqual(animationData.SubImageSize, animation.SubImageSize);
		}

		[Test]
		public void ThrowExceptionIfLoadDefaultDataIsNotAllowed()
		{
			Assert.Throws<ContentLoader.ContentNotFound>(
				() => ContentLoader.Load<MockXmlContent>("UnavailableXmlContent"));
		}

#if DEBUG
		//ncrunch: no coverage start
		[Test, Category("Slow")]
		public void ExceptionOnInstancingFromOutsideContentLoader()
		{
			if (!StackTraceExtensions.StartedFromNCrunchOrNunitConsole)
				Assert.Throws<ContentData.MustBeCalledFromContentLoader>(
					() => new MockXmlContent("VectorText"));
		}
#endif
	}
}