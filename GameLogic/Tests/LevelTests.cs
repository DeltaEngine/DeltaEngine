using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D.Cameras;
using NUnit.Framework;

namespace DeltaEngine.GameLogic.Tests
{
	public class LevelTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void Start()
		{
			TileMap map = CreateSimple2X3TileMap();
			Assert.AreEqual(2, map.Width);
			Assert.AreEqual(3, map.Height);
		}

		private static TileMap CreateSimple2X3TileMap()
		{
			var tilesData = new[]
			{
				new CompleteSquare(LevelTileType.ExitPoint), new CompleteSquare(LevelTileType.Nothing),
				new CompleteSquare(LevelTileType.Blocked), new CompleteSquare(LevelTileType.Nothing),
				new CompleteSquare(LevelTileType.SpawnPoint), new CompleteSquare(LevelTileType.Nothing)
			};
			return new TileMap(tilesData, new Size(2, 3));
		}

		private class TileMap
		{
			public TileMap(CompleteSquare[] mapData, Size size)
			{
				this.mapData = mapData;
				this.size = size;
			}

			private readonly CompleteSquare[] mapData;
			private Size size;

			public int Width
			{
				get { return (int)size.Width; }
			}

			public int Height
			{
				get { return (int)size.Height; }
			}

			public TileMap(string mapDataAsString)
			{
				if (String.IsNullOrWhiteSpace(mapDataAsString))
					throw new NoTileMapData();
				string[] tileLines = mapDataAsString.SplitAndTrim(Environment.NewLine);
				mapData = new CompleteSquare[tileLines.Length * tileLines[0].Length];
				size = new Size(tileLines[0].Length, tileLines.Length);
				for (int lineIndex = 0; lineIndex < Height; lineIndex++)
					for (int symbolIndex = 0; symbolIndex < Width; symbolIndex++)
					{
						var index = symbolIndex + lineIndex * Width;
						mapData[index] = new CompleteSquare(tileLines[lineIndex][symbolIndex]);
					}
			}

			public class NoTileMapData : Exception {}

			public string Save()
			{
				string data = "";
				for (int i = 0; i < Height; i++)
					SaveTileLine(i, ref data);
				return data;
			}

			private void SaveTileLine(int mapLineIndex, ref string data)
			{
				for (int i = 0; i < Width; i++)
					data += mapData[i + mapLineIndex * Width].ToTypeSymbol();
				data += Environment.NewLine;
			}

			public void FindPath(LevelTileType typeOfStartTile)
			{
				Vector2D startPoint = FindTile(typeOfStartTile);
				if (startPoint.Equals(Vector2D.Unused))
					throw new StartPointNotFound();
				var tileCoordinates = new Vector2D[Width * Height];
				for (int lineIndex = 0; lineIndex < Height; lineIndex++)
					for (int symbolIndex = 0; symbolIndex < Width; symbolIndex++)
						tileCoordinates[symbolIndex + lineIndex * Width] = new Vector2D(symbolIndex, lineIndex);
			}

			public class StartPointNotFound : Exception {}

			private Vector2D FindTile(LevelTileType typeOfTile)
			{
				for (int lineIndex = 0; lineIndex < Height; lineIndex++)
					for (int symbolIndex = 0; symbolIndex < Width; symbolIndex++)
						if (mapData[lineIndex * Width + symbolIndex].Type == typeOfTile)
							return new Vector2D(lineIndex, symbolIndex);
				return Vector2D.Unused;
			}
		}

		[Test, CloseAfterFirstFrame]
		public void SaveAndLoadMapData()
		{
			TileMap map = CreateSimple2X3TileMap();
			string mapData = map.Save();
			string newLine = Environment.NewLine;
			string expectedData = "E." + newLine + "X." + newLine + "S." + newLine;
			Assert.AreEqual(expectedData, mapData);
			var loadedMap = new TileMap(mapData);
			Assert.AreEqual(expectedData, loadedMap.Save());
		}

