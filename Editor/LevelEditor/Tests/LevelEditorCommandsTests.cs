using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.GameLogic;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.LevelEditor.Tests
{
	public class LevelEditorCommandsTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			viewModel = new LevelEditorViewModel(new MockService("Test", "Test"));
			commands = new LevelEditorCommands(viewModel);
		}

		private LevelEditorViewModel viewModel;
		private LevelEditorCommands commands;

		[Test, CloseAfterFirstFrame]
		public void LeftMouseClickSetsStartDragIndex()
		{
			commands.LeftMouseButton(new Vector2D(0.5f, 0.5f));
			Assert.AreEqual(78, commands.StartDragIndex);
		}

		[Test, CloseAfterFirstFrame]
		public void LeftMouseClickIsNotInsideGrid()
		{
			commands.LeftMouseButton(new Vector2D(1.0f, 1.0f));
			Assert.AreEqual(0, commands.StartDragIndex);
		}

		[Test, CloseAfterFirstFrame]
		public void LeftMouseClickSetsStartDragIndexIn3D()
		{
			viewModel.Is3D = true;
			commands.LeftMouseButton(new Vector2D(1.0f, 1.0f));
			Assert.AreEqual(16, commands.StartDragIndex);
		}

		[Test, CloseAfterFirstFrame]
		public void LeftMouseClickIsNotInsideGrid3D()
		{
			viewModel.Is3D = true;
			commands.LeftMouseButton(new Vector2D(-1.0f, -1.0f));
			Assert.AreEqual(0, commands.StartDragIndex);
		}

		[Test, CloseAfterFirstFrame]
		public void LeftMouseDragSetsTileToRed()
		{
			commands.SelectedTileType = LevelTileType.Red;
			commands.LeftMouseDrag(new Vector2D(0.2f, 0.2f));
			Assert.AreEqual(1, commands.Level.GetAllTilesOfType(LevelTileType.Red).Count);
		}

		[Test, CloseAfterFirstFrame]
		public void LeftMouseDragIsNotInsideGrid()
		{
			commands.SelectedTileType = LevelTileType.Red;
			commands.LeftMouseDrag(new Vector2D(1.0f, 1.0f));
			Assert.AreEqual(0, commands.Level.GetAllTilesOfType(LevelTileType.Red).Count);
		}

		[Test, CloseAfterFirstFrame]
		public void LeftMouseDragSetsTileToRed3D()
		{
			viewModel.Is3D = true;
			viewModel.renderer.RecreateTiles();
			commands.SelectedTileType = LevelTileType.Red;
			commands.LeftMouseDrag(new Vector2D(1.2f, 1.2f));
			Assert.AreEqual(1, commands.Level.GetAllTilesOfType(LevelTileType.Red).Count);
		}

		[Test, CloseAfterFirstFrame]
		public void LeftMouseDragIsNotInsideGrid3D()
		{
			viewModel.Is3D = true;
			commands.SelectedTileType = LevelTileType.Red;
			commands.LeftMouseDrag(new Vector2D(-1.0f, -1.0f));
			Assert.AreEqual(0, commands.Level.GetAllTilesOfType(LevelTileType.Red).Count);
		}

		[Test, CloseAfterFirstFrame]
		public void SetColoredTileToNothing()
		{
			commands.SelectedTileType = LevelTileType.Red;
			commands.LeftMouseDrag(new Vector2D(0.2f, 0.2f));
			Assert.AreEqual(1, commands.Level.GetAllTilesOfType(LevelTileType.Red).Count);
			commands.SetTileToNothingAndRemoveLevelObject(new Vector2D(0.2f, 0.2f));
			Assert.AreEqual(0, commands.Level.GetAllTilesOfType(LevelTileType.Red).Count);
		}

		[Test, CloseAfterFirstFrame]
		public void SetColoredTileToNothing3D()
		{
			viewModel.Is3D = true;
			viewModel.renderer.RecreateTiles();
			commands.SelectedTileType = LevelTileType.Red;
			commands.LeftMouseDrag(new Vector2D(1.2f, 1.2f));
			Assert.AreEqual(1, commands.Level.GetAllTilesOfType(LevelTileType.Red).Count);
			commands.SetTileToNothingAndRemoveLevelObject(new Vector2D(1.2f, 1.2f));
			Assert.AreEqual(0, commands.Level.GetAllTilesOfType(LevelTileType.Red).Count);
		}

		[Test, CloseAfterFirstFrame]
		public void DoesNotSetColoredTileToNothingIfNotInsideGrid()
		{
			commands.SelectedTileType = LevelTileType.Red;
			commands.SetTileToNothingAndRemoveLevelObject(new Vector2D(1.2f, 1.2f));
			Assert.AreEqual(0, commands.Level.GetAllTilesOfType(LevelTileType.Red).Count);
		}

		[Test, CloseAfterFirstFrame]
		public void DoesNotSetColoredTileToNothingIfNotInsideGrid3D()
		{
			viewModel.Is3D = true;
			viewModel.renderer.RecreateTiles();
			commands.SelectedTileType = LevelTileType.Red;
			commands.SetTileToNothingAndRemoveLevelObject(new Vector2D(7.2f, 7.2f));
			Assert.AreEqual(0, commands.Level.GetAllTilesOfType(LevelTileType.Red).Count);
		}

		//ncrunch: no coverage start
		[Test, Ignore]
		public void Zoom()
		{
			commands.Zoom();
		}

		[Test, Ignore]
		public void MiddleMouseDrag()
		{
			commands.MiddleMouseDrag();
		}
	}
}