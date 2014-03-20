using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DeltaEngine.Content.Mocks;
using DeltaEngine.Content.Xml;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Content.Disk.Tests
{
	/// <summary>
	/// ContentMetaData.xml does not exist on purpose here, it is created automatically.
	/// </summary>
	[Category("Slow")]
	public class DiskContentLoaderTests
	{
		//ncrunch: no coverage start
		[TestFixtureSetUp]
		public void Setup()
		{
			CreateContentMetaDataAndRealFiles();
			ContentLoader.Use<DiskContentLoader>();
			image = ContentLoader.Load<MockImage>("DeltaEngineLogo");
		}

		private Image image;

		private static void CreateContentMetaDataAndRealFiles()
		{
			Directory.CreateDirectory(ContentProjectDirectoryName);
			var root = new XmlData("ContentMetaData");
			root.AddAttribute("Name", "DeltaEngine.Content.Disk.Tests");
			root.AddAttribute("Type", "Scene");
			root.AddChild(CreateImageEntryAndFile("DeltaEngineLogo", new Size(128, 128)));
			root.AddChild(CreateImageEntryAndFile("SmallImage", new Size(32, 32)));
			root.AddChild(CreateAnimationNode());
			root.AddChild(CreateXmlEntryAndFile());
			var contentMetaData = new XmlFile(root);
			contentMetaData.Save(Path.Combine(ContentProjectDirectoryName, "ContentMetaData.xml"));
		}

		private const string ContentProjectDirectoryName = @"Content\DeltaEngine.Content.Disk.Tests";

		private static XmlData CreateImageEntryAndFile(string name, Size pixelSize)
		{
			var image = new XmlData("ContentMetaData");
			image.AddAttribute("Name", name);
			image.AddAttribute("Type", "Image");
			string filename = name + ".png";
			string filePath = Path.Combine(ContentProjectDirectoryName, filename);
			ContentDiskTestsExtensions.CreateImageAndContentMetaData(filePath, pixelSize, image);
			image.AddAttribute("LocalFilePath", filename);
			image.AddAttribute("LastTimeUpdated", DateTime.Now);
			image.AddAttribute("PlatformFileId", --platformFileId);
			return image;
		}

		private static int platformFileId;

		private static XmlData CreateAnimationNode()
		{
			var animation = new XmlData("ContentMetaData");
			animation.AddAttribute("Name", "TestAnimation");
			animation.AddAttribute("Type", "ImageAnimation");
			var frame1 = CreateImageEntryAndFile("ImageAnimation01", new Size(64, 64));
			var frame2 = CreateImageEntryAndFile("ImageAnimation02", new Size(64, 64));
			return animation.AddChild(frame1).AddChild(frame2);
		}

		private static XmlData CreateXmlEntryAndFile()
		{
			var xml = new XmlData("ContentMetaData");
			xml.AddAttribute("Name", "Test").AddAttribute("Type", "Xml");
			const string Filename = "Test.xml";
			using (var textWriter = File.CreateText(Path.Combine(ContentProjectDirectoryName, Filename)))
				textWriter.WriteLine("<Test></Test>");
			xml.AddAttribute("LocalFilePath", Filename);
			xml.AddAttribute("LastTimeUpdated", DateTime.Now);
			xml.AddAttribute("PlatformFileId", --platformFileId);
			return xml;
		}

		[TestFixtureTearDown]
		public void DisposeContentLoaderAndDeleteContentDirectory()
		{
			ContentLoader.DisposeIfInitialized();
			DeleteDirectoryAndAllIncludingFiles(ContentProjectDirectoryName);
		}

		private static void DeleteDirectoryAndAllIncludingFiles(string contentDirectoryName)
		{
			if (!Directory.Exists(contentDirectoryName))
				return;
			foreach (var file in Directory.GetFiles(contentDirectoryName))
				File.Delete(file);
			Directory.Delete(contentDirectoryName);
		}

		[TearDown]
		public void DeleteExtraContentDirectory()
		{
			DeleteDirectoryAndAllIncludingFiles(ExtraContentDirectoryName);
		}

		private const string ExtraContentDirectoryName = "ContentDirectory";

		[Test]
		public void LoadImageContent()
		{
			Assert.AreEqual("DeltaEngineLogo", image.Name);
			Assert.IsFalse(image.IsDisposed);
			Assert.AreEqual(new Size(128, 128), image.PixelSize);
			var smallImage = ContentLoader.Load<MockImage>("SmallImage");
			Assert.AreEqual(new Size(32, 32), smallImage.PixelSize);
		}

		[Test]
		public void LoadNonExistingImageFails()
		{
			if (Debugger.IsAttached)
				Assert.Throws<ContentLoader.ContentNotFound>(
					() => ContentLoader.Load<MockImage>("FailImage"));
		}

		[Test]
		public void LoadingCachedContentOfTheWrongTypeThrowsException()
		{
			Assert.DoesNotThrow(() => ContentLoader.Load<MockImage>("DeltaEngineLogo"));
			Assert.Throws<ContentLoader.CachedResourceExistsButIsOfTheWrongType>(
				() => ContentLoader.Load<MockXmlContent>("DeltaEngineLogo"));
		}

		[Test]
		public void LastTimeUpdatedShouldBeSet()
		{
			Assert.Greater(image.MetaData.LastTimeUpdated, DateTime.Now.AddSeconds(-2));
		}

		[Test]
		public void PlatformFileIdShouldBeSet()
		{
			Assert.Less(image.MetaData.PlatformFileId, 0);
		}

		[Test]
		public void FileSizeShouldBeSet()
		{
			Assert.Greater(image.MetaData.FileSize, 150);
		}

		[Test]
		public void ShouldCreateMetaDataFileIfNoneExists()
		{
			Directory.CreateDirectory(ExtraContentDirectoryName);
			var files = Directory.GetFiles(ContentProjectDirectoryName, "*.png");
			File.Copy(files[0], Path.Combine(ExtraContentDirectoryName, Path.GetFileName(files[0])));
			string metaDataFilePath = Path.Combine(ExtraContentDirectoryName, "ContentMetaData.xml");
			var contentLoader = (DiskContentLoader)ContentLoader.current;
			contentLoader.LoadMetaData(metaDataFilePath);
			Assert.IsTrue(File.Exists(metaDataFilePath));
		}

		[Test]
		public void LoadingContentDataFromBinaryDataWillLoadItFromName()
		{
			var content = new ImageAnimation(new[] { ContentLoader.Load<Image>("DeltaEngineLogo") }, 1);
			var data = BinaryDataExtensions.SaveDataIntoMemoryStream(content);
			Assert.AreEqual(63, data.Length);
			var loadedContent =
				BinaryDataExtensions.LoadDataWithKnownTypeFromMemoryStream<ImageAnimation>(data);
			Assert.AreEqual(content.Name, loadedContent.Name);
			Assert.AreEqual(content.Frames.Length, loadedContent.Frames.Length);
			Assert.AreEqual(content.Frames[0], loadedContent.Frames[0]);
		}

		[Test]
		public void CreateMetaDataViaFileCreator()
		{
			Directory.CreateDirectory(ExtraContentDirectoryName);
			foreach (var filePath in Directory.GetFiles(ContentProjectDirectoryName, "*.png"))
				File.Copy(filePath, Path.Combine(ExtraContentDirectoryName, Path.GetFileName(filePath)));
			string metaDataFilePath = Path.Combine(ExtraContentDirectoryName, "ContentMetaData.xml");
			var xml = CreateMetaDataXmlFileAndCheckPixelSizes(null, metaDataFilePath);
			CreateMetaDataXmlFileAndCheckPixelSizes(xml, metaDataFilePath);
		}

		private static XDocument CreateMetaDataXmlFileAndCheckPixelSizes(XDocument lastXml,
			string metaDataFilePath)
		{
			var xml = new ContentMetaDataFileCreator(lastXml).CreateAndLoad(metaDataFilePath);
			Assert.AreEqual(5, xml.Root.Elements().Count());
			foreach (var element in xml.Root.Elements())
				CheckElementPixelSize(element);
			return xml;
		}

		private static void CheckElementPixelSize(XElement element)
		{
			var expectedPixelSize = "1, 1";
			if (element.Attribute("Name").Value == "DeltaEngineLogo")
				expectedPixelSize = CheckBlendModeAndGetPixelSizeForDeltaEngineLogo(element);
			else if (element.Attribute("Name").Value == "SmallImage")
				expectedPixelSize = CheckBlendModeAndGetPixelSizeForSmallImage(element);
			else if (element.Attribute("Name").Value.Contains("ImageAnimation"))
				expectedPixelSize = "64, 64";
			if (element.Attribute("Type").Value == "Image")
				Assert.AreEqual(expectedPixelSize, element.Attribute("PixelSize").Value);
		}

		// ReSharper disable once UnusedParameter.Local
		private static string CheckBlendModeAndGetPixelSizeForDeltaEngineLogo(XElement element)
		{
			Assert.IsNull(element.Attribute("BlendMode"));
			return "128, 128";
		}

		private static string CheckBlendModeAndGetPixelSizeForSmallImage(XElement element)
		{
			Assert.AreEqual("Opaque", element.Attribute("BlendMode").Value);
			return "32, 32";
		}
	}
}