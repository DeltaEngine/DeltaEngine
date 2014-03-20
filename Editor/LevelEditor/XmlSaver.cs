using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DeltaEngine.Content.Xml;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
using DeltaEngine.GameLogic;

namespace DeltaEngine.Editor.LevelEditor
{
	public class XmlSaver
	{
		public XmlSaver(Service service)
		{
			this.service = service;
		}

		private readonly Service service;

		public void OpenXmlFile()
		{
			var filename = Path.Combine(Directory.GetCurrentDirectory(), "Content", service.ProjectName,
				ContentName + ".xml");
			if (File.Exists(filename))
				Process.Start(filename); //ncrunch: no coverage
			else
				throw (new CannotOpenLevelFile());
		} //ncrunch: no coverage

		public string ContentName { private get; set; }

		public class CannotOpenLevelFile : Exception { }

		public void SaveToServer()
		{
			var bytes = new XmlFile(BuildXmlData()).ToMemoryStream().ToArray();
			var dataAndName = new Dictionary<string, byte[]> { { ContentName + ".xml", bytes } };
			var metaDataCreator = new ContentMetaDataCreator();
			var contentMetaData = metaDataCreator.CreateMetaDataFromLevelData(ContentName, bytes);
			service.UploadContent(contentMetaData, dataAndName);
		}

		public XmlData BuildXmlData()
		{
			var xml = new XmlData("Level");
			xml.AddAttribute("Name", ContentName);
			xml.AddAttribute("Size", Level.Size);
			xml.AddAttribute("ModelName", ModelName);
			AddPoints(xml, LevelTileType.SpawnPoint);
			AddPoints(xml, LevelTileType.ExitPoint);
			xml.AddChild("Map", Level.ToTextForXml());
			AddWaves(xml);
			return xml;
		}

		public Level Level { private get; set; }

		public string ModelName { get; set; }

		private void AddPoints(XmlData xml, LevelTileType pointType)
		{
			int counter = 0;
			foreach (var point in FindPoints(pointType))
			{
				var pointXml = new XmlData(pointType.ToString());
				pointXml.AddAttribute("Name", pointType.ToString().Replace("Point", "") + (counter++));
				pointXml.AddAttribute("Position", point);
				xml.AddChild(pointXml);
			}
		}

		private IEnumerable<Vector2D> FindPoints(LevelTileType pointType)
		{
			var result = new List<Vector2D>();
			for (int y = 0; y < Level.Size.Height; y++)
				for (int x = 0; x < Level.Size.Width; x++)
				{
					var index = (int)(x + y * Level.Size.Width);
					if (Level.MapData[index] == pointType)
						result.Add(new Vector2D(x, y));
				}
			return result;
		}

		private void AddWaves(XmlData xml)
		{
			foreach (var wave in Level.Waves)
				xml.AddChild(wave.AsXmlData());
		}
	}
}