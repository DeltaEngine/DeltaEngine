using System.Collections.Generic;
using CreepyTowers.Enemy.Creeps;
using DeltaEngine.Datatypes;
using DeltaEngine.GameLogic;

namespace CreepyTowers.Levels
{
	public class ChildrensRoomLevelMap
	{
		public static LevelTileType[] MapInfo()
		{
			var mapData = new LevelTileType[(int)(Size.Width * Size.Height)];
			for (int i = 0; i < mapData.Length; i++)
				mapData[i] = LevelTileType.Placeable;

			//walls and emoty grid spaces
			SetMapValuesToBlocked(mapData, 0, 15, 0, 4);
			SetMapValuesToBlocked(mapData, 0, 15, 13, 15);
			SetMapValuesToBlocked(mapData, 0, 2, 0, 15);
			SetMapValuesToBlocked(mapData, 13, 15, 0, 15);

			SetTile(mapData, 12, 3, LevelTileType.Blocked);
			SetTile(mapData, 12, 4, LevelTileType.Blocked);
			SetTile(mapData, 12, 10, LevelTileType.Blocked);
			SetTile(mapData, 12, 11, LevelTileType.Blocked);
			SetTile(mapData, 12, 12, LevelTileType.Blocked);
			SetTile(mapData, 11, 12, LevelTileType.Blocked);

			//pillows
			SetTile(mapData, 10, 4, LevelTileType.Blocked);
			SetTile(mapData, 10, 5, LevelTileType.Blocked);

			//ball
			SetTile(mapData, 9, 8, LevelTileType.Blocked);

			//boxes
			SetTile(mapData, 5, 10, LevelTileType.Blocked);
			SetTile(mapData, 5, 11, LevelTileType.Blocked);

			//spawn points
			SetTile(mapData, 7, 3, LevelTileType.SpawnPoint);

			//Exit Points
			SetTile(mapData, 7, 13, LevelTileType.ExitPoint);
			SetTile(mapData, 8, 13, LevelTileType.ExitPoint);
			return mapData;
		}

		private static readonly Size Size = new Size(16, 16);

		private static void SetMapValuesToBlocked(LevelTileType[] mapData, int startRow, int endRow,
			int startCol, int endCol)
		{
			for (int x = startRow; x < endRow; x++)
				for (int y = startCol; y < endCol; y++)
					SetTile(mapData, x, y, LevelTileType.Blocked);
		}

		private static void SetTile(LevelTileType[] mapData, int x, int y, LevelTileType tileType)
		{
			mapData[(int)(x + y * Size.Width)] = tileType;
		}

		public static IEnumerable<CreepWave> GetListOfWaves()
		{
			return new List<CreepWave>
			{
				new CreepWave(2.0f, 2.0f, "Cloth"),
				new CreepWave(1.0f, 1.5f, "Cloth"),
				new CreepWave(1.0f, 1.5f, "Cloth")
			};
		}
	}
}