		[Test, CloseAfterFirstFrame]
		public void CanNotCreateTileMapWithoutData()
		{
			const string MapData = "";
			Assert.Throws<TileMap.NoTileMapData>(() => new TileMap(MapData));
		}

		[Test, CloseAfterFirstFrame]
		public void CanNotCreateTileMapWithoutValidData()
		{
			const string MapData = "   ";
			Assert.Throws<TileMap.NoTileMapData>(() => new TileMap(MapData));
		}

		[Test, CloseAfterFirstFrame]
		public void Start2()
		{
			TileMap map = CreateSimple2X3TileMap();
			Assert.Throws<TileMap.StartPointNotFound>(() => map.FindPath(LevelTileType.Red));
			Assert.DoesNotThrow(() => map.FindPath(LevelTileType.SpawnPoint));
		}

		[Test, CloseAfterFirstFrame]
		public void TestLevelTileChange()
		{
			var level = new MockLevel(new Size(1));
			level.TileChanged += position => CheckTileHasChanged(level, position);
			level.SetTile(Vector2D.Zero, LevelTileType.Blocked);
		}

		private static void CheckTileHasChanged(Level level, Vector2D position)
		{
			var index = (int)(position.X + position.Y * level.Size.Width);
			Assert.AreEqual(LevelTileType.Blocked, level.MapData[index]);
		}

		[Test, CloseAfterFirstFrame]
		public void TestLevelTileColor()
		{
			var level = new MockLevel(new Size(5));
			Assert.AreEqual(Color.LightGray, level.GetColor(LevelTileType.Blocked));
			Assert.AreEqual(Color.CornflowerBlue, level.GetColor(LevelTileType.Placeable));
			Assert.AreEqual(Color.LightBlue, level.GetColor(LevelTileType.BlockedPlaceable));
			Assert.AreEqual(Color.LightBlue, level.GetColor(LevelTileType.LightBlue));
			Assert.AreEqual(Color.Red, level.GetColor(LevelTileType.Red));
			Assert.AreEqual(Color.Green, level.GetColor(LevelTileType.Green));
			Assert.AreEqual(Color.Blue, level.GetColor(LevelTileType.Blue));
			Assert.AreEqual(Color.Yellow, level.GetColor(LevelTileType.Yellow));
			Assert.AreEqual(Color.Brown, level.GetColor(LevelTileType.Brown));
			Assert.AreEqual(Color.Gray, level.GetColor(LevelTileType.Gray));
			Assert.AreEqual(Color.PaleGreen, level.GetColor(LevelTileType.SpawnPoint));
			Assert.AreEqual(Color.DarkGreen, level.GetColor(LevelTileType.ExitPoint));
			Assert.AreEqual(Color.Black, level.GetColor(LevelTileType.Nothing));
			Assert.AreEqual(Color.TransparentBlack, level.GetColor(LevelTileType.NoSelection));
		}

		[Test, CloseAfterFirstFrame]
		public void TestGetAllTilesOfType()
		{
			var level = new MockLevel(new Size(2));
			level.SetTile(new Vector2D(0, 0), LevelTileType.SpawnPoint);
			level.SetTile(new Vector2D(0, 1), LevelTileType.SpawnPoint);
			Assert.AreEqual(2, level.GetAllTilesOfType(LevelTileType.SpawnPoint).Count);
		}

		[Test, CloseAfterFirstFrame]
		public void TestIsInsideLevelGrid()
		{
			var level = new MockLevel(new Size(3));
			Assert.True(level.IsInsideLevelGrid(new Vector2D(1, 1)));
			Assert.False(level.IsInsideLevelGrid(new Vector2D(4, 1)));
		}

