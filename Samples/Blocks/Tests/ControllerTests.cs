using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace Blocks.Tests
{
	/// <summary>
	/// Unit tests for Controller
	/// </summary>
	public class ControllerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			displayMode = ScreenSpace.Current.Viewport.Aspect >= 1.0f
				? Orientation.Landscape : Orientation.Portrait;
			content = new JewelBlocksContent();
			controller = new Controller(displayMode, content);
			sounds = controller.Get<Soundbank>();
			grid = controller.Get<Grid>();
		}

		private Orientation displayMode;
		private Controller controller;
		private JewelBlocksContent content;
		private Soundbank sounds;
		private Grid grid;

		[Test, CloseAfterFirstFrame]
		public void RunCreatesFallingAndUpcomingBlocks()
		{
			AdvanceTimeAndUpdateEntities(0.1f);
			Assert.IsNotNull(controller.FallingBlock);
			Assert.IsNotNull(controller.UpcomingBlock);
		}

		[Test, CloseAfterFirstFrame]
		public void DropSlowAffixesBlocksSlowly()
		{
			controller.IsFallingFast = false;
			controller.FallingBlock = new Block(displayMode, content, Vector2D.One);
			controller.UpcomingBlock = new Block(displayMode, content, Vector2D.One);
			AdvanceTimeAndUpdateEntities(0.1f);
			Assert.AreEqual(0, CountBricks(grid));
			AdvanceTimeAndUpdateEntities(1.5f);
			Assert.AreEqual(0, CountBricks(grid));
			AdvanceTimeAndUpdateEntities(9.0f);
			Assert.AreEqual(4, CountBricks(grid), 1);
		}

		internal static int CountBricks(Grid grid)
		{
			int count = 0;
			foreach (var brick in grid.bricks)
				if (brick != null)
					count++;
			return count;
		}

		[Test, CloseAfterFirstFrame]
		public void DropFastAffixesBlocksQuickly()
		{
			controller.IsFallingFast = true;
			controller.FallingBlock = new Block(displayMode, content, Vector2D.One);
			controller.UpcomingBlock = new Block(displayMode, content, Vector2D.One);
			Assert.AreEqual(0, CountBricks(grid));
			AdvanceTimeAndUpdateEntities(2.5f);
			Assert.GreaterOrEqual(CountBricks(grid), 4);
		}

		[Test, CloseAfterFirstFrame]
		public void ABlockAffixingPlaysASound()
		{
			Assert.IsFalse(sounds.BlockAffixed.IsAnyInstancePlaying);
			AdvanceTimeAndUpdateEntities(9.0f);
		}

		[Test, CloseAfterFirstFrame, Category("Slow")]
		public void RunScoresPointsOverTime()
		{
			int score = 0;
			controller.AddToScore += points => score += points;
			controller.FallingBlock = new Block(displayMode, content, Vector2D.One);
			controller.UpcomingBlock = new Block(displayMode, content, Vector2D.One);
			AdvanceTimeAndUpdateEntities(12.0f);
			Assert.AreEqual(1, score);
			AdvanceTimeAndUpdateEntities(12.0f);
			Assert.GreaterOrEqual(score, 2);
		}

		[Test, CloseAfterFirstFrame]
		public void WhenABlockAffixesTheUpcomingBlockBecomesTheFallingBlock()
		{
			AdvanceTimeAndUpdateEntities(1.0f);
			var upcomingBlock = controller.UpcomingBlock;
			AdvanceTimeAndUpdateEntities(10.0f);
			Assert.AreEqual(upcomingBlock, controller.FallingBlock);
		}

		[Test, CloseAfterFirstFrame]
		public void CantMoveLeftAtLeftWall()
		{
			Assert.IsFalse(sounds.BlockCouldNotMove.IsAnyInstancePlaying);
			controller.FallingBlock = new Block(displayMode, content, new Vector2D(0, 1));
			controller.MoveBlockLeftIfPossible();
			Assert.IsTrue(sounds.BlockCouldNotMove.IsAnyInstancePlaying);
			Assert.AreEqual(0, controller.FallingBlock.Left);
		}

		[Test, CloseAfterFirstFrame]
		public void CanMoveLeftElsewhere()
		{
			Assert.IsFalse(sounds.BlockMoved.IsAnyInstancePlaying);
			controller.FallingBlock = new Block(displayMode, content, new Vector2D(3, 1));
			controller.MoveBlockLeftIfPossible();
			Assert.IsTrue(sounds.BlockMoved.IsAnyInstancePlaying);
			Assert.AreEqual(2, controller.FallingBlock.Left);
		}

		[Test, CloseAfterFirstFrame]
		public void CantMoveRightAtRightWall()
		{
			Assert.IsFalse(sounds.BlockCouldNotMove.IsAnyInstancePlaying);
			controller.FallingBlock = new Block(displayMode, content, new Vector2D(11, 1));
			controller.MoveBlockRightIfPossible();
			Assert.AreEqual(11, controller.FallingBlock.Left);
			Assert.IsTrue(sounds.BlockCouldNotMove.IsAnyInstancePlaying);
		}

		[Test, CloseAfterFirstFrame]
		public void CanMoveRightElsewhere()
		{
			Assert.IsFalse(sounds.BlockMoved.IsAnyInstancePlaying);
			controller.FallingBlock = new Block(displayMode, content, new Vector2D(3, 1));
			controller.MoveBlockRightIfPossible();
			Assert.IsTrue(sounds.BlockMoved.IsAnyInstancePlaying);
			Assert.AreEqual(4, controller.FallingBlock.Left);
		}

		[Test, CloseAfterFirstFrame]
		public void RotateClockwise()
		{
			Assert.IsFalse(sounds.BlockMoved.IsAnyInstancePlaying);
			controller.FallingBlock = new Block(displayMode, content, new Vector2D(8, 1));
			controller.RotateBlockAntiClockwiseIfPossible();
			Assert.IsTrue(sounds.BlockMoved.IsAnyInstancePlaying);
			controller.FallingBlock.Left = 11;
			controller.RotateBlockAntiClockwiseIfPossible();
		}

		[Test, CloseAfterFirstFrame]
		public void LoseIfIsBrickOnTopRow()
		{
			Assert.IsFalse(sounds.GameLost.IsAnyInstancePlaying);
			grid.AffixBlock(new Block(displayMode, content, new Vector2D(1, 0)));
			bool lost = false;
			controller.Lose += () => lost = true;
			AdvanceTimeAndUpdateEntities(0.1f);
			Assert.IsTrue(lost);
			Assert.IsTrue(sounds.GameLost.IsAnyInstancePlaying);
		}

		[Test, CloseAfterFirstFrame]
		public void CurrentBlockBeingNullWillNotCrashMovement()
		{
			controller.FallingBlock = null;
			Assert.DoesNotThrow(controller.MoveBlockLeftIfPossible);
			Assert.DoesNotThrow(controller.MoveBlockRightIfPossible);
			Assert.DoesNotThrow(controller.RotateBlockAntiClockwiseIfPossible);
		}
	}
}