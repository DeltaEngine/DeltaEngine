using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Content.Mocks;
using DeltaEngine.Content.Xml;

namespace CreepyTowers.Tests
{
	public class CreepyTowersMockContentLoader : MockContentLoader
	{
		protected override Stream GetContentDataStream(ContentData content)
		{
			if (content.MetaData.Type == ContentType.Xml && content.Name.Equals("GroupProperties"))
				return CreateMockGroupProperties();
			if (content.MetaData.Type == ContentType.Xml && content.Name.Equals("TowerProperties"))
				return CreateMockTowerProperties();
			if (content.MetaData.Type == ContentType.Xml && content.Name.Equals("CreepProperties"))
				return CreateMockCreepProperties();
			if (content.MetaData.Type == ContentType.Xml && content.Name.Equals("BuffProperties"))
				return CreateMockBuffProperties();
			if (content.MetaData.Type == ContentType.Xml && content.Name.Equals("AdjustmentProperties"))
				return CreateMockAdjustmentProperties();
			if (content.MetaData.Type == ContentType.Level && content.Name.Equals("DummyLevelInfo"))
				return CreateMockGameLevel();
			return base.GetContentDataStream(content);
		}

		private static Stream CreateMockGroupProperties()
		{
			var xmlData = new XmlData("Groups");
			xmlData.AddChild(CreateTestGroup("Paper2", "Paper, Paper"));
			xmlData.AddChild(CreateTestGroup("Cloth3", "Cloth, Cloth, Cloth"));
			return new XmlFile(xmlData).ToMemoryStream();
		}

		private static XmlData CreateTestGroup(string groupName, string formation)
		{
			var testGroup = new XmlData("Group");
			testGroup.AddAttribute("Name", groupName);
			testGroup.AddAttribute("CreepTypeList", formation);
			testGroup.AddAttribute("SpawnIntervalList", "0.5");
			return testGroup;
		}

		private static Stream CreateMockTowerProperties()
		{
			var xmlData = new XmlData("TowerProperties");
			var testAcid = CreateTestTower("TowerAcidConeJanitorHigh", "Acid", "DirectShot");
			testAcid.AddAttribute("Cost", "200");
			xmlData.AddChild(testAcid);
			var testFire = CreateTestTower("TowerFireCandlehulaHigh", "Fire", "Circle");
			testFire.AddAttribute("Cost", "75");
			xmlData.AddChild(testFire);
			return new XmlFile(xmlData).ToMemoryStream();
		}

		private static XmlData CreateTestTower(string name, string type, string shotType)
		{
			var testTower = new XmlData("TowerData");
			testTower.AddAttribute("Name", name);
			testTower.AddAttribute("Type", type);
			testTower.AddAttribute("AttackType", shotType);
			testTower.AddAttribute("Range", "3.0");
			testTower.AddAttribute("AttackFrequency", "0.5");
			testTower.AddAttribute("AttackDamage", "35.0");
			return testTower;
		}

		private static Stream CreateMockCreepProperties()
		{
			var xmlData = new XmlData("CreepProperties");
			xmlData.AddChild(CreateTestCottonCreep());
			xmlData.AddChild(CreateTestGlassCreep());
			return new XmlFile(xmlData).ToMemoryStream();
		}

		private static XmlData CreateTestCottonCreep()
		{
			var testCreep = CreateCreepData("CreepCottonMummyHigh", "Cloth", "110");
			testCreep.AddAttribute("Resistance", "10");
			testCreep.AddAttribute("Speed", "1.1");
			testCreep.AddAttribute("Gold", "13");
			testCreep.AddChild(CreateCottonModifiers());
			return testCreep;
		}

		private static XmlData CreateCreepData(string name, string type, string hp)
		{
			var testCreep = new XmlData("CreepData");
			testCreep.AddAttribute("Name", name);
			testCreep.AddAttribute("Type", type);
			testCreep.AddAttribute("MaxHp", hp);
			return testCreep;
		}

		private static XmlData CreateCottonModifiers()
		{
			var testModifier = CreateModifier("3", "3", "1");
			testModifier.AddAttribute("Impact", "0.25");
			testModifier.AddAttribute("Slice", "2");
			testModifier.AddAttribute("Ice", "0.25");
			return testModifier;
		}

		private static XmlData CreateModifier(string acid, string fire, string water)
		{
			var testModifier = new XmlData("Modifiers");
			testModifier.AddAttribute("Acid", acid);
			testModifier.AddAttribute("Fire", fire);
			testModifier.AddAttribute("Water", water);
			return testModifier;
		}

		private static XmlData CreateTestGlassCreep()
		{
			var testCreep = CreateCreepData("CreepGlassHigh", "Glass", "100");
			testCreep.AddAttribute("Resistance", "5");
			testCreep.AddAttribute("Speed", "3");
			testCreep.AddAttribute("Gold", "100");
			testCreep.AddChild(CreateGlassModifiers());
			return testCreep;
		}

		private static XmlData CreateGlassModifiers()
		{
			var testModifier = CreateModifier("0.1", "0.5", "0.1");
			testModifier.AddAttribute("Impact", "3");
			testModifier.AddAttribute("Slice", "0.5");
			testModifier.AddAttribute("Ice", "2");
			return testModifier;
		}

