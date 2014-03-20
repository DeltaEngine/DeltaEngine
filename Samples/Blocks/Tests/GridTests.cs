using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace Blocks.Tests
{
	/// <summary>
	/// Unit tests for Grid
	/// </summary>
	public class GridTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			displayMode = ScreenSpace.Current.Viewport.Aspect >= 1.0f
				? Orientation.Landscape : Orientation.Portrait;
			content = new JewelBlocksContent();
			controller = new Controller(displayMode, content);
			grid = controller.Get<Grid>();
			//fixedRandomScope = NUnit.Framework.Randomizer.Use(new FixedRandom());
		}

		private Orientation displayMode;
		//private IDisposable fixedRandomScope;
		private JewelBlocksContent content;
		private Grid grid;
		private Controller controller;

		//ncrunch: no coverage start
		[Test, Ignore]
		public void AffixBlocksWhichFillOneRow()
		{
			Assert.AreEqual(0,
				AffixBlocks(grid, new[] { new Vector2D(0, 18), new Vector2D(4, 18), new Vector2D(7, 18) }));
			//using (NUnit.Framework.Randomizer.Use(new FixedRandom(IBlock)))
			//{
				Assert.AreEqual(1, grid.AffixBlock(new Block(displayMode, content, new Vector2D(11, 15))));
				Assert.AreEqual(3, ControllerTests.CountBricks(grid));
				Assert.IsNotNull(grid.bricks[11, 16]);
				Assert.IsNotNull(grid.bricks[11, 17]);
				Assert.IsNotNull(grid.bricks[11, 18]);
			//}
		}
		//ncrunch: no coverage end

		//private static readonly float[] IBlock = new[]
		//{ 0.0f, 0.0f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f };

		//ncrunch: no coverage start
		private int AffixBlocks(Grid gameGrid, IEnumerable<Vector2D> points)
		{
			return points.Sum(point => gameGrid.AffixBlock(new Block(displayMode, content, point)));
		}

		[Test, Ignore]
		public void AffixBlocksWhichFillTwoRows()
		{
			Assert.AreEqual(0,
				AffixBlocks(grid,
					new[]
					{
						new Vector2D(0, 17), new Vector2D(4, 17), new Vector2D(7, 17), new Vector2D(0, 18),
						new Vector2D(4, 18), new Vector2D(7, 18)
					}));
			//using (NUnit.Framework.Randomizer.Use(new FixedRandom(IBlock)))
			//{
			Assert.AreEqual(2, grid.AffixBlock(new Block(displayMode, content, new Vector2D(11, 15))));
			Assert.AreEqual(2, ControllerTests.CountBricks(grid));
			Assert.IsNotNull(grid.bricks[11, 17]);
			Assert.IsNotNull(grid.bricks[11, 18]);
			//}
		}

		[Test, Ignore]
		public void RowsDontSplit()
		{
			content.DoBricksSplitInHalfWhenRowFull = false;
			Assert.AreEqual(0,
				AffixBlocks(grid, new[] { new Vector2D(0, 18), new Vector2D(4, 18), new Vector2D(7, 18) }));
			//using (NUnit.Framework.Randomizer.Use(new FixedRandom(IBlock)))
			//{
				Assert.AreEqual(1, grid.AffixBlock(new Block(displayMode, content, new Vector2D(11, 15))));
				AdvanceTimeAndUpdateEntities();
				//Assert.AreEqual(30, entitySystem.GetHandler<Render>().NumberOfActiveRenderableObjects);
			//}
		}

		[Test, Ignore]
		public void RowsSplit()
		{
			content.DoBricksSplitInHalfWhenRowFull = true;
			Assert.AreEqual(0,
				AffixBlocks(grid, new[] { new Vector2D(0, 18), new Vector2D(4, 18), new Vector2D(7, 18) }));
			//using (NUnit.Framework.Randomizer.Use(new FixedRandom(IBlock)))
			//{
				Assert.AreEqual(1, grid.AffixBlock(new Block(displayMode, content, new Vector2D(11, 15))));
				AdvanceTimeAndUpdateEntities();
				//Assert.AreEqual(42, entitySystem.GetHandler<Render>().NumberOfActiveRenderableObjects);
			//}
		}

		[Test, Ignore]
		public void IsValidPositionInEmptyGrid()
		{
			Assert.IsFalse(grid.IsValidPosition(new Block(displayMode, content, new Vector2D(-1, 1))));
			Assert.IsFalse(grid.IsValidPosition(new Block(displayMode, content, new Vector2D(9, 1))));
			Assert.IsFalse(grid.IsValidPosition(new Block(displayMode, content, new Vector2D(0, 0))));
			//using (NUnit.Framework.Randomizer.Use(new FixedRandom(IBlock)))
				Assert.IsFalse(grid.IsValidPosition(new Block(displayMode, content, new Vector2D(0, 17))));

			Assert.IsTrue(grid.IsValidPosition(new Block(displayMode, content, new Vector2D(2, 2))));
		}

		[Test, Ignore]
		public void IsValidPositionInOccupiedGrid()
		{
			grid.AffixBlock(new Block(displayMode, content, new Vector2D(5, 1)));
			Assert.IsFalse(grid.IsValidPosition(new Block(displayMode, content, new Vector2D(3, 0))));
			Assert.IsTrue(grid.IsValidPosition(new Block(displayMode, content, new Vector2D(5, 2))));
		}
		//ncrunch: no coverage end

		[Test]
		public void IsABrickOnFirstRow()
		{
			var lost = false;
			controller.Lose += () => { lost = true; };
			Assert.IsFalse(grid.IsABrickOnFirstRow());
			grid.AffixBlock(new Block(displayMode, content, new Vector2D(1, 1)));
			Assert.IsFalse(grid.IsABrickOnFirstRow());
			grid.AffixBlock(new Block(displayMode, content, new Vector2D(2, 0)));
			Assert.IsTrue(grid.IsABrickOnFirstRow());
			AdvanceTimeAndUpdateEntities();
			Assert.IsTrue(lost);
		}

		//ncrunch: no coverage start
		[Test, Ignore]
		public void Clear()
		{
			//using (NUnit.Framework.Randomizer.Use(new FixedRandom(new[] { 0.8f, 0.0f })))
			//{
			grid.AffixBlock(new Block(displayMode, content, Vector2D.One));
				Assert.AreEqual(4, ControllerTests.CountBricks(grid));
				AdvanceTimeAndUpdateEntities();
				grid.Clear();
				Assert.AreEqual(0, ControllerTests.CountBricks(grid));
			//}
		}

		[Test, Ignore]
		public void GetValidStartingColumns()
		{
			content.DoBlocksStartInARandomColumn = true;
			var block = new Block(displayMode, content, Vector2D.Zero);
			Assert.AreEqual(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, grid.GetValidStartingColumns(block));
			grid.AffixBlock(new Block(displayMode, content, new Vector2D(1, 1)));
			Assert.AreEqual(new[] { 5, 6, 7, 8 }, grid.GetValidStartingColumns(block));
		}

		[Test, Ignore]
		public void GetSingleValidStartingColumn()
		{
			content.DoBlocksStartInARandomColumn = false;
			var block = new Block(displayMode, content, Vector2D.Zero);
			Assert.AreEqual(new[] { 4 }, grid.GetValidStartingColumns(block));
		}
		//ncrunch: no coverage end

		[TestFixtureTearDown]
		public void TearDown()
		{
			//if (fixedRandomScope != null)
			//	fixedRandomScope.Dispose();
		}
	}
}