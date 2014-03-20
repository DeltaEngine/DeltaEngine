using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace Blocks.Tests
{
	/// <summary>
	/// Unit tests for Block
	/// </summary>
	public class BlockTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			displayMode = ScreenSpace.Current.Viewport.Aspect >= 1.0f
				? Orientation.Landscape : Orientation.Portrait;
			content = new JewelBlocksContent();
		}

		private Orientation displayMode;
		private JewelBlocksContent content;

		[Test, CloseAfterFirstFrame]
		public void ConstructorTopLeft()
		{
			var block = new Block(displayMode, content, new Vector2D(1, 2));
			Assert.AreEqual(1, block.Left);
			Assert.AreEqual(2, block.Top);
		}

		[Test, CloseAfterFirstFrame]
		public void RotateClockwise()
		{
			var block = new Block(displayMode, content, new Vector2D(8, 1));
			block.RotateClockwise();
		}

		[Test, CloseAfterFirstFrame]
		public void RotateAntiClockwise()
		{
			var block = new Block(displayMode, content, new Vector2D(8, 1));
			block.RotateAntiClockwise();
		}

		[Test, CloseAfterFirstFrame]
		public void Left()
		{
			var shape = new Block(displayMode, content, Vector2D.Zero) { Left = 1 };
			Assert.AreEqual(1, shape.Left);
			Assert.AreEqual(1, shape.Bricks[0].TopLeftGridCoord.X);
			Assert.AreEqual(1, shape.Bricks[1].TopLeftGridCoord.X);
			Assert.AreEqual(1, shape.Bricks[2].TopLeftGridCoord.X);
			Assert.AreEqual(1, shape.Bricks[3].TopLeftGridCoord.X);
		}

		[Test, CloseAfterFirstFrame]
		public void Top()
		{
			var shape = new Block(displayMode, content, Vector2D.Zero) { Top = 1 };
			Assert.AreEqual(1, shape.Top);
			Assert.AreEqual(1, shape.Bricks[0].TopLeftGridCoord.Y);
			Assert.AreEqual(1, shape.Bricks[1].TopLeftGridCoord.Y);
			Assert.AreEqual(1, shape.Bricks[2].TopLeftGridCoord.Y);
			Assert.AreEqual(1, shape.Bricks[3].TopLeftGridCoord.Y);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckIBlockAppearsATenthOfTheTime()
		{
			int count = 0;
			for (int i = 0; i < 1000; i++)
			{
				var block = new Block(displayMode, content, Vector2D.Zero);
				if (block.ToString() == "OOOO/..../..../...." || block.ToString() == "O.../O.../O.../O...")
					count++;
			}

			Assert.AreEqual(100, count, 50);
		}

		[Test, CloseAfterFirstFrame]
		public void RenderJBlock()
		{
			{
				var block = new Block(displayMode, content, Vector2D.Zero);
				block.UpdateBrickDrawAreas(0.0f);
			}
		}
	}
}