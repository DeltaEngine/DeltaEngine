using System;
using System.Collections.Generic;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Content.Xml.Tests
{
	public class XmlMetaDataExtensionsTests
	{
		[SetUp]
		public void InitializeProjectMetaData()
		{
			projectMetaData = XmlMetaDataExtensions.CreateProjectMetaData(ProjectName, Platform);
		}

		private XmlData projectMetaData;
		private const string ProjectName = "SmallProject";
		private const string Platform = "Windows";

		[Test]
		public void CreateProjectMetaData()
		{
			AssertMinimumContentMetaData(projectMetaData, ProjectName, "Scene", DateTime.Now);
			Assert.AreEqual(Platform, projectMetaData.GetAttributeValue("ContentDeviceName"));
		}

		private static void AssertMinimumContentMetaData(XmlData xmlMetaData,
			string nameAttributeValue, string typeAttributeValue, DateTime lastUpdatedAttributeValue)
		{
			Assert.AreEqual("ContentMetaData", xmlMetaData.Name);
			Assert.AreEqual(nameAttributeValue, xmlMetaData.GetAttributeValue("Name"));
			Assert.AreEqual(typeAttributeValue, xmlMetaData.GetAttributeValue("Type"));
			Assert.AreEqual(lastUpdatedAttributeValue.GetIsoDateTime(),
				xmlMetaData.GetAttributeValue("LastTimeUpdated"));
		}

		[Test]
		public void ConvertMinimumContentMetaDataToXml()
		{
			ContentMetaData testMetaData = GetMinimumTestContentMetaData();
			Assert.IsEmpty(projectMetaData.Children);
			projectMetaData.AddMetaDataEntry(testMetaData);
			Assert.AreEqual(1, projectMetaData.Children.Count);
			AssertMinimumContentMetaData(projectMetaData.Children[0], testMetaData.Name,
				testMetaData.Type.ToString(), testMetaData.LastTimeUpdated);
		}

		private static ContentMetaData GetMinimumTestContentMetaData()
		{
			return new ContentMetaData
			{
				Name = "ImageObject",
				Type = ContentType.Image,
				LastTimeUpdated = DateTime.Now,
			};
		}

		[Test]
		public void ConvertBasicContentMetaDataToXml()
		{
			ContentMetaData testMetaData = GetBasicTestContentMetaData();
			Assert.IsEmpty(projectMetaData.Children);
			projectMetaData.AddMetaDataEntry(testMetaData);
			Assert.AreEqual(1, projectMetaData.Children.Count);
			AssertFullContentMetaDataEntry(projectMetaData.Children[0], testMetaData);
		}

		private static ContentMetaData GetBasicTestContentMetaData()
		{
			ContentMetaData basicMetaData = GetMinimumTestContentMetaData();
			basicMetaData.LocalFilePath = "ImagePixel.data";
			basicMetaData.FileSize = 0;
			return basicMetaData;
		}

		private static void AssertBasicContentMetaData(XmlData xmlMetaData, ContentMetaData metaData)
		{
			AssertMinimumContentMetaData(xmlMetaData, metaData.Name, metaData.Type.ToString(),
				metaData.LastTimeUpdated);
			Assert.AreEqual(metaData.LocalFilePath, xmlMetaData.GetAttributeValue("LocalFilePath"));
			Assert.AreEqual(metaData.FileSize.ToString(), xmlMetaData.GetAttributeValue("FileSize"));
		}

		[Test]
		public void ConvertFullContentMetaDataToXml()
		{
			ContentMetaData testMetaData = GetBasicTestContentMetaData();
			testMetaData.Language = "English";
			testMetaData.PlatformFileId = 3;
			testMetaData.Values.Add("CoolValue", "SuperCool");
			testMetaData.Values.Add("AnotherCoolValue", "SuperDuperCool");
			projectMetaData.AddMetaDataEntry(testMetaData);
			AssertFullContentMetaDataEntry(projectMetaData.Children[0], testMetaData);
		}

		private static void AssertFullContentMetaDataEntry(XmlData xmlMetaData,
			ContentMetaData metaData)
		{
			AssertBasicContentMetaData(xmlMetaData, metaData);
			AssertLanguageInMetaData(xmlMetaData, metaData);
			AssertPlatformFileIdInMetaData(xmlMetaData, metaData);
			foreach (KeyValuePair<string, string> valuePair in metaData.Values)
				Assert.AreEqual(metaData.Values[valuePair.Key],
					xmlMetaData.GetAttributeValue(valuePair.Key));
		}

		private static void AssertLanguageInMetaData(XmlData xmlMetaData, ContentMetaData metaData)
		{
			if (metaData.Language == null)
				Assert.IsFalse(IsAttributeAvailable(xmlMetaData, "Language"));
			else
				Assert.AreEqual(metaData.Language, xmlMetaData.GetAttributeValue("Language"));
		}

		private static bool IsAttributeAvailable(XmlData xmlData, string attributeName)
		{
			return xmlData.Attributes.FindIndex(a => a.Name == attributeName) > -1;
		}

		private static void AssertPlatformFileIdInMetaData(XmlData xmlMetaData, ContentMetaData metaData)
		{
			if (metaData.PlatformFileId == 0)
				Assert.IsFalse(IsAttributeAvailable(xmlMetaData, "PlatformFileId"));
			else
				Assert.AreEqual(metaData.PlatformFileId.ToString(),
					xmlMetaData.GetAttributeValue("PlatformFileId"));
		}
	}
}