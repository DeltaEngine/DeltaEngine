using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using DeltaEngine.Core;
using DeltaEngine.Extensions;

namespace DeltaEngine.Content.Disk
{
	/// <summary>
	/// Loads and caches files directly from disk using an xml file created earlier by ContentManager
	/// to get all content meta data (names, types, last time updated, pixel size, etc.)
	/// </summary>
	public sealed class DiskContentLoader : ContentLoader
	{
		//ncrunch: no coverage start
		private DiskContentLoader() {}

		private void LazyInitialize()
		{
			if (isInitialized)
				return;
			isInitialized = true;
			if (Directory.Exists(ContentProjectPath))
				LoadMetaData(ContentMetaDataFilePath);
			else
				Logger.Warning("Content path " + ContentProjectPath +
					" does not exist, cannot load content!");
		}

		private bool isInitialized;

		internal void LoadMetaData(string xmlFilePath)
		{
			if (IsMetaDataNoLongerUpToDate(xmlFilePath))
				xml = new ContentMetaDataFileCreator(xml).CreateAndLoad(xmlFilePath);
			ParseXmlNode(xml.Root);
		}

		private XDocument xml;

		private bool IsMetaDataNoLongerUpToDate(string filePath)
		{
			if (File.Exists(filePath))
			{
				using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					if (fs.Length > 0)
						xml = XDocument.Load(fs);
				return xml == null ||
					(DateTime.Now - new FileInfo(filePath).LastWriteTime).TotalSeconds > 90;
			}
			Logger.Info("ContentMetaData.xml not found, a new one will be created. " +
				"For proper content meta data please use the Delta Engine Editor.");
			return true;
		}

		private void ParseXmlNode(XElement currentNode)
		{
			var currentElement = ParseContentMetaData(currentNode.Attributes());
			var name = currentNode.Attribute("Name").Value;
			if (!metaData.ContainsKey(name))
				metaData.Add(name, currentElement);
			foreach (var node in currentNode.Elements())
				ParseXmlNode(node);
		}

		private static ContentMetaData ParseContentMetaData(IEnumerable<XAttribute> attributes)
		{
			var data = new ContentMetaData();
			foreach (var attribute in attributes)
				switch (attribute.Name.LocalName)
				{
				case "Name":
					data.Name = attribute.Value;
					break;
				case "Type":
					data.Type = attribute.Value.TryParse(ContentType.Image);
					break;
				case "LastTimeUpdated":
					data.LastTimeUpdated = DateExtensions.Parse(attribute.Value);
					break;
				case "LocalFilePath":
					data.LocalFilePath = attribute.Value;
					break;
				case "PlatformFileId":
					data.PlatformFileId = attribute.Value.Convert<int>();
					break;
				case "FileSize":
					data.FileSize = attribute.Value.Convert<int>();
					break;
				default:
					data.Values.Add(attribute.Name.LocalName, attribute.Value);
					break;
				}
			if (string.IsNullOrEmpty(data.Name))
				throw new InvalidContentMetaDataNameIsAlwaysNeeded();
			return data;
		}

		public class InvalidContentMetaDataNameIsAlwaysNeeded : Exception {}

		private readonly Dictionary<string, ContentMetaData> metaData =
			new Dictionary<string, ContentMetaData>(StringComparer.OrdinalIgnoreCase);

		public override ContentMetaData GetMetaData(string contentName, Type contentClassType = null)
		{
			LazyInitialize();
			return metaData.ContainsKey(contentName) ? metaData[contentName] : null;
		}

		protected override bool HasValidContentAndMakeSureItIsLoaded()
		{
			LazyInitialize();
			return metaData.Count > 0;
		}

		public override DateTime LastTimeUpdated
		{
			get { return DateTime.Now; }
		}
	}
}