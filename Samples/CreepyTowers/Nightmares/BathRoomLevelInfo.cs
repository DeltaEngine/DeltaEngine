using System.Collections.Generic;
using CreepyTowers.Enemy.Creeps;
using DeltaEngine.Datatypes;
using DeltaEngine.GameLogic;

namespace CreepyTowers.Nightmares
{
	public class BathRoomLevelInfo
	{
		public static LevelTileType[] MapInfo(Size gridSize)
		{
			size = gridSize;
			var mapData = new LevelTileType[(int)(BathRoomLevelInfo.size.Width * BathRoomLevelInfo.size.Height)];
			for (int i = 0; i < mapData.Length; i++)
				mapData[i] = LevelTileType.Nothing;

			//walls and emoty grid spaces
			SetMapValuesToBlocked(mapData, 0, 0, 0, 23);
			SetMapValuesToBlocked(mapData, 12, 13, 0, 23);
			SetMapValuesToBlocked(mapData, 1, 1, 17, 23);
			SetMapValuesToBlocked(mapData, 11, 11, 0, 6);

			SetTile(mapData, 9, 0, LevelTileType.Blocked);
			SetTile(mapData, 10, 0, LevelTileType.Blocked);
			SetTile(mapData, 11, 0, LevelTileType.Blocked);
			SetTile(mapData, 11, 12, LevelTileType.Blocked);

			//towel shelf
			SetTile(mapData, 6, 8, LevelTileType.Blocked);
			SetTile(mapData, 7, 8, LevelTileType.Blocked);
			SetTile(mapData, 8, 8, LevelTileType.Blocked);
			SetTile(mapData, 6, 9, LevelTileType.Blocked);
			SetTile(mapData, 7, 9, LevelTileType.Blocked);
			SetTile(mapData, 8, 9, LevelTileType.Blocked);

			//plant
			SetTile(mapData, 2, 23, LevelTileType.Blocked);

			//toilet
			SetTile(mapData, 1, 14, LevelTileType.Blocked);

			//towels
			SetTile(mapData, 3, 4, LevelTileType.Blocked);
			SetTile(mapData, 4, 5, LevelTileType.Blocked);

			SetTile(mapData, 5, 15, LevelTileType.Blocked);
			SetTile(mapData, 7, 16, LevelTileType.Blocked);
			SetTile(mapData, 9, 14, LevelTileType.Blocked);

			//stools
			SetTile(mapData, 4, 20, LevelTileType.Blocked);
			SetTile(mapData, 2, 10, LevelTileType.Blocked);

			//toilet paper
			SetTile(mapData, 5, 14, LevelTileType.Blocked);

			//spawn points
			SetTile(mapData, 2, 1, LevelTileType.SpawnPoint);

			//Exit Points
			SetTile(mapData, 7, 23, LevelTileType.ExitPoint);

			//Tower Placeable Points
			SetTile(mapData, 3, 7, LevelTileType.Yellow); //impact tower
			SetTile(mapData, 6, 13, LevelTileType.Red); //fire tower
			SetTile(mapData, 8, 20, LevelTileType.Blue); //water tower
			return mapData;
		}

		private static Size size;

		private static void SetMapValuesToBlocked(LevelTileType[] mapData, int startRow, int endRow,
			int startCol, int endCol)
		{
			for (int x = startRow; x < endRow; x++)
				for (int y = startCol; y < endCol; y++)
					SetTile(mapData, x, y, LevelTileType.Blocked);
		}

		private static void SetTile(LevelTileType[] mapData, int x, int y, LevelTileType tileType)
		{
			mapData[(int)(x + y * size.Width)] = tileType;
		}

		public static List<CreepWave> GetListOfWaves()
		{
			return new List<CreepWave>
			{
				new CreepWave(2.0f, 2.0f, "Sand"),
				new CreepWave(2.0f, 6.0f, "Glass"),
				new CreepWave(2.0f, 1.5f, "Glass")
			};
		}
	}
}