		private static Stream CreateMockBuffProperties()
		{
			var xmlData = new XmlData("BuffProperties");
			xmlData.AddChild(CreateDragonRangeBuff());
			xmlData.AddChild(CreatePenguinAttackFrequencyBuff());
			xmlData.AddChild(CreatePiggyBankGoldMultiplierBuff());
			xmlData.AddChild(CreatePiggyBankPayDayMultiplierBuff());
			xmlData.AddChild(CreateTestGoldBuff());
			xmlData.AddChild(CreateTestHpBuff());
			return new XmlFile(xmlData).ToMemoryStream();
		}

		private static XmlData CreateDragonRangeBuff()
		{
			var xmlData = new XmlData("DragonRangeMultiplier");
			xmlData.AddAttribute("Attribute", "Range");
			xmlData.AddAttribute("Multiplier", 1.25f);
			xmlData.AddAttribute("Addition", 0.0f);
			xmlData.AddAttribute("Duration", 0.0f);
			return xmlData;
		}

		private static XmlData CreatePenguinAttackFrequencyBuff()
		{
			var xmlData = new XmlData("PenguinAttackFrequencyMultiplier");
			xmlData.AddAttribute("Attribute", "AttackFrequency");
			xmlData.AddAttribute("Multiplier", 1.3333f);
			xmlData.AddAttribute("Addition", 0.0f);
			xmlData.AddAttribute("Duration", 0.0f);
			return xmlData;
		}

		private static XmlData CreatePiggyBankGoldMultiplierBuff()
		{
			var xmlData = new XmlData("PiggyBankGoldMultiplier");
			xmlData.AddAttribute("Attribute", "Gold");
			xmlData.AddAttribute("Multiplier", 1.05f);
			xmlData.AddAttribute("Addition", 0.0f);
			xmlData.AddAttribute("Duration", 0.0f);
			return xmlData;
		}

		private static XmlData CreatePiggyBankPayDayMultiplierBuff()
		{
			var xmlData = new XmlData("PiggyBankPayDayGoldMultiplier");
			xmlData.AddAttribute("Attribute", "Gold");
			xmlData.AddAttribute("Multiplier", 1.5f);
			xmlData.AddAttribute("Addition", 0.0f);
			xmlData.AddAttribute("Duration", 5.0f);
			return xmlData;
		}

		private static XmlData CreateTestGoldBuff()
		{
			var xmlData = new XmlData("TestGoldBuff");
			xmlData.AddAttribute("Attribute", "Gold");
			xmlData.AddAttribute("Multiplier", 2.0f);
			xmlData.AddAttribute("Addition", -3.0f);
			xmlData.AddAttribute("Duration", 5.0f);
			return xmlData;
		}

		private static XmlData CreateTestHpBuff()
		{
			var xmlData = new XmlData("TestHpBuff");
			xmlData.AddAttribute("Attribute", "Hp");
			xmlData.AddAttribute("Multiplier", 3.0f);
			xmlData.AddAttribute("Addition", 4.0f);
			xmlData.AddAttribute("Duration", 6.0f);
			return xmlData;
		}

		private static Stream CreateMockAdjustmentProperties()
		{
			var xmlData = new XmlData("AdjustmentProperties");
			xmlData.AddChild(CreateTestAdjustment());
			return new XmlFile(xmlData).ToMemoryStream();
		}

		private static XmlData CreateTestAdjustment()
		{
			var xmlData = new XmlData("TestAdjustment");
			xmlData.AddAttribute("Attribute", "Hp");
			xmlData.AddAttribute("Resist", "");
			xmlData.AddAttribute("Adjustment", -100.0f);
			return xmlData;
		}

		private static Stream CreateMockGameLevel()
		{
			var xmlData = new XmlData("Level");
			xmlData.AddAttribute("Name", "TestGameLevelInfo");
			xmlData.AddAttribute("Size", "16, 10");
			xmlData.AddChild(CreateSpawnPoint());
			xmlData.AddChild(CreateExitPoint());
			xmlData.AddChild("Map", KidsRoomMapInfo());
			xmlData.AddChild("Wave", CreateWave());
			return new XmlFile(xmlData).ToMemoryStream();
		}

		private static XmlData CreateSpawnPoint()
		{
			var xmlData = new XmlData("SpawnPoint");
			xmlData.AddAttribute("Name", "Spawn0");
			xmlData.AddAttribute("Position", "2, 4");
			return xmlData;
		}

		private static XmlData CreateExitPoint()
		{
			var xmlData = new XmlData("ExitPoint");
			xmlData.AddAttribute("Name", "Exit0");
			xmlData.AddAttribute("Position", "13, 4");
			return xmlData;
		}

		private static string KidsRoomMapInfo()
		{
			return "\n" + "XXXXXXXXXXXXXXXX\n" + "XXXXXXXXXXXXXXXX\n" + "XXXXXXXXXXXXXXXX\n" +
				"XXXXXXPXXXXXXXXX\n" + "XXS..........EXX\n" + "XXS..........EXX\n" + "XXXXXXXXXXPXXXXX\n" +
				"XXXXXXXXXXXXXXXX\n" + "XXXXXXXXXXXXXXXX\n" + "XXXXXXXXXXXXXXXX\n";
		}

		private static XmlData CreateWave()
		{
			var xmlData = new XmlData("Wave");
			xmlData.AddAttribute("WaitTime", "2");
			xmlData.AddAttribute("SpawnInterval", "2");
			xmlData.AddAttribute("ShortName", "Wave0");
			xmlData.AddAttribute("MaxTime", "0");
			xmlData.AddAttribute("MaxSpawnItems", "1");
			xmlData.AddAttribute("SpawnTypeList", "Cloth");
			return xmlData;
		}
	}
}