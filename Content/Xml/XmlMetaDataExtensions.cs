using System;
using System.Collections.Generic;
using DeltaEngine.Extensions;

namespace DeltaEngine.Content.Xml
{
	public static class XmlMetaDataExtensions
	{
		public static XmlData CreateProjectMetaData(string projectName, string platformName)
		{
			var data = new XmlData(typeof(ContentMetaData).Name);
			data.AddAttribute("Name", projectName);
			data.AddAttribute("Type", "Scene");
			data.AddAttribute("LastTimeUpdated", DateTime.Now.GetIsoDateTime());
			data.AddAttribute("ContentDeviceName", platformName);
			return data;
		}

		public static void AddMetaDataEntry(this XmlData projectMetaData,
			ContentMetaData contentEntry)
		{
			var newEntry = new XmlData(typeof(ContentMetaData).Name);
			AddBasicMetaDataValues(newEntry, contentEntry);
			if (contentEntry.Language != null)
				newEntry.AddAttribute("Language", contentEntry.Language);
			if (contentEntry.PlatformFileId != 0)
				newEntry.AddAttribute("PlatformFileId", contentEntry.PlatformFileId);
			foreach (KeyValuePair<string, string> valuePair in contentEntry.Values)
				newEntry.AddAttribute(valuePair.Key, valuePair.Value);
			projectMetaData.AddChild(newEntry);
		}

		private static void AddBasicMetaDataValues(XmlData xmlMetaData, ContentMetaData metaData)
		{
			xmlMetaData.AddAttribute("Name", metaData.Name);
			xmlMetaData.AddAttribute("Type", metaData.Type);
			xmlMetaData.AddAttribute("LastTimeUpdated", metaData.LastTimeUpdated.GetIsoDateTime());
			if (metaData.LocalFilePath == null)
				return;
			xmlMetaData.AddAttribute("LocalFilePath", metaData.LocalFilePath);
			xmlMetaData.AddAttribute("FileSize", metaData.FileSize);
		}
	}
}