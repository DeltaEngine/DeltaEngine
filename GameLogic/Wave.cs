using System.Collections.Generic;
using System.Globalization;
using DeltaEngine.Content.Xml;
using DeltaEngine.Extensions;

namespace DeltaEngine.GameLogic
{
	/// <summary>
	/// DataContainer that provides data which object should be created when a wave is started.
	/// </summary>
	public class Wave
	{
		public Wave(float waitTime, float spawnInterval, string thingsToSpawn, string shortName = "",
			float maxTime = 0.0f, int maxSpawnItems = 1)
		{
			WaitTime = waitTime;
			SpawnInterval = spawnInterval;
			ShortName = shortName;
			MaxTime = maxTime;
			MaxSpawnItems = maxSpawnItems;
			if (string.IsNullOrEmpty(thingsToSpawn))
				return; //ncrunch: no coverage
			foreach (var thing in thingsToSpawn.SplitAndTrim(','))
				SpawnTypeList.Add(thing);
		}

		public float WaitTime { get; private set; }
		public float SpawnInterval { get; private set; }
		public string ShortName { get; private set; }
		public float MaxTime { get; private set; }
		public int MaxSpawnItems { get; private set; }
		public readonly List<string> SpawnTypeList = new List<string>();

		public Wave(Wave wave)
		{
			WaitTime = wave.WaitTime;
			SpawnInterval = wave.SpawnInterval;
			ShortName = wave.ShortName;
			MaxTime = wave.MaxTime;
			MaxSpawnItems = wave.MaxSpawnItems;
			SpawnTypeList = wave.SpawnTypeList;
		}

		public XmlData AsXmlData()
		{
			var xml = new XmlData("Wave");
			xml.AddAttribute("WaitTime", WaitTime);
			xml.AddAttribute("SpawnInterval", SpawnInterval);
			xml.AddAttribute("ShortName", ShortName);
			xml.AddAttribute("MaxTime", MaxTime);
			xml.AddAttribute("MaxSpawnItems", MaxSpawnItems);
			xml.AddAttribute("SpawnTypeList", SpawnTypeList.ToText());
			return xml;
		}

		public override string ToString()
		{
			string baseString = WaitTime.ToString(CultureInfo.InvariantCulture) + ", " +
				SpawnInterval.ToString(CultureInfo.InvariantCulture) + ", " +
				MaxTime.ToString(CultureInfo.InvariantCulture) + ", " + MaxSpawnItems + ", " +
				SpawnTypeList.ToText();
			return string.IsNullOrEmpty(ShortName) ? baseString : ShortName + " " + baseString;
		}
	}
}