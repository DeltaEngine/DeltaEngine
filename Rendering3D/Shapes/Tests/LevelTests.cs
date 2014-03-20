using System.Collections.Generic;
using DeltaEngine.Content.Xml;
using DeltaEngine.Datatypes;
using DeltaEngine.GameLogic;
using NUnit.Framework;

namespace DeltaEngine.Rendering3D.Shapes.Tests
{
	public class LevelTests
	{
		[Test]
		public void CreateALevel()
		{
			level = new Level(new Size(5, 5));
			level.SetTile(new Vector2D(0, 0), LevelTileType.SpawnPoint);
			level.SetTile(new Vector2D(1, 1), LevelTileType.ExitPoint);
			level.SetTile(new Vector2D(2, 2), LevelTileType.BlockedPlaceable);
			level.SetTile(new Vector2D(0, 1), LevelTileType.Blue);
			level.SetTile(new Vector2D(0, 2), LevelTileType.Brown);
			level.SetTile(new Vector2D(0, 3), LevelTileType.Green);
			level.SetTile(new Vector2D(0, 4), LevelTileType.Gray);
			level.SetTile(new Vector2D(1, 0), LevelTileType.Placeable);
			level.SetTile(new Vector2D(1, 2), LevelTileType.Yellow);
			level.SetTile(new Vector2D(1, 3), LevelTileType.Blocked);
			level.SetTile(new Vector2D(1, 4), LevelTileType.Red);
			level.AddWave(new Wave(2.0f, 1.0f, "Test", "Test", 20.0f));
			var stream = new XmlFile(BuildXmlData()).ToMemoryStream();
			var loadedLevel = new MockLevel(new Size(0, 0));
			loadedLevel.LoadData(stream);
			Assert.AreEqual(loadedLevel.Size, level.Size);
			Assert.AreEqual(loadedLevel.MapData[0], level.MapData[0]);
			Assert.AreEqual(loadedLevel.Waves[0].MaxTime, level.Waves[0].MaxTime);
			Assert.AreEqual(loadedLevel.Waves[0].SpawnInterval, level.Waves[0].SpawnInterval);
			Assert.AreEqual(loadedLevel.Waves[0].SpawnTypeList, level.Waves[0].SpawnTypeList);
			Assert.AreEqual(loadedLevel.Waves[0].WaitTime, level.Waves[0].WaitTime);
			level.Dispose();
		}

		private Level level;

		public XmlData BuildXmlData()
		{
			var xml = new XmlData("Level");
			xml.AddAttribute("Name", "TestName");
			xml.AddAttribute("Size", level.Size);
			AddPoints(xml, LevelTileType.SpawnPoint);
			AddPoints(xml, LevelTileType.ExitPoint);
			xml.AddChild("Map", level.ToTextForXml());
			AddWaves(xml);
			return xml;
		}

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
			for (int y = 0; y < level.Size.Height; y++)
				for (int x = 0; x < level.Size.Width; x++)
				{
					var index = (int)(x + y * level.Size.Width);
					if (level.MapData[index] == pointType)
						result.Add(new Vector2D(x, y));
				}
			return result;
		}

		private void AddWaves(XmlData xml)
		{
			foreach (var wave in level.Waves)
				xml.AddChild(wave.AsXmlData());
		}
	}
}