		[Test, CloseAfterFirstFrame]
		public void TestAddWave()
		{
			var level = new MockLevel(new Size(3));
			Assert.AreEqual(0, level.Waves.Count);
			level.AddWave(new Wave(0.0f, 0.0f, ""));
			Assert.AreEqual(1, level.Waves.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void TestToTextForXml()
		{
			var level = new MockLevel(new Size(3, 4));
			level.SetTile(new Vector2D(0, 0), LevelTileType.Nothing);
			level.SetTile(new Vector2D(1, 0), LevelTileType.Blocked);
			level.SetTile(new Vector2D(2, 0), LevelTileType.Placeable);
			level.SetTile(new Vector2D(0, 1), LevelTileType.BlockedPlaceable);
			level.SetTile(new Vector2D(1, 1), LevelTileType.Red);
			level.SetTile(new Vector2D(2, 1), LevelTileType.Green);
			level.SetTile(new Vector2D(0, 2), LevelTileType.Blue);
			level.SetTile(new Vector2D(1, 2), LevelTileType.Yellow);
			level.SetTile(new Vector2D(2, 2), LevelTileType.Brown);
			level.SetTile(new Vector2D(0, 3), LevelTileType.Gray);
			level.SetTile(new Vector2D(1, 3), LevelTileType.SpawnPoint);
			level.SetTile(new Vector2D(2, 3), LevelTileType.ExitPoint);
			string text = level.ToTextForXml();
			Assert.AreEqual("\r\n.XP\r\nLRG\r\nBYO\r\nASE\r\n", text);
		}

		[Test, CloseAfterFirstFrame]
		public void TestLoadData()
		{
			var level = new MockLevel(new Size());
			Assert.AreEqual(new Size(), level.Size);
			level.LoadTestData();
			Assert.AreEqual(new Size(4, 3), level.Size);
		}

		[Test, CloseAfterFirstFrame]
		public void ConvertMapCoordinatesToWorldCoordinates()
		{
			var level = new Level(new Size(3, 5));
			level.RenderIn3D = true;
			Assert.AreEqual(new Vector2D(0, 0), level.GetWorldCoordinates(new Vector2D(1, 2)));
			Assert.AreEqual(new Vector2D(-1, 2), level.GetWorldCoordinates(new Vector2D(0, 0)));
			Assert.AreEqual(new Vector2D(1, 2), level.GetWorldCoordinates(new Vector2D(2, 0)));
			Assert.AreEqual(new Vector2D(-1, -2), level.GetWorldCoordinates(new Vector2D(0, 4)));
			Assert.AreEqual(new Vector2D(1, -2), level.GetWorldCoordinates(new Vector2D(2, 4)));
		}

		[Test, CloseAfterFirstFrame]
		public void FetchMapCoordinatesOfClosestTileCenter()
		{
			var level = new Level(new Size(3, 3));
			level.RenderIn3D = true;
			Assert.AreEqual(new Vector2D(0, 0), level.GetWorldCoordinates(new Vector2D(1.1f, 1.1f)));
			Assert.AreEqual(new Vector2D(0, 0), level.GetWorldCoordinates(new Vector2D(1.1f, 1.9f)));
			Assert.AreEqual(new Vector2D(0, 0), level.GetWorldCoordinates(new Vector2D(1.9f, 1.9f)));
			Assert.AreEqual(new Vector2D(0, 0), level.GetWorldCoordinates(new Vector2D(1.9f, 1.1f)));
		}

		[Test, CloseAfterFirstFrame]
		public void ConvertWoldCoordinatesToMapCoordinates()
		{
			var level = new Level(new Size(3, 5));
			level.RenderIn3D = true;
			Assert.AreEqual(new Vector2D(1, 2), level.GetMapCoordinates(new Vector2D(0, 0)));
			Assert.AreEqual(new Vector2D(0, 0), level.GetMapCoordinates(new Vector2D(-1, 2)));
			Assert.AreEqual(new Vector2D(2, 0), level.GetMapCoordinates(new Vector2D(1, 2)));
			Assert.AreEqual(new Vector2D(0, 4), level.GetMapCoordinates(new Vector2D(-1, -2)));
			Assert.AreEqual(new Vector2D(2, 4), level.GetMapCoordinates(new Vector2D(1, -2)));
		}

		[Test, CloseAfterFirstFrame]
		public void TestSetTileWithScreenPositionIn2D()
		{
			var level = new MockLevel(new Size(3));
			SetTilesWithScreenPosition(level);
			AssertTilesWithScreenPosition(level);
		}

		private void SetTilesWithScreenPosition(Level level)
		{
			level.SetTileWithScreenPosition(topLeft.Position, topLeft.Type);
			level.SetTileWithScreenPosition(bottomLeft.Position, bottomLeft.Type);
			level.SetTileWithScreenPosition(bottomRight.Position, bottomRight.Type);
			level.SetTileWithScreenPosition(topRight.Position, topRight.Type);
			level.SetTileWithScreenPosition(middle.Position, middle.Type);
			level.SetTileWithScreenPosition(unitX.Position, unitX.Type);
			level.SetTileWithScreenPosition(unitY.Position, unitY.Type);
		}

		private readonly LevelTile topLeft = new LevelTile(new Vector2D(0.45f, 0.45f),
			LevelTileType.Blue);
		private readonly LevelTile bottomLeft = new LevelTile(new Vector2D(0.45f, 0.55f),
			LevelTileType.Yellow);
		private readonly LevelTile bottomRight = new LevelTile(new Vector2D(0.55f, 0.55f),
			LevelTileType.Brown);
		private readonly LevelTile topRight = new LevelTile(new Vector2D(0.55f, 0.45f),
			LevelTileType.LightBlue);
		private readonly LevelTile middle = new LevelTile(new Vector2D(0.5f, 0.5f),
			LevelTileType.Gray);
		private readonly LevelTile unitX = new LevelTile(new Vector2D(0.5f, 0.45f), LevelTileType.Red);
		private readonly LevelTile unitY = new LevelTile(new Vector2D(0.45f, 0.5f),
			LevelTileType.Green);

		private class LevelTile
		{
			public LevelTile(Vector2D position, LevelTileType type)
			{
				Position = position;
				Type = type;
			}

			public Vector2D Position { get; private set; }
			public LevelTileType Type { get; private set; }
		}

		private void AssertTilesWithScreenPosition(Level level)
		{
			Assert.AreEqual(topLeft.Type, level.MapData[0]);
			Assert.AreEqual(bottomLeft.Type, level.MapData[6]);
			Assert.AreEqual(bottomRight.Type, level.MapData[8]);
			Assert.AreEqual(topRight.Type, level.MapData[2]);
			Assert.AreEqual(middle.Type, level.MapData[4]);
			Assert.AreEqual(unitX.Type, level.MapData[1]);
			Assert.AreEqual(unitY.Type, level.MapData[3]);
		}

		[Test, CloseAfterFirstFrame]
		public void TestSetTileWithScreenPositionIn3D()
		{
			SetUpCamera();
			var level = new MockLevel(new Size(3)) { RenderIn3D = true };
			SetTilesWithScreenPosition(level);
			AssertTilesWithScreenPosition(level);
		}

		private void SetUpCamera()
		{
			Camera.Use<LookAtCamera>();
			var camera = new LookAtCamera(Resolve<Device>(), Resolve<Window>());
			Camera.Current = camera;
			camera.Target = Vector3D.Zero;
			camera.Position = new Vector3D(0.0f, -0.00001f, 10.0f);
		}

		[Test, CloseAfterFirstFrame]
		public void InteractWithOnlyInteractableTiles()
		{
			var level = new Level(new Size(2, 2));
			level.RenderIn3D = true;
			level.SetTile(new Vector2D(0, 0), LevelTileType.Blocked);
			level.SetTile(new Vector2D(1, 0), LevelTileType.ExitPoint);
			level.SetTile(new Vector2D(0, 1), LevelTileType.SpawnPoint);
			level.SetTile(new Vector2D(1, 1), LevelTileType.Placeable);
			Assert.IsFalse(level.IsTileInteractable(new Vector2D(0, 0)));
			Assert.IsFalse(level.IsTileInteractable(new Vector2D(1, 0)));
			Assert.IsFalse(level.IsTileInteractable(new Vector2D(0, 1)));
			Assert.IsTrue(level.IsTileInteractable(new Vector2D(1, 1)));
		}
	}
}