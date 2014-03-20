using System;
using System.IO;
using System.Text;
using DeltaEngine.Datatypes;

namespace DeltaEngine.GameLogic.Tests
{
	public class MockLevel : Level
	{
		//ncrunch: no coverage start
		protected MockLevel(string contentName)
			: base(contentName) {}

		//ncrunch: no coverage end

		public MockLevel(Size size)
			: base(size) {}

		public void LoadTestData()
		{
			string text = @"<?xml version=""1.0""?>
<Level Name=""LevelsLivingRoom"" Size=""4,3"" StartingGold=""150"">
	<SpawnPoint Name=""LeftDoor"" Position=""0,0""/>
	<SpawnPoint Name=""RightDoor"" Position=""1,1""/>
	<ExitPoint Name=""ExitDoor"" Position=""2,2""/>
	<Map>
		XPLR
		GBYO
		ASE.
	</Map>
		<Wave WaitTime=""3"" SpawnInterval=""1.5"" MaxCreeps=""3"" MaxTimeIntervalTillNextWave=""10"" CreepTypeList=""Wood""/>
</Level>";
			var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
			LoadData(stream);
		}
	}

	internal struct CompleteSquare
	{
		public CompleteSquare(char typeSymbol, int distanceCosts = 100, bool isPath = false)
			: this(ToTileType(typeSymbol), distanceCosts, isPath) {}

		private static LevelTileType ToTileType(char letter)
		{
			switch (letter)
			{
			case 'X':
				return LevelTileType.Blocked;
			case 'S':
				return LevelTileType.SpawnPoint;
			case 'E':
				return LevelTileType.ExitPoint;
			default:
				return LevelTileType.Nothing;
			}
		}

		public CompleteSquare(LevelTileType type, int distanceCosts = 100, bool isPath = false)
			: this()
		{
			Type = type;
		}

		public LevelTileType Type { get; private set; }

		public char ToTypeSymbol()
		{
			switch (Type)
			{
			case LevelTileType.Nothing:
				return '.';
			case LevelTileType.Blocked:
				return 'X';
			case LevelTileType.SpawnPoint:
				return 'S';
			case LevelTileType.ExitPoint:
				return 'E';
			default:
				throw new UnsupportedTileType(Type); //ncrunch: no coverage
			}
		}

		//ncrunch: no coverage start
		private class UnsupportedTileType : Exception
		{
			public UnsupportedTileType(LevelTileType tileType)
				: base(tileType.ToString()) {}
		}
		//ncrunch: no coverage end
	}
}