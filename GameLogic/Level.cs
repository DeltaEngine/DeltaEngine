using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Content.Xml;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering3D.Cameras;

namespace DeltaEngine.GameLogic
{
	/// <summary>
	/// Simple 2D map of TileType data to be used for 2D tile games or for game logic tests.
	/// </summary>
	public class Level : ContentData
	{
		protected Level(string contentName)
			: base(contentName) {}

		public Level(Size size)
			: base("<GeneratedLevel" + size + ">")
		{
			InitializeLists();
			Current = this;
			Size = size;
			ModelName = "None";
		}

		public string ModelName { get; set; }

		private void InitializeLists()
		{
			Waves = new List<Wave>();
			Triggers = new List<GameTrigger>();
			SpawnPoints = new List<Vector2D>();
			GoalPoints = new List<Vector2D>();
		}

		public List<Wave> Waves { get; private set; }
		public List<GameTrigger> Triggers { get; private set; }
		public List<Vector2D> SpawnPoints { get; set; }
		public List<Vector2D> GoalPoints { get; set; }
		public static Level Current { get; protected set; }

		public Size Size
		{
			get { return mapSize; }
			set
			{
				mapSize = value;
				MapData = new LevelTileType[(int)mapSize.Width * (int)mapSize.Height];
			}
		}

		private Size mapSize;

		public LevelTileType[] MapData { get; set; }

		protected virtual void UpdateLevelData() {}

		protected override void LoadData(Stream fileData)
		{
			data = new XmlFile(fileData).Root;
			InitializeData();
		}

		protected XmlData data;

		public void InitializeData()
		{
			var mapXml = data.GetChild("Map");
			if (mapXml == null || string.IsNullOrEmpty(mapXml.Value))
				throw new InvalidTileMapData(); //ncrunch: no coverage
			Size = new Size(data.GetAttributeValue("Size"));
			ModelName = data.GetAttributeValue("ModelName");
			InitializeLists();
			StoreSpawnAndGoalPoints();
			StoreWaves();
			StoreGameTriggers();
			InitializeMapFromXml(mapXml.Value);
			LoadCustomLevelData(data);
			Current = this;
		}

		public class InvalidTileMapData : Exception {}

		private void StoreSpawnAndGoalPoints()
		{
			var spawnPoints = data.GetChildren("SpawnPoint");
			foreach (var spawnPoint in spawnPoints)
				SpawnPoints.Add(new Vector2D(spawnPoint.GetAttributeValue("Position")));
			var goalPoints = data.GetChildren("ExitPoint");
			foreach (var goalPoint in goalPoints)
				GoalPoints.Add(new Vector2D(goalPoint.GetAttributeValue("Position")));
		}

		private void StoreWaves()
		{
			var loadedWaves = data.GetChildren("Wave");
			foreach (var loadedWave in loadedWaves)
				Waves.Add(new Wave(loadedWave.GetAttributeValue("WaitTime", 0.0f),
					loadedWave.GetAttributeValue("SpawnInterval", 5.0f),
					loadedWave.GetAttributeValue("SpawnTypeList"), loadedWave.GetAttributeValue("ShortName"),
					loadedWave.GetAttributeValue("MaxTime", 0.0f),
					loadedWave.GetAttributeValue("MaxNumber", 1)));
		}

		protected virtual void StoreGameTriggers() {}

		private void InitializeMapFromXml(string mapXmlData)
		{
			mapXmlData = mapXmlData.Replace("\t", "");
			int x = 0;
			int y = -1;
			foreach (var letter in mapXmlData)
				if (letter == '\n')
				{
					x = 0;
					y++;
				}
				else
				{
					SetTile(new Vector2D(x, y), LetterToTileType(letter));
					x++;
				}
		}

		public void SetTile(Vector2D gridPosition, LevelTileType selectedTileType)
		{
			if (gridPosition.X >= 0 && gridPosition.Y >= 0 && gridPosition.X < mapSize.Width &&
				gridPosition.Y < mapSize.Height)
			{
				int index = GetIndexForMapData(gridPosition);
				MapData[index] = selectedTileType;
				if (TileChanged != null)
					TileChanged(gridPosition);
			}
		}

		public event Action<Vector2D> TileChanged;

		private static LevelTileType LetterToTileType(char letter)
		{
			switch (letter)
			{
			case 'X':
				return LevelTileType.Blocked;
			case 'P':
				return LevelTileType.Placeable;
			case 'L':
				return LevelTileType.BlockedPlaceable;
			case 'R':
				return LevelTileType.Red;
			case 'G':
				return LevelTileType.Green;
			case 'B':
				return LevelTileType.Blue;
			case 'Y':
				return LevelTileType.Yellow;
			case 'O':
				return LevelTileType.Brown;
			case 'A':
				return LevelTileType.Gray;
			case 'S':
				return LevelTileType.SpawnPoint;
			case 'E':
				return LevelTileType.ExitPoint;
			default:
				return LevelTileType.Nothing;
			}
		}

		protected virtual void LoadCustomLevelData(XmlData xmlData) {}

		protected override void DisposeData()
		{
			Current = null;
		}

