using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D.Cameras;
using NUnit.Framework;

namespace DeltaEngine.Rendering3D.Shapes.Tests
{
	public class Grid3DTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetupCameraFortyFiveDegreesView()
		{
			CreateLookAtCamera(Vector3D.One * 4.0f, Vector3D.Zero);
		}

		private static void CreateLookAtCamera(Vector3D position, Vector3D target)
		{
			var camera = Camera.Use<LookAtCamera>();
			camera.Position = position;
			camera.Target = target;
		}
		
		[Test]
		public void RenderQuadraticGridWithSizeOfOne()
		{
			var grid = CreateQuadraticGrid(1);
			new Line3D(Vector3D.Zero, Vector3D.UnitZ, Color.Red);
			AssertQuadraticGrid(1, grid);
			foreach (var line in grid.lines)
				Assert.IsTrue(line.IsActive);
		}

		private static Grid3D CreateQuadraticGrid(int dimension)
		{
			return new Grid3D(new Size(dimension), 0.2f);
		}

		private static void AssertQuadraticGrid(int expectedDimension, Grid3D grid)
		{
			Assert.AreEqual(new Size(expectedDimension, expectedDimension), grid.Dimension);
		}

		[Test]
		public void RenderQuadraticGridWithSizeOfTen()
		{
			var grid = CreateQuadraticGrid(10);
			AssertQuadraticGrid(10, grid);
		}

		[Test, CloseAfterFirstFrame]
		public void InactivatingInactivatesLines()
		{
			var grid = CreateQuadraticGrid(2);
			grid.IsActive = false;
			foreach (var line in grid.lines)
				Assert.IsFalse(line.IsActive);
		}

		[Test]
		public void CheckGridScale()
		{
			var grid = CreateQuadraticGrid(10);
			Assert.AreEqual(0.2f, grid.GridScale);
			grid.GridScale = 1.0f;
			Assert.AreEqual(1.0f, grid.GridScale);
		}

		[Test]
		public void CheckGridDimension()
		{
			var grid = CreateQuadraticGrid(10);
			Assert.AreEqual(new Size(10, 10), grid.Dimension);
			grid.Dimension = new Size(20, 20);
			Assert.AreEqual(new Size(20, 20), grid.Dimension);
		}

		[Test]
		public void RenderGridWithCenterInOtherPosition()
		{
			var grid = new Grid3D(new Vector3D(5, 5, 0), new Size(1));
			new Line3D(Vector3D.Zero, Vector3D.UnitX, Color.Red);
			new Line3D(Vector3D.Zero, Vector3D.UnitY, Color.Green);
			new Line3D(Vector3D.Zero, Vector3D.UnitZ, Color.Blue);
			AssertQuadraticGrid(1, grid);
			new Line3D(new Vector3D(5, 5, 0), new Vector3D(5, 5, 1), Color.Blue);
			foreach (var line in grid.lines)
				Assert.IsTrue(line.IsActive);
			Assert.AreEqual(new Vector3D(5, 5, 0), grid.Position);
		}

		[Test]
		public void RedrawGridIfItChanges()
		{
			var grid = new Grid3D(new Size(1));
			Assert.AreEqual(4, EntitiesRunner.Current.GetEntitiesOfType<Line3D>().Count);
			grid.Dimension = new Size(10);
			Assert.AreEqual(22, EntitiesRunner.Current.GetEntitiesOfType<Line3D>().Count);
		}

		[Test]
		public void UpdateCenterOfGrid()
		{
			var grid = new Grid3D(new Size(1));
			Assert.AreEqual(Vector3D.Zero, grid.Position);
			grid.Position = new Vector3D(1, 1, 0);
			Assert.AreEqual(4, EntitiesRunner.Current.GetEntitiesOfType<Line3D>().Count);
			Assert.AreEqual(new Vector3D(1, 1, 0), grid.Position);
		}
	}
}