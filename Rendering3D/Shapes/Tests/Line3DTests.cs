using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D.Cameras;
using NUnit.Framework;

namespace DeltaEngine.Rendering3D.Shapes.Tests
{
	public class Line3DTests : TestWithMocksOrVisually
	{
		[Test]
		public void RenderCoordinateSystemCross()
		{
			CreateLookAtCamera(Vector3D.One * 4.0f, Vector3D.Zero);
			new Line3D(-Vector3D.UnitX, Vector3D.UnitX * 3, Color.Red);
			new Line3D(-Vector3D.UnitY, Vector3D.UnitY * 3, Color.Green);
			new Line3D(-Vector3D.UnitZ, Vector3D.UnitZ * 3, Color.Blue);
		}
		
		private static void CreateLookAtCamera(Vector3D position, Vector3D target)
		{
			var camera = Camera.Use<LookAtCamera>();
			camera.Position = position;
			camera.Target = target;
		}

		[Test]
		public void RenderGrid()
		{
			const int GridSize = 10;
			const float GridScale = 0.5f;
			const float HalfGridSize = GridSize * 0.5f;
			var axisXz = new Vector2D(-HalfGridSize, -HalfGridSize);
			CreateLookAtCamera(Vector3D.One * 4.0f, Vector3D.Zero);
			for (int i = 0; i <= GridSize; i++, axisXz.X += 1, axisXz.Y += 1)
			{
				new Line3D(new Vector3D(-HalfGridSize * GridScale, axisXz.Y * GridScale, 0.0f),
					new Vector3D(HalfGridSize * GridScale, axisXz.Y * GridScale, 0.0f), Color.White);
				new Line3D(new Vector3D(axisXz.X * GridScale, -HalfGridSize * GridScale, 0.0f),
					new Vector3D(axisXz.X * GridScale, HalfGridSize * GridScale, 0.0f), Color.White);
			}
		}

		[Test]
		public void RenderRedLine()
		{
			CreateLookAtCamera(Vector3D.UnitY, Vector3D.Zero);
			new Line3D(-Vector3D.UnitX, Vector3D.UnitX, Color.Red);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateLine3D()
		{
			CreateLookAtCamera(Vector3D.One * 4.0f, Vector3D.Zero);
			var entity = new Line3D(Vector3D.Zero, Vector3D.One, Color.Red);
			Assert.AreEqual(Vector3D.Zero, entity.StartPoint);
			Assert.AreEqual(Vector3D.One, entity.EndPoint);
		}

		[Test, CloseAfterFirstFrame]
		public void SetLine3DPoints()
		{
			CreateLookAtCamera(Vector3D.One * 4.0f, Vector3D.Zero);
			var entity = new Line3D(Vector3D.Zero, Vector3D.Zero, Color.Red)
			{
				StartPoint = Vector3D.UnitX,
				EndPoint = Vector3D.UnitY
			};
			Assert.AreEqual(Vector3D.UnitX, entity.StartPoint);
			Assert.AreEqual(Vector3D.UnitY, entity.EndPoint);
		}

		[Test, CloseAfterFirstFrame]
		public void SetLine3DPointList()
		{
			CreateLookAtCamera(Vector3D.One * 4.0f, Vector3D.Zero);
			var entity = new Line3D(Vector3D.Zero, Vector3D.Zero, Color.Red)
			{
				Points = new List<Vector3D> { Vector3D.UnitZ, Vector3D.UnitY }
			};
			Assert.AreEqual(Vector3D.UnitZ, entity.StartPoint);
			Assert.AreEqual(Vector3D.UnitY, entity.EndPoint);
		}

		[Test, CloseAfterFirstFrame]
		public void RenderingHiddenLineDoesNotThrowException()
		{
			var camera = Camera.Use<LookAtCamera>();
			camera.Position = Vector3D.UnitZ;
			new Line3D(-Vector3D.UnitX, Vector3D.UnitX, Color.Red) { IsVisible = false };
			Assert.DoesNotThrow(() => AdvanceTimeAndUpdateEntities());
		}
	}
}