		public string ToTextForXml()
		{
			string result = Environment.NewLine;
			for (int y = 0; y < Size.Height; y++)
			{
				for (int x = 0; x < Size.Width; x++)
				{
					var index = (int)(x + y * mapSize.Width);
					result += TileToLetterType(MapData[index]);
				}
				result += Environment.NewLine;
			}
			return result;
		}

		private static char TileToLetterType(LevelTileType tileType)
		{
			switch (tileType)
			{
			case LevelTileType.Nothing:
				return '.';
			case LevelTileType.Blocked:
				return 'X';
			case LevelTileType.Placeable:
				return 'P';
			case LevelTileType.BlockedPlaceable:
				return 'L';
			case LevelTileType.Red:
				return 'R';
			case LevelTileType.Green:
				return 'G';
			case LevelTileType.Blue:
				return 'B';
			case LevelTileType.Yellow:
				return 'Y';
			case LevelTileType.Brown:
				return 'O';
			case LevelTileType.Gray:
				return 'A';
			case LevelTileType.SpawnPoint:
				return 'S';
			case LevelTileType.ExitPoint:
				return 'E';
			default:
				throw new ArgumentOutOfRangeException("tileType"); //ncrunch: no coverage
			}
		}

		public Color GetColor(LevelTileType tileType)
		{
			switch (tileType)
			{
			case LevelTileType.Blocked:
				return Color.LightGray;
			case LevelTileType.Placeable:
				return Color.CornflowerBlue;
			case LevelTileType.BlockedPlaceable:
				return Color.LightBlue;
			case LevelTileType.Red:
				return Color.Red;
			case LevelTileType.Green:
				return Color.Green;
			case LevelTileType.Blue:
				return Color.Blue;
			case LevelTileType.Yellow:
				return Color.Yellow;
			case LevelTileType.Brown:
				return Color.Brown;
			case LevelTileType.Gray:
				return Color.Gray;
			case LevelTileType.LightBlue:
				return Color.LightBlue;
			case LevelTileType.SpawnPoint:
				return Color.PaleGreen;
			case LevelTileType.ExitPoint:
				return Color.DarkGreen;
			case LevelTileType.NoSelection:
				return Color.TransparentBlack;
			default:
				return Color.Black;
			}
		}

		public void AddWave(Wave wave)
		{
			Waves.Add(wave);
		}

		public List<Vector2D> GetAllTilesOfType(LevelTileType tileType)
		{
			var list = new List<Vector2D>();
			for (int x = 0; x < Size.Width; x++)
				for (int y = 0; y < Size.Height; y++)
				{
					var index = (int)(x + y * Size.Width);
					if (MapData[index] == tileType)
						list.Add(GetWorldCoordinates(new Vector2D(x, y)));
				}
			return list;
		}

		public Vector2D GetWorldCoordinates(Vector2D mapCoordinates)
		{
			var mapX = (int)mapCoordinates.X + 0.5f;
			var mapY = (int)mapCoordinates.Y + 0.5f;
			var worldX = mapX - Size.Width / 2;
			var worldY = Size.Height / 2 - mapY;
			return new Vector2D(worldX, worldY);
		}

		public void SetTileWithScreenPosition(Vector2D screenPosition, LevelTileType selectedTileType)
		{
			var position = GetIntersectionWithFloor(screenPosition);
			if (position != null && RenderIn3D)
				SetTile(GetMapCoordinates(new Vector2D(position.Value.X, position.Value.Y)),
					selectedTileType);
			else
				SetTile(GetTileIndex(screenPosition), selectedTileType);
		}

		public static Vector3D? GetIntersectionWithFloor(Vector2D screenPosition)
		{
			var ray = Camera.Current.ScreenPointToRay(screenPosition);
			var floor = new Plane(Vector3D.UnitZ, 0.0f);
			return floor.Intersect(ray);
		}

		public bool RenderIn3D { get; set; }

		public Vector2D GetMapCoordinates(Vector2D worldCoordinates)
		{
			var mapCoordinates = new Vector2D(worldCoordinates.X + Size.Width / 2,
				-worldCoordinates.Y + Size.Height / 2);
			return new Vector2D((int)mapCoordinates.X, (int)mapCoordinates.Y);
		}

		public Vector2D GetTileIndex(Vector2D screenPosition)
		{
			return new Vector2D(screenPosition.X / ZoomFactor - (M * Size.Width + B),
				(screenPosition.Y / ZoomFactor - (M * Size.Height + B)));
		}

		private const float ZoomFactor = 0.05f;
		private const float M = -0.5f;
		private const float B = 10.0f;

		public int GetIndexForMapData(Vector2D gridPosition)
		{
			return (int)gridPosition.X + (int)gridPosition.Y * (int)Size.Width;
		}

		public bool IsTileInteractable(Vector2D gridPosition)
		{
			var index = GetIndexForMapData(gridPosition);
			return IsInsideLevelGrid(gridPosition) && MapData[index] != LevelTileType.Blocked &&
				MapData[index] != LevelTileType.SpawnPoint && MapData[index] != LevelTileType.ExitPoint;
		}

		public bool IsInsideLevelGrid(Vector2D gridPosition)
		{
			return (gridPosition.X >= 0) && (gridPosition.X <= Size.Width ) &&
				(gridPosition.Y >= 0) && (gridPosition.Y <= Size.Height);
		}
	}
}