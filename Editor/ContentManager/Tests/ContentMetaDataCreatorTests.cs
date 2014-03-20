using System;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests
{
	[Ignore]
	public class ContentMetaDataCreatorTests : TestWithMocksOrVisually
	{
		//ncrunch: no coverage start
		[Test, CloseAfterFirstFrame]
		public void CreateContentMetaDataFromImageFileToBeUploadedToTheOnlineService()
		{
			var creator = new ContentMetaDataCreator();
			var filePath = Path.Combine("Content", "DeltaEngineLogo.png");
			var metaData = creator.CreateMetaDataFromFile(filePath);
			Assert.AreEqual("DeltaEngineLogo", metaData.Name);
			Assert.AreEqual(ContentType.Image, metaData.Type);
			Assert.LessOrEqual((DateTime.Now - metaData.LastTimeUpdated).Seconds, 1);
			Assert.AreEqual("en", metaData.Language);
			Assert.AreEqual("DeltaEngineLogo.png", metaData.LocalFilePath);
			Assert.AreEqual(0, metaData.PlatformFileId);
			Assert.AreEqual(8153, metaData.FileSize);
			Assert.AreEqual(2, metaData.Values.Count);
			Assert.AreEqual("(128,128)", metaData.Values["PixelSize"]);
			Assert.AreEqual("Normal", metaData.Values["BlendMode"]);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateContentMetaDataFromXmlFileToBeUploadedToTheOnlineService()
		{
			var creator = new ContentMetaDataCreator();
			var filePath = Path.Combine("Content", "Verdana12.xml");
			var metaData = creator.CreateMetaDataFromFile(filePath);
			Assert.AreEqual("Verdana12", metaData.Name);
			Assert.AreEqual(ContentType.Xml, metaData.Type);
			Assert.LessOrEqual((DateTime.Now - metaData.LastTimeUpdated).Seconds, 1);
			Assert.AreEqual("en", metaData.Language);
			Assert.AreEqual("Verdana12.xml", metaData.LocalFilePath);
			Assert.AreEqual(0, metaData.PlatformFileId);
			Assert.AreEqual(11354, metaData.FileSize);
			Assert.AreEqual(0, metaData.Values.Count);
		}
	}
}