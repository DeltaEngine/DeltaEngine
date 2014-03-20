using DeltaEngine.Datatypes;
using DeltaEngine.GameLogic;

namespace CreepyTowers.Nightmares
{
	internal class LivingRoomLevelInfo
	{
		public static LevelTileType[] MapInfo(Size gridSize)
		{
			size = gridSize;
			var mapData = new LevelTileType[(int)(size.Width * size.Height)];
			for (int i = 0; i < mapData.Length; i++)
				mapData[i] = LevelTileType.Nothing;

			// walls and empty grid spaces
			SetMapValuesToBlocked(mapData, 0, 24, 0, 2);
			SetMapValuesToBlocked(mapData, 0, 24, 24, 26);
			SetMapValuesToBlocked(mapData, 16, 23, 2, 4);
			SetMapValuesToBlocked(mapData, 18, 23, 4, 8);
			SetMapValuesToBlocked(mapData, 21, 23, 8, 12);
			SetMapValuesToBlocked(mapData, 21, 23, 19, 24);
			SetMapValuesToBlocked(mapData, 18, 21, 22, 24);
			SetMapValuesToBlocked(mapData, 1, 7, 21, 24);
			SetMapValuesToBlocked(mapData, 1, 4, 19, 21);
			SetMapValuesToBlocked(mapData, 1, 8, 2, 10);

			SetTile(mapData, 5, 8, LevelTileType.Nothing);
			SetTile(mapData, 6, 8, LevelTileType.Nothing);
			SetTile(mapData, 7, 8, LevelTileType.Nothing);
			SetTile(mapData, 5, 9, LevelTileType.Nothing);
			SetTile(mapData, 6, 9, LevelTileType.Nothing);
			SetTile(mapData, 7, 9, LevelTileType.Nothing);

			SetMapValuesToBlocked(mapData, 8, 17, 22, 24); // tv stand
			SetMapValuesToBlocked(mapData, 22, 23, 12, 15); // book rack to right
			SetMapValuesToBlocked(mapData, 1, 2, 14, 18); //book shelf
			SetMapValuesToBlocked(mapData, 13, 15, 17, 19); // small couch in middle
			SetMapValuesToBlocked(mapData, 13, 15, 2, 4); // small couch at top
			SetMapValuesToBlocked(mapData, 13, 15, 12, 16); // table
			SetMapValuesToBlocked(mapData, 9, 11, 13, 18); // vertical couch
			SetMapValuesToBlocked(mapData, 11, 15, 9, 11); // horizontal couch
			SetMapValuesToBlocked(mapData, 3, 5, 8, 10); // chair near wall

			//standing objects
			SetTile(mapData, 10, 10, LevelTileType.Blocked); // book stand near couch
			SetTile(mapData, 10, 18, LevelTileType.Blocked); // stand near couch
			SetTile(mapData, 22, 17, LevelTileType.Blocked); // tree pot near bookshelf
			SetTile(mapData, 15, 3, LevelTileType.Blocked); // tree pot near wall
			SetTile(mapData, 7, 7, LevelTileType.Blocked); // lamp near wall
			SetTile(mapData, 20, 8, LevelTileType.Blocked); // lamp near chair
			SetTile(mapData, 19, 9, LevelTileType.Blocked); // chair to left
			SetTile(mapData, 9, 12, LevelTileType.Blocked); // chair near couch

			//ball
			SetTile(mapData, 18, 8, LevelTileType.Blocked);
			SetTile(mapData, 9, 8, LevelTileType.Blocked);

			// spawn points
			SetTile(mapData, 1, 9, LevelTileType.SpawnPoint);
			SetTile(mapData, 8, 3, LevelTileType.SpawnPoint);

			// exit points
			SetTile(mapData, 19, 20, LevelTileType.ExitPoint);

			//Tower Placeable Points
			SetTile(mapData, 10, 4, LevelTileType.LightBlue); //ice tower
			SetTile(mapData, 4, 13, LevelTileType.Green); //acid tower
			SetTile(mapData, 18, 18, LevelTileType.Gray); //slice tower

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
	}
}