using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace DeltaEngine.Rendering2D.Shapes.Tests
{
	public class Grid2DTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void Setup2DCamera()
		{
			camera = new Camera2DScreenSpace(Resolve<Window>());
		}

		private Camera2DScreenSpace camera;

		[TearDown]
		public void Dispose2DCamera()
		{
			camera.Dispose();
		}

		[Test]
		public void RenderQuadraticGridWithSizeOfOne()
		{
			var grid = new Grid2D(new Size(2), Vector2D.Half);
			AssertQuadraticGrid(2, grid);
			foreach (var line in grid.lines)
				Assert.IsTrue(line.IsActive);
		}

		private static void AssertQuadraticGrid(int expectedDimension, Grid2D grid)
		{
			Assert.AreEqual(new Size(expectedDimension, expectedDimension), grid.Dimension);
		}

		[Test]
		public void RenderQuadraticGridWithSizeOfTen()
		{
			var grid = new Grid2D(new Size(10), Vector2D.Half);
			AssertQuadraticGrid(10, grid);
		}

		[Test, CloseAfterFirstFrame]
		public void InactivatingInactivatesLines()
		{
			var grid = new Grid2D(new Size(2), Vector2D.Half);
			grid.IsActive = false;
			foreach (var line in grid.lines)
				Assert.IsFalse(line.IsActive);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeDimensionAndScale()
		{
			var grid = new Grid2D(new Size(2), Vector2D.Half);
			grid.Dimension = new Size(4);
			grid.GridScale = 2;
			Assert.AreEqual(2, grid.GridScale);
		}
	}
}
